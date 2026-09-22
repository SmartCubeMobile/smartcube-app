using System.Net.Http;
using MoreLinq;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using static SmartCubeMobile.FrontEndGUI;




#if WINFORMS
using System.Windows;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

#if WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading;
#endif

#if ANDROIDX
using AndroidX.AppCompat.App;
#endif

namespace SmartCubeMobile
{
    /// <summary>
    /// Interaction logic for MainMeter.xaml
    /// </summary>
    public class CryptoBollocksV2025
    {
        internal static string wherewereat = "";

        [RequiresUnreferencedCode("Calls SmartCubeMobile.CryptoBollocksV2025.User_Click(AppCompatActivity, MainViewModel, FinanceViewModel, List<V2Transactions>, String, Int16, Int16, Char, List<DanApiKey>, List<Transaction_Groups>, List<Transaction_Types>)")]
        internal static async Task CryptoMate(//object sender, RoutedEventArgs e,
#if WINFORMS
                                                SmartDashboard.MainProcess components,
                                                RichTextBox textBoxConsole,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string start_url,
                                                string exchange,
                                                short institutionCode,
                                                short brandCode,
                                                char categoryCode,  // Because it can only be 'C'
                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                List<SmartFinance.DanApiKey> apikeys)

        {
            switch (brandCode)
            {
                case 20001:
                    // Coinbase

                    financeviewmodel.v2accountsList = new List<SmartCrypto.V2Accounts>();
                    financeviewmodel.v3accountsList = new List<SmartCrypto.V3Accounts>();
                    financeviewmodel.v2addressesList = new List<SmartCrypto.V2Addresses>();
                    List<SmartCrypto.V2Transactions> v2transactionsList = new List<SmartCrypto.V2Transactions>();
                    // Doesn't appear to be needed
                    financeviewmodel.v2exchangeratesList = new List<SmartCrypto.V2ExchangeRates>();


                    //herewereat = "GetGoing";
                    //financeviewmodel.TransActivityEnabled = 
                    //financeviewmodel.AcctsActivityEnabled = false;

                    if (financeviewmodel.financeToken == CancellationToken.None)
                    {
                        financeviewmodel.financeCts = new CancellationTokenSource();
                        financeviewmodel.financeToken = financeviewmodel.financeCts.Token;
                        //TextBlockUpdateFinance(ourviewmodel, "Finance token: " + financeviewmodel.financeToken.WaitHandle.SafeWaitHandle.GetHashCode().ToString());
                    }
                    //financeviewmodel.financeToken = financeviewmodel.financeCts.Token;


                    if (!await User_Click(
#if WINFORMS
                                components,
                                textBoxConsole,
#endif
#if ANDROIDX
                                meterActivity,
#endif
                                ourviewmodel,
                                financeviewmodel,
                                v2transactionsList,
                                exchange,
                                institutionCode,
                                brandCode,
                                categoryCode,
                                apikeys,
                                transaction_groupsFound,
                                transaction_typesFound))
                    {

#if WPF
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Scrape failed");
#endif
                        return;// false;
                    }
                    else
                    {
#if WPF
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Scrape succeeded");
#endif

                    }
                    break;
                default:
                    break;
            }
            return;// true;
        }

        [RequiresUnreferencedCode("Calls SmartCubeMobile.SmartDanV2025.RebuildXRPLedger(MainViewModel, FinanceViewModel, List<V2Transactions>, Int16, Int16, String, String, List<Wallet>)")]
        internal static async Task<bool> User_Click(
#if WINFORMS
                                        SmartDashboard.MainProcess components,
                                        RichTextBox textBoxConsole,
#endif
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        List<SmartCrypto.V2Transactions> v2transactionsList,
                                        string exchange,
                                        short institutionCode,
                                        short brandCode,
                                        char categoryCode,
                                        List<SmartFinance.DanApiKey> apikeys,
                                        List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                        List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            ourviewmodel.wherewereat = "User clicked";
#if WPF
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, wherewereat);
#endif
            //string brokerageaccountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";

            string V3accountsEndpoint = "api.coinbase.com/api/v3/brokerage/accounts";
            string V2accountsEndpoint = "api.coinbase.com/v2/accounts";
            string V2userEndpoint = "api.coinbase.com/v2/user";

            financeviewmodel.cryptouserV2 = new SmartCrypto.V2User();

            List<string> accountsDone = new List<string>();
            //accountsDone.Add("NewAccount");
            //List<string> isitDone = new List<string>(
            //    from Done in accountsDone
            //    where Done == "NewAccount"
            //    select Done);
            //if (isitDone.Count > 0)
            //{
            //    Console.WriteLine("We done it");
            //}



            foreach (SmartFinance.DanApiKey danApiKey in apikeys)
            {

                // The way this works is that for every set of API keys, I get
                // a series of Accounts, and for each Account in THAT series
                // I get a list of Transactions.
                // So every time I start an Account in the series, I need to 'add'
                // in that Account and 'add' in the Transactions it finds
                // What's complicated about that???
                //
                // Now to solve the 'missing transactions problem' i.e. the ones
                // that XRPLedger could find, but I couldn't, I need to include
                // EVERY set of API Keys that I can lay my hands on ... and that
                // includes mine!  Hence I cannot do a test on API Key Username
                // = to the executing User!  It could be that some EXTERNAL person
                // has sent XRP to Daniel ... without him knowing about it!

                if (danApiKey.ApiKey != null &&
                        danApiKey.ApiSecret != null)
                {
                    financeviewmodel.v2userList.Clear();
                    string cbPrivateKey = SmartDanV2025.parseKey(danApiKey.ApiSecret);
                    // Get the User info
                    string userToken = SmartDanV2025.generateToken(financeviewmodel,
                                                                    danApiKey.ApiKey,
                                                                    cbPrivateKey,
                                                                    $"GET {V2userEndpoint}");
                    string userData = await SmartBobV2017.CallApiGET($"https://{V2userEndpoint}",
                                                                            ourviewmodel.cookies,
                                                                            ourviewmodel.timespanTimeout,
                                                                            ourviewmodel.quitCts.Token,
                                                                            err => ourviewmodel.errorMessage = err,
                                                                            userToken,      // Its a different token from the one above!!!
                                                                            financeviewmodel.cbversion);
                    SmartDanV2025.AnalyzeUserV2(userData, financeviewmodel.v2userList, danApiKey.Username, danApiKey.AccessId);
                    if (financeviewmodel.v2userList.Count != 1)
                    {
#if CRYPTOS
                        Console.WriteLine("Lookup user failed");
#endif
#if WPF
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Lookup user failed");
#endif
                        continue;   // There may be others
                    }
                    if (financeviewmodel.v2userList.First().EMAIL != danApiKey.AccessId)
                    {
#if CRYPTOS
                        Console.WriteLine("No email match");
#endif
#if WPF
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "No email match");
#endif
                        continue;   // There may be others
                    }
                    // All is good thus far                        
                    wherewereat = danApiKey.AccessId;
                    DateTime accountCreated = financeviewmodel.v2userList.First().CREATED_AT;  // Not sure UTC time

#if WPF
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, wherewereat);
#endif
                    // Does this ("Residential") Address already exist?
                    // Step 2 - Check for existing address
                    // Do User addresses                    
#if WPF
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, wherewereat);
#endif
                    SmartDanV2025.RebuildUserAddresses(ourviewmodel,
                                                        financeviewmodel.v2userList.First().EMAIL,
                                                        financeviewmodel.v2userList.First().CREATED_AT,
                                                        financeviewmodel.v2userList[0].ID,
                                                        exchange);

                    // We've found one, now do we need to add it to Accounts?
                    short accountOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, financeviewmodel.v2userList.First().NATIVE_CURRENCY);

                    List<SmartFinance.Accounts> accounts_found =
                        SmartSpikeFinanceV2017.Finance_Lookup_AccountsCategory(ourviewmodel,
                                                                        financeviewmodel,
                                                                        institutionCode,
                                                                        brandCode,
                                                                        categoryCode,
                                                                        danApiKey.Exchange,
                                                                        financeviewmodel.v2userList[0].ID,
                                                                        financeviewmodel.v2userList[0].EMAIL,
                                                                        accountOrdinal);
                    if (accounts_found.Count == 0)
                    {
                        // Dummy these two up
                        int balance = 0;    // Zero balance to start with
                        char status = SmartParametersV2016.AccountStatus; // O = Open
                        // Its not in the Accounts List so add it in
                        SmartFinance.Accounts account_record =
                            SmartFinanceV2025.Account_Template(ourviewmodel,
                                                                financeviewmodel,
                                                                institutionCode,
                                                                brandCode,
                                                                danApiKey.Exchange,
                                                                financeviewmodel.v2userList[0].ID,
                                                                financeviewmodel.v2userList[0].EMAIL,
                                                                accountCreated,
                                                                categoryCode,
                                                                accountOrdinal,
                                                                financeviewmodel.v2userList[0].NAME,   // Title
                                                                balance,
                                                                status);
                        financeviewmodel.PLO.finance_accounts_changesList.Add(account_record);
                        // Do this now so the Account table is up to date
                        if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                                            financeviewmodel,
                                                                            true,
                                                                            SmartParametersV2016.sqliteformat))
                        {
                            financeviewmodel.errorMessage = "Cannot update accounts(1) - cannot continue";
#if WPF
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
#endif
                            // This is a bit fatal, because if we can't do this, there's
                            // every chance we can't do lots of things we need to further on
                            return false;
                        }
                    }

                    financeviewmodel.cryptouserV2 = new SmartCrypto.V2User();
                    string version = "2023-11-01";

                    wherewereat = danApiKey.AccessId + " V2 Accounts";

                    List<JsonDocument> accounts2 = await GetAccountsV2(
                        ourviewmodel,
                        financeviewmodel,
                        danApiKey.ApiKey,
                        danApiKey.ApiSecret,
                        version,
                        V2accountsEndpoint);

                    foreach (JsonDocument account in accounts2)
                    {
                        JsonElement root = account.RootElement;

                        string accountId = root.GetProperty("id").GetString()!;
                        if (!accountsDone.Contains(accountId))
                        {
                            string name = root.GetProperty("name").GetString()!;

                            SmartDanV2025.AnalyzeAccountV2(
                                root.ToString(),
                                financeviewmodel.v2accountsList,
                                danApiKey.Username,
                                danApiKey.Exchange,
                                danApiKey.AccessId);

                            // Addresses
                            wherewereat = danApiKey.AccessId + " V2 Addresses";
                            string addressesEndpoint = V2accountsEndpoint + "/" + accountId + "/addresses";

                            string addressesToken = SmartDanV2025.generateToken(
                                financeviewmodel,
                                danApiKey.ApiKey,
                                cbPrivateKey,
                                $"GET {addressesEndpoint}");

                            string addressesData = await SmartBobV2017.CallApiGET(
                                $"https://{addressesEndpoint}",
                                ourviewmodel.cookies,
                                ourviewmodel.timespanTimeout,
                                ourviewmodel.quitCts.Token,
                                err => ourviewmodel.errorMessage = err,
                                addressesToken,
                                financeviewmodel.cbversion);

                            SmartDanV2025.AnalyzeAddressV2(
                                addressesData,
                                financeviewmodel.v2addressesList,
                                danApiKey.Username,
                                danApiKey.Exchange,
                                danApiKey.AccessId,
                                accountId);

                            // Transactions
                            string transactionsEndpoint = V2accountsEndpoint + "/" + accountId + "/transactions";
                            await GetAllTransactions(
                                ourviewmodel,
                                financeviewmodel,
                                v2transactionsList,
                                accountId,
                                danApiKey.ApiKey,
                                danApiKey.ApiSecret,
                                version,
                                transactionsEndpoint,
                                danApiKey.Username,
                                danApiKey.Exchange,
                                danApiKey.AccessId,
                                accountCreated);

                            accountsDone.Add(accountId);
                        }
                    }

                    // Repeat for V3 accounts
                    List<JsonDocument> accounts3 = await GetAccountsV3(
                        ourviewmodel,
                        financeviewmodel,
                        danApiKey.ApiKey,
                        danApiKey.ApiSecret,
                        version,
                        V3accountsEndpoint);

                    foreach (JsonDocument account in accounts3)
                    {
                        JsonElement root = account.RootElement;

                        string accountId = root.GetProperty("uuid").GetString()!;
                        if (!accountsDone.Contains(accountId))
                        {
                            string name = root.GetProperty("name").GetString()!;

                            SmartDanV2025.AnalyzeAccountV3(
                                root.ToString(),
                                financeviewmodel.v3accountsList,
                                danApiKey.Username,
                                danApiKey.Exchange,
                                danApiKey.AccessId);

                            // V3 uses V2 transactions endpoint
                            string transactionsEndpoint = V2accountsEndpoint + "/" + accountId + "/transactions";
                            await GetAllTransactions(
                                ourviewmodel,
                                financeviewmodel,
                                v2transactionsList,
                                accountId,
                                danApiKey.ApiKey,
                                danApiKey.ApiSecret,
                                version,
                                transactionsEndpoint,
                                danApiKey.Username,
                                danApiKey.Exchange,
                                danApiKey.AccessId,
                                accountCreated);

                            accountsDone.Add(accountId);
                        }
                    }                    
                }
            }

            // The scrape has ended

            wherewereat = "Rebuilding";
#if WPF
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, wherewereat);
#endif
            // Accounts
            SmartDanV2025.RebuildCryptoAccounts(financeviewmodel);

            // Addresses
            SmartDanV2025.RebuildCryptoAddresses(ourviewmodel,
                                                    financeviewmodel,
                                                    institutionCode,
                                                    brandCode);


            wherewereat = "Exchange Rates";
            // Get the exchange rates
            if (await ExtractExchangeRates(ourviewmodel, financeviewmodel))
            {
#if CRYPTOS
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Crypto Exchange Rates refreshed");
#endif
#if WPF
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Crypto Exchange Rates refreshed");
#endif
                // Build the Transaction list and the Currencies list
                SmartDanV2025.RebuildCryptoTransactions(ourviewmodel,
                                                        financeviewmodel,
                                                        v2transactionsList,
                                                        "",
                                                        institutionCode,
                                                        brandCode);
            }

            // SmartSwitch Addresses
            SmartDanV2025.RebuildPLOCategories(ourviewmodel,
                                            financeviewmodel,
                                            SmartParametersV2016.Cryptos);

            SmartDanV2025.RebuildPLOCategoryTypes(ourviewmodel,
                                            financeviewmodel,
                                            SmartParametersV2016.Cryptos);

            wherewereat = "PLO Transactions";
#if WPF
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, wherewereat);
#endif


            await SmartDanV2025.RebuildPLOTransactions(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                financeviewmodel,
                                                categoryCode,
                                                institutionCode,
                                                brandCode,
                                                transaction_groupsFound,
                                                transaction_typesFound);

            SmartDanV2025.RebuildPLOAddresses(ourviewmodel,
                                                financeviewmodel,
                                                institutionCode,
                                                brandCode);
            SmartDanV2025.RebuildPLOAccounts(ourviewmodel,
                                                financeviewmodel,
                                                institutionCode,
                                                brandCode,
                                                SmartParametersV2016.Cryptos);

            List<SmartCrypto.Wallet> xrpuniquewallets = await SmartDanV2025.XRPFindWallets(ourviewmodel,
                                                                                    financeviewmodel,
                                                                                    v2transactionsList,
                                                                                    ourviewmodel.UserName,
                                                                                    financeviewmodel.financeToken,
                                                                                    SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, "XRP"),
                                                                                    "XRP");
#if CRYPTOS
            // This is where the User does his Wallet Name association
            foreach (SmartCrypto.Wallet wallet in xrpuniquewallets)
            {
                switch (wallet.CRYPTO_DESTINATION)
                {
                    case "rKHhfp7f5oYLZXtjQLL8BHEc47yccEbSpx":
                        wallet.CRYPTO_WALLET_NAME = "Hot Wallet";
                        break;
                    case "rfb6rwWPgm44EPRNrWYKcFzNZZZtAQC2ri":
                        wallet.CRYPTO_WALLET_NAME = "Tangem Wallet";
                        break;
                    case "rh6hEVLyLZmpkhhcoivjvMJnti4uQ7wkXR":
                        wallet.CRYPTO_WALLET_NAME = "Arculus Wallet";
                        break;
                    default:
                        wallet.CRYPTO_WALLET_NAME = "No idea";
                        break;

                }
            }
#endif
            wherewereat = "ETH Find Wallets";

            List<SmartCrypto.Wallet> ethuniquewallets = SmartDanV2025.ETHFindWallets(ourviewmodel,
                                                                                    financeviewmodel,
                                                                                    ourviewmodel.UserName,
                                                                                    financeviewmodel.financeToken);


            wherewereat = "ETH Ledger";
            if (!await SmartDanV2025.RebuildETHLedger(ourviewmodel,
                                            financeviewmodel,
                                            v2transactionsList,
                                            institutionCode,
                                            brandCode,
                                            "ETH",
                                            "https://api.etherscan.io/api",
                                            ethuniquewallets))
            {
                return false;
            }




            ourviewmodel.wherewereat = "XRP Ledger";

            if (!await SmartDanV2025.RebuildXRPLedger(ourviewmodel,
                                            financeviewmodel,
                                            v2transactionsList,
                                            institutionCode,
                                            brandCode,
                                            "XRP",
                                            "https://s1.ripple.com:51234/",
                                            xrpuniquewallets))
            {
                return false;
            }

            if (!await SmartDanV2025.RebuildXRPWalletTotals(ourviewmodel,
                                                        financeviewmodel,
                                                        xrpuniquewallets,
                                                        financeviewmodel.financeToken,
                                                        institutionCode,
                                                        brandCode))
            {
                return false;
            }

            if (!SmartDanV2025.RebuildETHWalletTotals(ourviewmodel,
                                                        financeviewmodel,
                                                        ethuniquewallets,
                                                        financeviewmodel.financeToken,
                                                        institutionCode,
                                                        brandCode))
            {
                return false;
            }

#if WPF  || WINUI
            financeviewmodel.CryptoVisible = Visibility.Visible;
#endif

            //financeviewmodel.rateEnabled = true;
            ourviewmodel.wherewereat = "Finished";
#if WPF
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.wherewereat);
#endif
            return true;
        }

        internal static double FindRate(FinanceViewModel financeviewmodel,
                                        short currencyOrdinal)
        {
            double rate = 1;
            List<SmartCrypto.V2ExchangeRates> rates_found =
                        new List<SmartCrypto.V2ExchangeRates>
                        (from Rate in financeviewmodel.v2exchangeratesList
                         where Rate.NAME_ORDINAL == currencyOrdinal
                         select Rate);
            if (rates_found.Count > 0)
            {
                rate = Convert.ToDouble(rates_found.First().RATE);
                if (rate != 0)
                {
                    rate = 1.0 / rate;
                }
            }
            return rate;
        }
        
        internal static async Task<List<JsonDocument>> GetAccountsV2(
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string apiKey,
                                        string apiSecret,
                                        string apiVersion,
                                        string v2Endpoint)
        {
            string requestPath = v2Endpoint;

            // CallCoinbaseApi now returns a JsonDocument
            JsonDocument response = await CallCoinbaseApi(
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    requestPath,
                                                    HttpMethod.Get,
                                                    apiKey,
                                                    apiSecret,
                                                    apiVersion,
                                                    v2Endpoint);

            var accounts = new List<JsonDocument>();

            // Navigate to the "data" property
            if (response.RootElement.TryGetProperty("data", out JsonElement dataElement) &&
                dataElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement acc in dataElement.EnumerateArray())
                {
                    // Wrap each JsonElement into a new JsonDocument
                    using var doc = JsonDocument.Parse(acc.GetRawText());
                    accounts.Add(doc); // Add a copy of the account
                }
            }
            return accounts;
        }

        internal static async Task<List<JsonDocument>> GetAccountsV3(
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string apiKey,
                                            string apiSecret,
                                            string apiVersion,
                                            string v3Endpoint)
        {
            string requestPath = v3Endpoint;

            // CallCoinbaseApi returns a JsonDocument
            JsonDocument response = await CallCoinbaseApi(
                                                ourviewmodel,
                                                financeviewmodel,
                                                requestPath,
                                                HttpMethod.Get,
                                                apiKey,
                                                apiSecret,
                                                apiVersion,
                                                v3Endpoint);

            var accounts = new List<JsonDocument>();

            // Navigate to the "accounts" array
            if (response.RootElement.TryGetProperty("accounts", out JsonElement accountsElement) &&
                accountsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement acc in accountsElement.EnumerateArray())
                {
                    // Wrap each JsonElement in a new JsonDocument
                    using var doc = JsonDocument.Parse(acc.GetRawText());
                    accounts.Add(doc);
                }
            }

            return accounts;
        }
        //internal static async Task GetAllTransactions(MainViewModel ourviewmodel,
        //                                        FinanceViewModel financeviewmodel,
        //                                        List<SmartCrypto.V2Transactions> v2transactionsList,
        //                                        string accountId,
        //                                        string apiKey,
        //                                        string apiSecret,
        //                                        string apiVersion,
        //                                        string v2Endpoint,
        //                                        string username,
        //                                        string exchange,
        //                                        string accessId,
        //                                        DateTime accountCreated)
        //{
        //    string endpoint = $"accounts/{accountId}/transactions";
        //    string url = v2Endpoint;
        //    bool hasNextPage = true;

        //    while (hasNextPage)
        //    {
        //        JObject response = await CallCoinbaseApi(ourviewmodel,
        //                                                financeviewmodel,
        //                                                url,
        //                                                HttpMethod.Get,
        //                                                apiKey,
        //                                                apiSecret,
        //                                                apiVersion,
        //                                                v2Endpoint);
        //        // If there is no 'data' then transactions will be null
        //        JToken transactions = response["data"];
        //        if (transactions != null)
        //        {
        //            foreach (JToken tx in transactions)
        //            {
        //                string id = tx["id"]?.ToString();
        //                string type = tx["type"]?.ToString();
        //                string status = tx["status"]?.ToString();
        //                string amount = tx["amount"]?["amount"]?.ToString();
        //                string currency = tx["amount"]?["currency"]?.ToString();
        //                string createdAt = tx["created_at"]?.ToString();
        //                //if (id.Substring(0,2) == "0x")
        //                //{
        //                //    Console.WriteLine("Here");
        //                //}
        //                //if (currency == "ETH")
        //                //{
        //                //    Console.WriteLine("here");
        //                //}
        //                SmartDanV2025.AnalyzeTransactionV2(tx.ToString(), v2transactionsList, username, exchange, accessId, accountId, accountCreated);

        //            }
        //            string nextUri = response["pagination"]?["next_uri"]?.ToString();
        //            hasNextPage = !string.IsNullOrEmpty(nextUri);
        //            url = @"api.coinbase.com/" + nextUri?.TrimStart('/');
        //        }
        //    }
        //    return;
        //}

        internal static async Task GetAllTransactions(
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    List<SmartCrypto.V2Transactions> v2transactionsList,
                                    string accountId,
                                    string apiKey,
                                    string apiSecret,
                                    string apiVersion,
                                    string v2Endpoint,
                                    string username,
                                    string exchange,
                                    string accessId,
                                    DateTime accountCreated)
        {
            string endpoint = $"accounts/{accountId}/transactions";
            string url = v2Endpoint;
            bool hasNextPage = true;

            while (hasNextPage)
            {
                JsonDocument response = await CallCoinbaseApi(
                    ourviewmodel,
                    financeviewmodel,
                    url,
                    HttpMethod.Get,
                    apiKey,
                    apiSecret,
                    apiVersion,
                    v2Endpoint);

                JsonElement root = response.RootElement;

                // If there is no 'data' then transactions will be null
                if (root.TryGetProperty("data", out JsonElement transactions) &&
                    transactions.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement tx in transactions.EnumerateArray())
                    {
                        string id = tx.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                        string type = tx.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : null;
                        string status = tx.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : null;

                        string amount = null;
                        string currency = null;

                        if (tx.TryGetProperty("amount", out JsonElement amountObj) &&
                            amountObj.ValueKind == JsonValueKind.Object)
                        {
                            amount = amountObj.TryGetProperty("amount", out var amt) ? amt.GetString() : null;
                            currency = amountObj.TryGetProperty("currency", out var cur) ? cur.GetString() : null;
                        }

                        string createdAt = tx.TryGetProperty("created_at", out var createdProp)
                            ? createdProp.GetString()
                            : null;

                        // Pass raw JSON to your existing analyzer
                        SmartDanV2025.AnalyzeTransactionV2(
                            tx.GetRawText(),
                            v2transactionsList,
                            username,
                            exchange,
                            accessId,
                            accountId,
                            accountCreated);
                    }

                    // Handle pagination
                    string nextUri = null;

                    if (root.TryGetProperty("pagination", out JsonElement pagination) &&
                        pagination.ValueKind == JsonValueKind.Object &&
                        pagination.TryGetProperty("next_uri", out JsonElement nextUriProp))
                    {
                        nextUri = nextUriProp.GetString();
                    }

                    hasNextPage = !string.IsNullOrEmpty(nextUri);

                    if (hasNextPage)
                    {
                        url = "api.coinbase.com/" + nextUri.TrimStart('/');
                    }
                }
                else
                {
                    hasNextPage = false;
                }
            }
            return;
        }

        internal static async Task<JsonDocument> CallCoinbaseApi(
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string requestPath,
                                        HttpMethod method,
                                        string apiKey,
                                        string apiSecret,
                                        string apiVersion,
                                        string v2Endpoint)
        {
            string cbPrivateKey = SmartDanV2025.parseKey(apiSecret);

            // Generate the API token
            string accountToken = SmartDanV2025.generateToken(financeviewmodel, apiKey, cbPrivateKey, $"GET {v2Endpoint}");

            string accountsData = "";

            try
            {
                accountsData = await SmartBobV2017.CallApiGET(
                    $"https://{requestPath}",
                    ourviewmodel.cookies,
                    ourviewmodel.timespanTimeout,
                    ourviewmodel.quitCts.Token,
                    err => ourviewmodel.errorMessage = err,
                    accountToken,      // It's a different token from the one above
                    cbPrivateKey);
            }
            catch (Exception ex)
            {
#if WINFORMS
                Console.WriteLine(ex.ToString());
#endif
#if CRYPTOS || SMARTMAUI
                Console.WriteLine(ex.Message);
#endif
#if WPF  || WINUI || ANDROID
                financeviewmodel.errorMessage = ex.Message;
#endif
                accountsData = "";
            }

            Console.WriteLine(accountsData);

            try
            {
                // Test parsing
                using var testDoc = JsonDocument.Parse(accountsData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Return the actual JsonDocument
            return JsonDocument.Parse(accountsData);
        }

#if WPF  || WINUI
#if WPF 
        internal void GridViewColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void GridViewColumnHeader_Click(object sender, EventArgs e)
        {
#endif
#if WPF
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked.Column != null)
            {
                GridViewColumnHeader gvch = headerClicked.Column.Header as GridViewColumnHeader;
                string sortBy = gvch.Content.ToString();
                SmartDanV2025.RebuildCryptoTransactions(ourviewmodel,
                                                        financeviewmodel,
                                                        new List<SmartCrypto.V2Transactions>(),
                                                        sortBy);

            }
#endif
            return;
        }
#endif

#if WPF
        internal void TabItemChanged(object sender, SelectionChangedEventArgs e,
                                    FinanceViewModel financeviewmodel)
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
                                financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
                                financeviewmodel.TransactionsRibbonVisible = Visibility.Visible;
                                financeviewmodel.TransActivityEnabled = true;
                                financeviewmodel.AcctsActivityEnabled = false;
                                break;
                            case "Accounts":
                                financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
                                financeviewmodel.AccountsRibbonVisible = Visibility.Visible;
                                financeviewmodel.TransActivityEnabled = false;
                                financeviewmodel.AcctsActivityEnabled = true;
                                break;
                            case "Addresses":
                                financeviewmodel.TransactionsRibbonVisible = Visibility.Hidden;
                                financeviewmodel.AccountsRibbonVisible = Visibility.Hidden;
                                financeviewmodel.TransActivityEnabled = false;
                                financeviewmodel.AcctsActivityEnabled = false;
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
        //        SmartCrypto.V2CryptoTotals selectedItem = CryptoTotals.SelectedItem as SmartCrypto.V2CryptoTotals;
        //        string currency = selectedItem.CRYPTO_CURRENCY.ToString();
        //        string rate = selectedItem.RATE.ToString();

        //        financeviewmodel.FinanceCryptoTransactionsSpecific =
        //            new List<SmartFinance.CryptoTransactionsView>
        //            (from Currencies in financeviewmodel.FinanceCryptoTransactions
        //             where Currencies.CRYPTO_CURRENCY == currency
        //             orderby Currencies.CREATED_AT
        //             select Currencies);
        //        financeviewmodel.PopupCurrency = currency + "(" +
        //            financeviewmodel.FinanceCryptoTransactionsSpecific.Count.ToString() +
        //                                                              ")";                                                            
        //        financeviewmodel.PopupRate = "Rate now: " + Convert.ToDouble(rate).ToString("F3");

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


#if WINFORMS || WPF  || WINUI
        internal void TransActivityChecked(object sender, EventArgs e, 
                                           MainViewModel ourviewmodel,
                                           FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
                e != null)
            {
                if (financeviewmodel.TransActivityEnabled)
                {
                    CheckBox abc = sender as CheckBox;
                    string tag = abc.Tag.ToString();
                    switch (tag)
                    {
                        case "F":
#if WINFORMS
                            financeviewmodel.transfiat = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.transfiat = (bool)(abc.IsChecked);
#endif
                            break;
                        case "B":
#if WINFORMS
                            financeviewmodel.buy = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.buy = (bool)(abc.IsChecked);
#endif
                            break;
                        case "S":
#if WINFORMS
                            financeviewmodel.sell = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.sell = (bool)(abc.IsChecked);
#endif
                            break;
                        case "T":
#if WINFORMS
                            financeviewmodel.trade = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.trade = (bool)(abc.IsChecked);
#endif
                            break;
                        case "X":
#if WINFORMS
                            financeviewmodel.send = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.send = (bool)(abc.IsChecked);
#endif
                            break;
                        default:
                            break;
                    }
                    SmartDanV2025.RebuildCryptoTransactions(ourviewmodel, 
                                                            financeviewmodel,
                                                            new List<SmartCrypto.V2Transactions>());
                }
            }
            return;
        }


        internal void AcctsActivityChecked(object sender, EventArgs e,
                                           FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
                e != null)
            {
                if (financeviewmodel.AcctsActivityEnabled)
                {
                    CheckBox abc = sender as CheckBox;
                    string tag = abc.Tag.ToString();
                    switch (tag)
                    {
                        case "F":
#if WINFORMS
                            financeviewmodel.acctsfiat = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.acctsfiat = (bool)(abc.IsChecked);
#endif
                            break;
                        case "C":
#if WINFORMS
                            financeviewmodel.crypto = (bool)(abc.Checked);
#endif
#if WPF
                            financeviewmodel.crypto = (bool)(abc.IsChecked);
#endif
                            break;
                        default:
                            break;
                    }
                    SmartDanV2025.RebuildCryptoAccounts(financeviewmodel);
                }
            }
            return;
        }
#endif

        internal static async Task<bool> ExtractExchangeRates(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            string V2exchangeratesEndpoint = "api.coinbase.com/v2/exchange-rates?currency=" + financeviewmodel.WorkingCurrency;
            string exchangerateData = await SmartBobV2017.CallApiGET($"https://{V2exchangeratesEndpoint}",
                                                                    ourviewmodel.cookies,
                                                                    ourviewmodel.timespanTimeout,
                                                                    ourviewmodel.quitCts.Token,
                                                                    err => ourviewmodel.errorMessage = err,
                                                                    "",
                                                                    "");
            SmartDanV2025.AnalyzeExchangeRates(ourviewmodel,
                                                    financeviewmodel.WorkingCurrency,
                                                    exchangerateData,
                                                    financeviewmodel.v2exchangeratesList);

            return true;
        }

#if WINFORMS || WPF  || WINUI
#if WINFORMS || WPF 
        internal void Leave_Click(object sender, EventArgs e,
                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void Leave_Click(object sender, EventArgs e,
                                    FinanceViewModel financeviewmodel)
        {
#endif
            if (sender != null &&
                e != null)
            {
#if WPF
                App.Current.Shutdown();
#endif
            }
        }
#endif

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

#if WPF
        internal static void FinanceTransactionsMouseDoubleClick(object sender, MouseAction e,
                                                    FinanceViewModel financeviewmodel)
        {
            return;
        }
#endif
    }
}