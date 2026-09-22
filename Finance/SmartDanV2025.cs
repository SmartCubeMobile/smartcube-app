//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is

//using System.Net.Http;        // No more of this absolute bollocks shit

using System.Text.Json;

#if WINFORMS
using MoreLinq;
using System.Net.Http;
using System.Security.Cryptography;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using Nethereum.Util;
#endif


#if WPF
using MoreLinq;
// Crypto shit
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Jose;
using System.Net.Http;
using System.IO;
using Nethereum.Util;
using System.Numerics;
#endif

#if WINUI
using Windows.Devices.Input;
using OxyPlot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using System.Linq;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using System.Security.Cryptography;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Numerics;
using MoreLinq;
// Crypto shit
using System.IO;
using Nethereum.Util;
#endif

#if SMARTMAUI
using MoreLinq;
// Crypto shit
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Jose;
using System.Net.Http;
using System.IO;
using Nethereum.Util;
using System.Numerics;
#endif

#if ANDROIDX
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Jose;
using System.Numerics;
using Nethereum.Util;
using AndroidX.AppCompat.App;
using System.Text.Json.Nodes;
using System.Diagnostics.CodeAnalysis;

#endif

namespace SmartCubeMobile
{
    public class SmartDanV2025
    {
#if ANDROIDX
        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        //[RequiresUnreferencedCode("Calls SmartCubeMobile.SmartDanV2025.XRPLedgerScan(MainViewModel, FinanceViewModel, String, String, List<XRPLedgerTransaction>, String)")]
        
#endif
        internal static long UnixSequenceNo()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();    // UTC time
        }

        // XRP Ledger starts
        internal static async Task<bool> XRPLedgerScan(
                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                string xrpLedgerApiUrl,
                                string destAddress,
                                List<XRPLedgerTransaction> xrptrans,
                                string currencyDefault)
        {
            var requestBodyAccountTransactions = new
            {
                method = "account_tx",
                @params = new[]
                {
                    new
                    {
                        account = destAddress,
                        binary = false,
                        limit = 999
                    }
                }
            };

            try
            {
                // ✅ System.Text.Json serialization
                string jsonRequest = JsonSerializer.Serialize(requestBodyAccountTransactions);

                string postData = "";

                SmartRoutinesV2018.CreateUri(ourviewmodel, xrpLedgerApiUrl, "");

                string response = await SmartBobV2017.HTTPCLIENT_POST_ASYNC_JSON(
                    ourviewmodel,
                    ourviewmodel.TargetUrl,
                    financeviewmodel.financeToken,
                    jsonRequest,
                    postData,
                    "",
                    new Guid());

                if (string.IsNullOrEmpty(ourviewmodel.errorMessage) &&
                    !string.IsNullOrEmpty(response))
                {
                    // ✅ No change needed here if your decoder is already converted
                    XRPDecodeResponse(financeviewmodel, response, xrptrans, currencyDefault);
                }
            }
            catch (HttpRequestException ex)
            {
                financeviewmodel.errorMessage = "Request error: " + ex.Message;
                return false;
            }

            return true;
        }
        internal static DateTime ConvertLedgerDateToDateTime(long timestamp, int startYear)
        {
            // UNIX epoch is January 1, 1970
            // but
            // Etherscan epoch starts at January 1, 1970
            // but
            // Ripple epoch starts at January 1, 2000
            DateTime rippleEpoch = new DateTime(startYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Convert Ledger timestamp (seconds since January 1, startYear) to DateTime
            DateTime rippleDateTime = rippleEpoch.AddSeconds(timestamp);
            //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);

            return rippleDateTime;
        }


        internal static void XRPDecodeResponse(
                                                FinanceViewModel financeviewmodel,
                                                string responseBody,
                                                List<XRPLedgerTransaction> xrptrans,
                                                string currencyDefault)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("result", out JsonElement result))
                    return;

                if (!result.TryGetProperty("transactions", out JsonElement transactions))
                    return;

                foreach (JsonElement txWrapper in transactions.EnumerateArray())
                {
                    XRPLedgerTransaction item = null;

                    // ---- TX OBJECT ----
                    if (txWrapper.TryGetProperty("tx", out JsonElement tx))
                    {
                        item = new XRPLedgerTransaction
                        {
                            Memos = "",
                            Flags = ""
                        };

                        foreach (JsonProperty prop in tx.EnumerateObject())
                        {
                            string name = prop.Name;

                            switch (name)
                            {
                                case "Account":
                                    item.Account = prop.Value.ToString();
                                    break;

                                case "Amount":
                                    string amountStr = "";
                                    string currency = currencyDefault;

                                    if (prop.Value.ValueKind == JsonValueKind.Object)
                                    {
                                        // Token amount (issued currency)
                                        if (prop.Value.TryGetProperty("currency", out var cur))
                                            currency = cur.ToString();

                                        if (prop.Value.TryGetProperty("value", out var val))
                                            amountStr = val.ToString();
                                    }
                                    else
                                    {
                                        // XRP drops → convert to XRP
                                        amountStr = prop.Value.ToString();

                                        if (amountStr.Length < 6)
                                            amountStr = amountStr.PadLeft(7, '0');

                                        int characteristic = amountStr.Length - 6;
                                        amountStr = amountStr.Insert(characteristic, ".");
                                    }

                                    try
                                    {
                                        item.Amount = Convert.ToDecimal(amountStr);
                                        item.Currency = currency;
                                    }
                                    catch (Exception ex)
                                    {
                                        financeviewmodel.errorMessage = ex.Message + " " + amountStr;
                                        return;
                                    }
                                    break;

                                case "date":  // <= I'm not sure this is right
                                    item.Date = prop.Value.GetInt64();
                                    break;

                                case "DeliverMax":
                                    item.DeliverMax = prop.Value.ToString();
                                    break;

                                case "Destination":
                                    item.Destination = prop.Value.ToString();
                                    break;

                                case "Fee":
                                    item.Fee = prop.Value.ToString();
                                    break;

                                case "Flags":
                                    item.Flags = prop.Value.ToString();
                                    break;

                                case "hash":
                                    item.Hash = prop.Value.ToString();
                                    break;

                                case "inLedger":
                                    item.InLedger = prop.Value.ToString();
                                    break;

                                case "LastLedgerSequence":
                                    item.LasLedgerSequence = prop.Value.ToString();
                                    break;

                                case "ledger_index":
                                    item.LedgerIndex = prop.Value.ToString();
                                    break;

                                case "Memos":
                                    item.Memos = prop.Value.ToString();
                                    break;

                                case "Sequence":
                                    item.Sequence = prop.Value.ToString();
                                    break;

                                case "SigningPubKey":
                                    item.SigningPubKey = prop.Value.ToString();
                                    break;

                                case "TransactionType":
                                    item.TransactionType = prop.Value.ToString();
                                    break;

                                case "TxnSignature":
                                    item.TransactionSignature = prop.Value.ToString();
                                    break;

                                default:
                                    break;
                            }
                        }

                        item.NetworkName = SmartParametersV2016.XRPNETWORK;
                        xrptrans.Add(item);
                    }

                    // ---- VALIDATED FLAG ----
                    if (txWrapper.TryGetProperty("validated", out JsonElement validated))
                    {
                        if (validated.ValueKind == JsonValueKind.True && xrptrans.Count > 0)
                        {
                            xrptrans[xrptrans.Count - 1].Validated = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
        }

        // XRP Ledger ends

        // ETH Ledger Starts
        internal static async Task<bool> ETHLedgerScan(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string LedgerApiUrl,
                                            string destAddress,
                                            List<ETHLedgerTransaction> ethtrans,
                                            string currencyDefault)
        {
            string apiKey = @"T23AITUIC41I6PKFWIMAK8QN8GSRQ4VSZN";

            string url = LedgerApiUrl +
                         $"?module=account" +
                         $"&action=txlist" +
                         $"&address={destAddress}" +
                         $"&startblock=0" +
                         $"&endblock=99999999" +
                         $"&sort=asc" +
                         $"&apikey={apiKey}";
            //
            // Prepare the request payload for the account_info method
            //            
            Guid guid = Guid.NewGuid();
            try
            {
                string ethresponse = await SmartBobV2017.HTTPCLIENT_GET_ASYNC_JSONX(ourviewmodel,
                                            new Uri(url),
                                            financeviewmodel.financeToken,
                                            guid.ToString());
                if (ourviewmodel.errorMessage == "" &&
                    ethresponse != "")
                {
                    // Output the response in xrptrans (which will contain account info)  
                    ETHDecodeResponse(financeviewmodel, ethresponse, ethtrans, currencyDefault);
                }
            }
            catch (HttpRequestException ex)
            {
                financeviewmodel.errorMessage = "Request error: " + ex.Message;
                return false;
            }
            return true;
        }

        //internal static void ETHDecodeResponse(FinanceViewModel financeviewmodel,
        //                                    string responseBody,
        //                                    List<ETHLedgerTransaction> ethtrans,
        //                                    string currencyDefault)
        //{
        //    JObject json = JObject.Parse(responseBody);

        //    if (json["status"].ToString() == "1")
        //    {
        //        try
        //        {
        //            foreach (JToken tx in json["result"])
        //            {
        //                ETHLedgerTransaction item = new ETHLedgerTransaction()
        //                {
        //                    BlockHash = tx["blockHash"].ToString(),
        //                    BlockNumber = Convert.ToInt32(tx["blockNumber"]),
        //                    Confirmations = tx["confirmations"].ToString(),
        //                    ContactAddress = tx["contractAddress"].ToString(),
        //                    CumulativeGasUsed = Convert.ToDecimal(tx["cumulativeGasUsed"]),
        //                    From = tx["from"].ToString(),
        //                    FunctionName = tx["functionName"].ToString(),
        //                    Gas = Convert.ToDecimal(tx["gas"]),
        //                    GasPrice = Convert.ToDecimal(tx["gasPrice"]),
        //                    GasUsed = Convert.ToDecimal(tx["gasUsed"]),
        //                    Hash = tx["hash"].ToString(),
        //                    Input = "input",   // Data is WAY to long
        //                    IsError = Convert.ToInt16(tx["isError"]),
        //                    MethodId = tx["methodId"].ToString(),
        //                    Nonce = Convert.ToInt32(tx["nonce"]),
        //                    TransactionDate = ConvertLedgerDateToDateTime((long)(tx["timeStamp"]), 1970),
        //                    To = tx["to"].ToString(),
        //                    TransactionIndex = Convert.ToInt32(tx["transactionIndex"]),
        //                    TxReceiptStatus = Convert.ToInt32(tx["txreceipt_status"]),
        //                    Value = Convert.ToDecimal(tx["value"]),
        //                    NetworkName = SmartParametersV2016.ETHNETWORK
        //                };
        //                // We ONLY have to do this for Addresses,
        //                // Hash(es) are not checksummed therefore are always
        //                // returned in LOWERCASE so we have to convert our
        //                // Coinbase hashes to lowercase IF we are going to
        //                // compare them. Doh! Goes to show that Etherium/Crypto
        //                // is populated by the same set of chimps Microshit has ..
        //                item.From = ToChecksumAddress(item.From);
        //                item.To = ToChecksumAddress(item.To);
        //                // See if its a Transfer
        //                if (string.IsNullOrEmpty(item.FunctionName))
        //                {
        //                    EtherscanTransaction fuckme = new EtherscanTransaction()
        //                    {
        //                        From = tx["from"].ToString(),
        //                        To = tx["to"].ToString(),
        //                        Value = tx["value"].ToString(),
        //                        Input = tx["input"].ToString(),
        //                        FunctionName = tx["functionName"].ToString()
        //                    };
        //                    if (IsEthTransfer(fuckme))
        //                    {
        //                        item.FunctionName = "Transfer";
        //                    }
        //                }
        //                else
        //                {
        //                    // Look for a "("
        //                    int parenth = item.FunctionName.IndexOf("(");
        //                    if (parenth >= 0)
        //                    {
        //                        item.FunctionName = item.FunctionName.Substring(0, parenth);
        //                        item.FunctionName = char.ToUpper(item.FunctionName[0]) + item.FunctionName.Substring(1);
        //                    }
        //                }
        //                //if (item.Hash.Substring(0,2) == "0x")
        //                //{
        //                //    item.Hash = item.Hash.Substring(2);
        //                //}

        //                ethtrans.Add(item);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message.ToString());
        //        }
        //    }
        //    return;
        //}

        internal static void ETHDecodeResponse(
                                FinanceViewModel financeviewmodel,
                                string responseBody,
                                List<ETHLedgerTransaction> ethtrans,
                                string currencyDefault)
        {
            using JsonDocument json = JsonDocument.Parse(responseBody);
            JsonElement root = json.RootElement;

            if (root.TryGetProperty("status", out JsonElement statusProp) &&
                statusProp.GetString() == "1")
            {
                try
                {
                    if (root.TryGetProperty("result", out JsonElement result) &&
                        result.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement tx in result.EnumerateArray())
                        {
                            ETHLedgerTransaction item = new ETHLedgerTransaction()
                            {
                                BlockHash = tx.TryGetProperty("blockHash", out var bh) ? bh.GetString() : "",
                                BlockNumber = tx.TryGetProperty("blockNumber", out var bn) && bn.TryGetInt32(out int bnVal) ? bnVal : 0,
                                Confirmations = tx.TryGetProperty("confirmations", out var conf) ? conf.GetString() : "",
                                ContactAddress = tx.TryGetProperty("contractAddress", out var ca) ? ca.GetString() : "",
                                CumulativeGasUsed = tx.TryGetProperty("cumulativeGasUsed", out var cgu) && cgu.TryGetDecimal(out decimal cguVal) ? cguVal : 0,
                                From = tx.TryGetProperty("from", out var from) ? from.GetString() : "",
                                FunctionName = tx.TryGetProperty("functionName", out var fn) ? fn.GetString() : "",
                                Gas = tx.TryGetProperty("gas", out var gas) && gas.TryGetDecimal(out decimal gasVal) ? gasVal : 0,
                                GasPrice = tx.TryGetProperty("gasPrice", out var gp) && gp.TryGetDecimal(out decimal gpVal) ? gpVal : 0,
                                GasUsed = tx.TryGetProperty("gasUsed", out var gu) && gu.TryGetDecimal(out decimal guVal) ? guVal : 0,
                                Hash = tx.TryGetProperty("hash", out var hash) ? hash.GetString() : "",
                                Input = "input", // still intentionally trimmed
                                IsError = tx.TryGetProperty("isError", out var ie) && ie.TryGetInt16(out short ieVal) ? ieVal : (short)0,
                                MethodId = tx.TryGetProperty("methodId", out var mid) ? mid.GetString() : "",
                                Nonce = tx.TryGetProperty("nonce", out var nonce) && nonce.TryGetInt32(out int nonceVal) ? nonceVal : 0,
                                TransactionDate = tx.TryGetProperty("timeStamp", out var ts) && ts.TryGetInt64(out long tsVal)
                                    ? ConvertLedgerDateToDateTime(tsVal, 1970)
                                    : DateTime.MinValue,
                                To = tx.TryGetProperty("to", out var to) ? to.GetString() : "",
                                TransactionIndex = tx.TryGetProperty("transactionIndex", out var ti) && ti.TryGetInt32(out int tiVal) ? tiVal : 0,
                                TxReceiptStatus = tx.TryGetProperty("txreceipt_status", out var trs) && trs.TryGetInt32(out int trsVal) ? trsVal : 0,
                                Value = tx.TryGetProperty("value", out var val) && val.TryGetDecimal(out decimal valVal) ? valVal : 0,
                                NetworkName = SmartParametersV2016.ETHNETWORK
                            };

                            // Checksum addresses
                            item.From = ToChecksumAddress(item.From);
                            item.To = ToChecksumAddress(item.To);

                            // Determine function type
                            if (string.IsNullOrEmpty(item.FunctionName))
                            {
                                EtherscanTransaction temp = new EtherscanTransaction()
                                {
                                    From = tx.TryGetProperty("from", out var f2) ? f2.GetString() : "",
                                    To = tx.TryGetProperty("to", out var t2) ? t2.GetString() : "",
                                    Value = tx.TryGetProperty("value", out var v2) ? v2.GetString() : "",
                                    Input = tx.TryGetProperty("input", out var i2) ? i2.GetString() : "",
                                    FunctionName = tx.TryGetProperty("functionName", out var fn2) ? fn2.GetString() : ""
                                };

                                if (IsEthTransfer(temp))
                                {
                                    item.FunctionName = "Transfer";
                                }
                            }
                            else
                            {
                                int parenth = item.FunctionName.IndexOf("(");
                                if (parenth >= 0)
                                {
                                    item.FunctionName = item.FunctionName.Substring(0, parenth);
                                    item.FunctionName = char.ToUpper(item.FunctionName[0]) + item.FunctionName.Substring(1);
                                }
                            }

                            ethtrans.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return;
        }

        public static string ToChecksumAddress(string address)
        {
#if CRYPTOS
            var util = new AddressUtil();

            return util.ConvertToChecksumAddress(address);
#else
            return "";
#endif
        }

        public class EtherscanTransaction
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Value { get; set; } // In Wei
            public string Input { get; set; }
            public string FunctionName { get; set; }
        }

        public static bool IsEthTransfer(EtherscanTransaction tx)
        {
            bool isFunctionEmpty = string.IsNullOrEmpty(tx.FunctionName);
            bool isInputEmpty = tx.Input == "0x";
            bool hasToAddress = !string.IsNullOrEmpty(tx.To);

            BigInteger valueWei;
            bool hasValue = BigInteger.TryParse(tx.Value, out valueWei) && valueWei > 0;

            return isFunctionEmpty && isInputEmpty && hasValue && hasToAddress;
        }


        // ETH Ledger Ends

        // Crypto starts
        public static void RebuildUserAddresses(MainViewModel ourviewmodel,
                                            string emailAddress,
                                            DateTime created,
                                            string id,
                                            string exchange)
        {
            // Right at the start!
            // Is it in our PLO list
            List<SmartProfile.AddressesView> addresses_found = SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                        ourviewmodel.UserName,
                                                        emailAddress);
            if (addresses_found.Count == 0)
            {
                // Is it in our Changes list?
                addresses_found = new List<SmartProfile.AddressesView>
                    (from Address in ourviewmodel.SmartProfile.addressesview_changesList
                     where (Address.USERNAME == ourviewmodel.UserName &&
                     Address.UDPRN == emailAddress)
                     select Address);
                if (addresses_found.Count == 0)
                // i.e. we already know about it Just to be sure
                {
                    short areaCode = 99;
                    SmartProfile.AddressesView address = new SmartProfile.AddressesView()
                    {
                        USERNAME = ourviewmodel.UserName,
                        UDPRN = emailAddress,
                        RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        ADDRESS_CREATED = created,
                        AREA_CODE = areaCode,
                        BASIC = id,       // The one scraped from website (Bank, Utility, Water, Community Charge)
                        BUILDING_NAME = "",
                        BUILDING_NUMBER = "",
                        COUNTY = "",
                        DOUBLE_DEPENDANT_LOCALITY = "",
                        DEPENDANT_THOROUGHFARE = "",
                        DEPENDANT_LOCALITY = "",
                        ORGANIZATION = exchange,
                        POSTCODE_OUTWARD = "",
                        POSTCODE = "",
                        POBOX = "",
                        SUB_BUILDING_NAME = "",
                        THOROUGHFARE = "",
                        TOWN = "",
                        RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                        CHECKED = false,
                        Updated = false     // Its new
                    };
                    ourviewmodel.SmartProfile.addressesview_changesList.Add(address);
                }
            }
            return;
        }

        internal static void RebuildPLOCategories(MainViewModel ourviewmodel,
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
                        CHECKED = SmartParametersV2016.lastChecked,
                        Updated = false // Its a new
                    };
                    financeviewmodel.PLO.finance_categories_changesList.Add(newCategory);
                }
            }
            return;
        }

        internal static void RebuildPLOCategoryTypes(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode)
        {
            if (financeviewmodel.v2accountsList.Count > 0)
            {
                foreach (SmartCrypto.V2Accounts acct in financeviewmodel.v2accountsList)
                {
                    // Find the CategoryType
                    // First find the account currency ordinal
                    short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, acct.CURRENCY.CODE);
                    // Only do this if we have found a genuine currency
                    if (currencyOrdinal > 0)
                    {
                        // Is it in our PLO list?
                        List<SmartFinance.CategoryTypes> categorytypesFound = 
                            SmartSpikeFinanceV2017.Finance_Find_CategoryTypes(ourviewmodel,
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

        internal static int GetMonthsDifference(DateTime startDate, DateTime endDate)
        {
            int yearsDifference = endDate.Year - startDate.Year;
            int monthsDifference = endDate.Month - startDate.Month;

            // If the end month is before the start month, adjust the year difference
            if (monthsDifference < 0)
            {
                yearsDifference--;
                monthsDifference += 12; // Add 12 months to the monthsDifference
            }

            // Convert the total difference into months
            return yearsDifference * 12 + monthsDifference;
        }

        internal static DateTime GetFirstOfNextMonth(DateTime date)
        {
            // Add 1 month to the current date, and set the day to the 1st
            return new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1);
        }

        internal static SmartFinance.TransactionsCategories TransactionCategory_Template(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            short institution_code,
                                                                            short brand_code,
                                                                            string sortcode,
                                                                            string account_no,
                                                                            string UDPRN,
                                                                            DateTime statementDate,
                                                                            short statementNo,
                                                                            long sequenceNo,
                                                                            int random1,
                                                                            DateTime bookingDate,
                                                                            int isPaidIn,
                                                                            short[] transCodes,
                                                                            string description,
                                                                            string type,            // Crypto
                                                                            double cryptoAmount,
                                                                            short cryptoAmountOrdinal,
                                                                            double amount,
                                                                            short amountOrdinal,
                                                                            double balance,
                                                                            short balanceOrdinal,
                                                                            int balanceCreditDebit,
                                                                            int random2,
                                                                            bool what)
        {
            SmartFinance.TransactionsCategories transactionCategoryTemplate = new SmartFinance.TransactionsCategories()
            {
                USERNAME = ourviewmodel.UserName,
                CUBEFACE_CODE = financeviewmodel.cubeface_code,
                INSTITUTION_CODE = institution_code,
                BRAND_CODE = brand_code,
                SORTCODE = sortcode,
                ACCOUNT_NO = account_no,
                STATEMENT_DATE = statementDate,
                STATEMENT_NO = statementNo,
                UDPRN = UDPRN,
                RANDOMKEY1 = random1,
                SEQUENCE_NO = sequenceNo,
                TRANSACTION_DATE = bookingDate,                 // 0
                CREDITDEBIT_INDICATOR = isPaidIn,               // 1
                PTC_CODE = 0,            // Not yet assigned    // 2
                TRANSGROUP_CODE = transCodes[0],                // 3
                TRANSACTION_CODE = transCodes[1],               // 4
                DESCRIPTION = description,                      // 5
                TYPE = type,                                    // 6    Crypto
                CRYPTO_AMOUNT = cryptoAmount,                   // 7    Crypto
                CRYPTO_CURRENCY_ORDINAL = cryptoAmountOrdinal,  // 8    Crypto
                AMOUNT = amount,                                // 9
                AMOUNT_CURRENCY_ORDINAL = amountOrdinal,        // 10
                BALANCE_AMOUNT = balance,                       // 11
                BALANCE_CURRENCY_ORDINAL = balanceOrdinal,      // 12
                BALANCE_CREDITDEBIT_INDICATOR = balanceCreditDebit, //11
                RANDOMKEY2 = random2,
                Updated = what
            };
            return transactionCategoryTemplate;
        }

        internal static async Task<List<SmartCrypto.Wallet>> XRPFindWallets(
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    List<SmartCrypto.V2Transactions> v2transactionsList,
                                    string Username,
                                    CancellationToken cancellationToken,
                                    short currencyOrdinal,
                                    string currencyName)
        {
            List<SmartCrypto.Wallet> uniquewallets = new List<SmartCrypto.Wallet>();

            foreach (SmartCrypto.V2Transactions trans in v2transactionsList)
            {
                string transactionHash = trans.NETWORK.HASH;

                if (!string.IsNullOrEmpty(transactionHash) &&
                    trans.NETWORK.NETWORK_NAME == SmartParametersV2016.XRPNETWORK)
                {
                    string url = "https://api.xrpscan.com";
                    string wa = $"api/v1/tx/{transactionHash}";

                    try
                    {
                        SmartRoutinesV2018.CreateUri(ourviewmodel, url, wa);

                        string response = await SmartBobV2017.HTTPCLIENT_GET_ASYNC_JSONX(
                            ourviewmodel,
                            ourviewmodel.TargetUrl,
                            cancellationToken,
                            "",
                            new Guid());

                        if (string.IsNullOrEmpty(ourviewmodel.errorMessage) &&
                            !string.IsNullOrEmpty(response))
                        {
                            using JsonDocument jsonDoc = JsonDocument.Parse(response);
                            JsonElement root = jsonDoc.RootElement;

                            if (root.TryGetProperty("Destination", out JsonElement destElement))
                            {
                                string destination = destElement.GetString();

                                if (!string.IsNullOrEmpty(destination))
                                {
                                    SmartCrypto.Wallet wallet = new SmartCrypto.Wallet()
                                    {
                                        USERNAME = Username,
                                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                                        CRYPTO_DESTINATION = destination,
                                        CRYPTO_WALLET_NAME = "",
                                        CRYPTO_CURRENCY_ORDINAL = currencyOrdinal,
                                        CRYPTO_CURRENCY = currencyName,
                                        CRYPTO_AMOUNT = 0
                                    };

                                    uniquewallets.Add(wallet);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        financeviewmodel.errorMessage = ex.Message;
                        return uniquewallets;
                    }
                }
            }

            // Distinct wallets
            uniquewallets = uniquewallets
                .DistinctBy(w => new
                {
                    w.USERNAME,
                    w.CUBEFACE_CODE,
                    w.CRYPTO_DESTINATION
                })
                .ToList();

            return uniquewallets;
        }
        internal static  List<SmartCrypto.Wallet> ETHFindWallets(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            string Username,
                                                                            CancellationToken cancellationToken)
        {
            List<SmartFinance.CryptoTransactionsView> ethershit = new List<SmartFinance.CryptoTransactionsView>();

            List<SmartCrypto.Wallet> ethuniquewallets = new List<SmartCrypto.Wallet>();
            foreach (SmartFinance.CryptoTransactionsView ethtrans in financeviewmodel.FinanceCryptoTransactions)
            {
                if (ethtrans.NETWORK_NAME != "" &&
                    ethtrans.NETWORK_NAME != "ripple" &&
                    ethtrans.NETWORK_NAME != "solana" &&
                    ethtrans.NETWORK_NAME != "stellar" &&
                    ethtrans.NETWORK_NAME != "hedera" &&
                    ethtrans.NETWORK_NAME != "cosmos" &&
                    ethtrans.NETWORK_NAME != "cardano" &&
                    ethtrans.NETWORK_NAME != "algorand" &&
                    ethtrans.NETWORK_NAME != "dogecoin" &&
                    ethtrans.NETWORK_NAME != "dfinity" &&
                    ethtrans.NETWORK_NAME != "polkadot" &&
                    ethtrans.NETWORK_NAME != "bitcoin")
                {

                    if (ethtrans.TO_ADDRESS != "")
                    {
                        //if (ethtrans.TO_ADDRESS.Substring(0, 2) == "0x")
                        //{

                            bool foundit = false;
                            foreach (SmartCrypto.Wallet wollet in ethuniquewallets)
                            {
                                if (wollet.CRYPTO_DESTINATION == ethtrans.TO_ADDRESS)
                                {
                                    foundit = true;
                                    break;
                                }
                            }
                            if (!foundit)
                            {
                                string cryptoName = "";
                                switch (ethtrans.TO_ADDRESS)
                                {
                                    case "0x76bC6c5D2a693425C202946A72f5be5eDBEDd061":
                                        cryptoName = "Arculus Wallet";
                                        break;
                                    case "0x8D66f67743aDEc4086dB75e59Fe6BC25875AAC27":
                                        cryptoName = "Unknown Wallet";
                                        break;
                                    case "0x20bb6FBc36E28548ac5630785a90aA864cE2Bfd2":
                                        cryptoName = "Uni Swap Hot Wallet";
                                        break;
                                    case "0x4cb1e122ab5733a8a9ac586127e668fb2a231e055b6a2ce3a17bda6346ba30ee":
                                        cryptoName = "SUI wallet";
                                        break;
                                    case "0x89d8857Dc405515d49Bbdf01145ff795DD8dE475":
                                        cryptoName = "USDC Wallet";
                                        break;
                                    case "0xe61d6C8316d5fF65dc52Ba5284F84AB803a9A425":
                                        cryptoName = "Tangem Wallet";
                                        break;
                                    case "0x287b9eA7Ecc265904B537B13c0894957DC6eb111":
                                        cryptoName = "Tangem Cold Wallet";
                                        break;
                                    case "0x6230B81c21A4Cf64151ADdD1cd0976a65130Da88":
                                        cryptoName = "Phantom Hot Wallet";
                                        break;
                                    default:
                                        cryptoName = "No idea";
                                        break;
                                }
                                SmartCrypto.Wallet wallet = new SmartCrypto.Wallet()
                                {
                                    
                                    USERNAME = ethtrans.USERNAME,
                                    CUBEFACE_CODE = ethtrans.CUBEFACE_CODE,
                                    CRYPTO_DESTINATION = ethtrans.TO_ADDRESS,
                                    CRYPTO_WALLET_NAME = cryptoName,
                                    CRYPTO_CURRENCY_ORDINAL = 0,
                                    CRYPTO_CURRENCY = "",
                                    CRYPTO_AMOUNT = 0
                                };
                                ethuniquewallets.Add(wallet);
                            }
                        //}
                    }
                }
            }
            SmartCrypto.Wallet wallet1 = new SmartCrypto.Wallet()
            {

                USERNAME = "Daniel",
                CUBEFACE_CODE = 'F',
                CRYPTO_DESTINATION = "0x76bc6c5d2a693425c202946a72f5be5edbedd061",
                CRYPTO_WALLET_NAME = "FROM Coinbase1",
                CRYPTO_CURRENCY_ORDINAL = 0,
                CRYPTO_CURRENCY = "",
                CRYPTO_AMOUNT = 0
            };
            ethuniquewallets.Add(wallet1);
            SmartCrypto.Wallet wallet2 = new SmartCrypto.Wallet()
            {

                USERNAME = "Daniel",
                CUBEFACE_CODE = 'F',
                CRYPTO_DESTINATION = "0x7830c87c02e56aff27fa8ab1241711331fa86f43",
                CRYPTO_WALLET_NAME = "FROM Coinbase2",
                CRYPTO_CURRENCY_ORDINAL = 0,
                CRYPTO_CURRENCY = "",
                CRYPTO_AMOUNT = 0
            };
            ethuniquewallets.Add(wallet2);

            SmartCrypto.Wallet wallet3 = new SmartCrypto.Wallet()
            {

                USERNAME = "Daniel",
                CUBEFACE_CODE = 'F',
                CRYPTO_DESTINATION = "0x4a220e6096b25eadb88358cb44068a3248254675",
                CRYPTO_WALLET_NAME = "TO Coinbase1",
                CRYPTO_CURRENCY_ORDINAL = 0,
                CRYPTO_CURRENCY = "",
                CRYPTO_AMOUNT = 0
            };
            ethuniquewallets.Add(wallet3);

            SmartCrypto.Wallet wallet4 = new SmartCrypto.Wallet()
            {

                USERNAME = "Daniel",
                CUBEFACE_CODE = 'F',
                CRYPTO_DESTINATION = "0x514910771af9ca656af840dff83e8264ecf986ca",
                CRYPTO_WALLET_NAME = "TO Coinbase2",
                CRYPTO_CURRENCY_ORDINAL = 0,
                CRYPTO_CURRENCY = "",
                CRYPTO_AMOUNT = 0
            };
            ethuniquewallets.Add(wallet4);

            SmartCrypto.Wallet wallet6 = new SmartCrypto.Wallet()
            {

                USERNAME = "Daniel",
                CUBEFACE_CODE = 'F',
                CRYPTO_DESTINATION = "0xa9d1e08c7793af67e9d92fe308d5697fb81d3e43",
                CRYPTO_WALLET_NAME = "TO Coinbase3",
                CRYPTO_CURRENCY_ORDINAL = 0,
                CRYPTO_CURRENCY = "",
                CRYPTO_AMOUNT = 0
            };
            ethuniquewallets.Add(wallet6);
            

            return ethuniquewallets;
        }

        internal static bool RebuildETHWalletTotals(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    List<SmartCrypto.Wallet> ethuniquewallets,
                                                    CancellationToken cancellationToken,
                                                    short institutionCode,
                                                    short brandCode)
        {
            
            List<SmartCrypto.Wallet> tempo = new List<SmartCrypto.Wallet>();
            foreach (SmartCrypto.LedgerTransactionView trans in financeviewmodel.FinanceETHCryptoLedger)
            {
                short ordinal = trans.CRYPTO_CURRENCY_ORDINAL;
                List<SmartCrypto.Wallet> isit = new List<SmartCrypto.Wallet>(
                    from dd in tempo
                    where dd.CRYPTO_CURRENCY_ORDINAL == ordinal
                    select dd);
                if (isit.Count > 0)
                {
                    isit[0].CRYPTO_AMOUNT += trans.AMOUNT;
                }
                else
                {
                    SmartCrypto.Wallet newitem = new SmartCrypto.Wallet()
                    {
                        USERNAME = ourviewmodel.UserName,
                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                        CRYPTO_WALLET_NAME = trans.DESTINATION,
                        CRYPTO_DESTINATION = trans.DESTINATION,
                        CRYPTO_CURRENCY_ORDINAL = trans.CRYPTO_CURRENCY_ORDINAL,
                        CRYPTO_CURRENCY = trans.CURRENCY_DISPLAY,
                        CRYPTO_AMOUNT = trans.AMOUNT
                    };
                    tempo.Add(newitem);
                }

            }

            foreach (SmartCrypto.Wallet wally in tempo) 
            {
                // Is it in PLO??
                List<SmartFinance.CryptoWalletTotals> isittherePLO = new List<SmartFinance.CryptoWalletTotals>(
                                    from Totals in financeviewmodel.PLO.crypto_wallettotalsList
                                    where Totals.USERNAME == ourviewmodel.UserName &&
                                        Totals.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                        Totals.INSTITUTION_CODE == institutionCode &&
                                        Totals.BRAND_CODE == brandCode &&
                                        Totals.ADDRESS == wally.CRYPTO_DESTINATION
                                    select Totals);
                SmartFinance.CryptoWalletTotals newTotal = new SmartFinance.CryptoWalletTotals();
                if (isittherePLO.Count == 0)
                {
                    List<SmartFinance.CryptoWalletTotals> isittherePLO1 = new List<SmartFinance.CryptoWalletTotals>(
                                    from Totals in financeviewmodel.PLO.crypto_wallettotals_changesList
                                    where Totals.USERNAME == ourviewmodel.UserName &&
                                        Totals.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                        Totals.INSTITUTION_CODE == institutionCode &&
                                        Totals.BRAND_CODE == brandCode &&
                                        Totals.ADDRESS == wally.CRYPTO_DESTINATION
                                    select Totals);
                    if (isittherePLO1.Count == 0)
                    {
                        newTotal.USERNAME = ourviewmodel.UserName;
                        newTotal.CUBEFACE_CODE = SmartParametersV2016.Finance;
                        newTotal.INSTITUTION_CODE = institutionCode;
                        newTotal.BRAND_CODE = brandCode;
                        newTotal.ADDRESS = wally.CRYPTO_DESTINATION;
                        newTotal.CRYPTO_WALLET_NAME = wally.CRYPTO_WALLET_NAME;
                        newTotal.CRYPTO_CURRENCY_ORDINAL = wally.CRYPTO_CURRENCY_ORDINAL;
                        newTotal.CRYPTO_CURRENCY = wally.CRYPTO_CURRENCY;
                        newTotal.CRYPTO_AMOUNT = Convert.ToDouble(wally.CRYPTO_AMOUNT);

                        newTotal.RATE = CryptoBollocksV2025.FindRate(financeviewmodel, newTotal.CRYPTO_CURRENCY_ORDINAL);
                        newTotal.VALUE = (newTotal.RATE * newTotal.CRYPTO_AMOUNT).ToString("F3");
                        newTotal.Updated = false;
                    }
                    else
                    {
                        newTotal = isittherePLO1.First();
                        newTotal.CRYPTO_AMOUNT = Convert.ToDouble(wally.CRYPTO_AMOUNT);
                        newTotal.Updated = true;
                    }
                }
                financeviewmodel.PLO.crypto_wallettotals_changesList.Add(newTotal);
                // Lets hope PostProcessing catches all of these??

            }
#if CRYPTOS
            if (financeviewmodel.PLO.crypto_wallettotals_changesList.Count > 0)
            {
                List<SmartFinance.CryptoWalletTotals> tempTotals = new List<SmartFinance.CryptoWalletTotals>
                (from Totals in financeviewmodel.PLO.crypto_wallettotals_changesList
                 select Totals).ToList();

                financeviewmodel.FinanceWalletTotals.AddRange(tempTotals);
            }
#endif
            return true;
        }
        internal static async Task<bool> RebuildXRPWalletTotals(
            MainViewModel ourviewmodel,
            FinanceViewModel financeviewmodel,
            List<SmartCrypto.Wallet> xrpuniquewallets,
            CancellationToken cancellationToken,
            short institutionCode,
            short brandCode)
        {
            string url = $"https://api.xrpscan.com";

            foreach (SmartCrypto.Wallet wally in xrpuniquewallets)
            {
                string walletAddress = wally.CRYPTO_DESTINATION;
                string wa = $"api/v1/account/{walletAddress}";
                double balanceXRP = 0;

                try
                {
                    SmartRoutinesV2018.CreateUri(ourviewmodel, url, wa);

                    string response = await SmartBobV2017.HTTPCLIENT_GET_ASYNC_JSONX(
                        ourviewmodel,
                        ourviewmodel.TargetUrl,
                        cancellationToken,
                        "",
                        new Guid());

                    if (string.IsNullOrEmpty(ourviewmodel.errorMessage) &&
                        !string.IsNullOrEmpty(response))
                    {
                        using JsonDocument jsonDoc = JsonDocument.Parse(response);
                        JsonElement root = jsonDoc.RootElement;

                        if (root.TryGetProperty("Balance", out JsonElement balanceElement))
                        {
                            string balanceDrops = balanceElement.GetString();

                            if (!string.IsNullOrEmpty(balanceDrops) &&
                                double.TryParse(balanceDrops, out double drops))
                            {
                                balanceXRP = drops / 1_000_000;
                                wally.CRYPTO_AMOUNT = balanceXRP;
                            }
                            else
                            {
                                financeviewmodel.errorMessage = "Invalid balance format";
                                return false;
                            }
                        }
                        else
                        {
                            financeviewmodel.errorMessage = "Could not find balance";
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = ex.Message;
                    return false;
                }

                // --- Existing logic unchanged below ---

                List<SmartFinance.CryptoWalletTotals> isittherePLO =
                    (from Totals in financeviewmodel.PLO.crypto_wallettotalsList
                     where Totals.USERNAME == ourviewmodel.UserName &&
                           Totals.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                           Totals.INSTITUTION_CODE == institutionCode &&
                           Totals.BRAND_CODE == brandCode &&
                           Totals.ADDRESS == wally.CRYPTO_DESTINATION
                     select Totals).ToList();

                SmartFinance.CryptoWalletTotals newTotal = new SmartFinance.CryptoWalletTotals();

                if (isittherePLO.Count == 0)
                {
                    List<SmartFinance.CryptoWalletTotals> isittherePLO1 =
                        (from Totals in financeviewmodel.PLO.crypto_wallettotals_changesList
                         where Totals.USERNAME == ourviewmodel.UserName &&
                               Totals.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                               Totals.INSTITUTION_CODE == institutionCode &&
                               Totals.BRAND_CODE == brandCode &&
                               Totals.ADDRESS == wally.CRYPTO_DESTINATION
                         select Totals).ToList();

                    if (isittherePLO1.Count == 0)
                    {
                        newTotal.USERNAME = ourviewmodel.UserName;
                        newTotal.CUBEFACE_CODE = SmartParametersV2016.Finance;
                        newTotal.INSTITUTION_CODE = institutionCode;
                        newTotal.BRAND_CODE = brandCode;
                        newTotal.ADDRESS = wally.CRYPTO_DESTINATION;
                        newTotal.CRYPTO_WALLET_NAME = wally.CRYPTO_WALLET_NAME;
                        newTotal.CRYPTO_CURRENCY_ORDINAL = wally.CRYPTO_CURRENCY_ORDINAL;
                        newTotal.CRYPTO_CURRENCY = wally.CRYPTO_CURRENCY;
                        newTotal.CRYPTO_AMOUNT = Convert.ToDouble(wally.CRYPTO_AMOUNT);

                        newTotal.RATE = CryptoBollocksV2025.FindRate(
                            financeviewmodel,
                            newTotal.CRYPTO_CURRENCY_ORDINAL);

                        newTotal.VALUE = (newTotal.RATE * newTotal.CRYPTO_AMOUNT).ToString("F3");
                        newTotal.Updated = false;
                    }
                    else
                    {
                        newTotal = isittherePLO1.First();
                        newTotal.CRYPTO_AMOUNT = Convert.ToDouble(wally.CRYPTO_AMOUNT);
                        newTotal.Updated = true;
                    }

                    financeviewmodel.PLO.crypto_wallettotals_changesList.Add(newTotal);
                }
            }

#if CRYPTOS
            if (financeviewmodel.PLO.crypto_wallettotals_changesList.Count > 0)
            {
                List<SmartFinance.CryptoWalletTotals> tempTotals =
                    financeviewmodel.PLO.crypto_wallettotals_changesList.ToList();

                financeviewmodel.FinanceWalletTotals.AddRange(tempTotals);
            }
#endif

            return true;
        }

        internal static async Task LookupTransaction(
                                FinanceViewModel financeviewmodel,
                                string txHash,
                                string apiKey)
        {
            //
            // Change required 29-Sep-2025 (ish)
            // For more info https://docs.etherscan.io/etherscan-v2/v2-quickstart}
            //

            string url = $"https://api.etherscan.io/v2/api?chainid=1&module=proxy&action=eth_getTransactionByHash&txhash={txHash}&apikey={apiKey}";

            using HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(url);

            using JsonDocument jsonDoc = JsonDocument.Parse(response);
            JsonElement root = jsonDoc.RootElement;

            // Check if "result" exists and is not null
            if (!root.TryGetProperty("result", out JsonElement tx) ||
                tx.ValueKind == JsonValueKind.Null)
            {
                Console.WriteLine("Transaction not found.");
                return;
            }

            try
            {
                Console.WriteLine("Transaction Details:");

                string hash = tx.TryGetProperty("hash", out var h) ? h.GetString() : "";
                string from = tx.TryGetProperty("from", out var f) ? f.GetString() : "";
                string to = tx.TryGetProperty("to", out var t) ? t.GetString() : "";

                financeviewmodel.from = from ?? "";
                financeviewmodel.to = to ?? "";

                string value = tx.TryGetProperty("value", out var v) ? v.GetString() : "";
                string gas = tx.TryGetProperty("gas", out var g) ? g.GetString() : "";
                string gasPrice = tx.TryGetProperty("gasPrice", out var gp) ? gp.GetString() : "";
                string nonce = tx.TryGetProperty("nonce", out var n) ? n.GetString() : "";
                string blockNumber = tx.TryGetProperty("blockNumber", out var bn) ? bn.GetString() : "";

                Console.WriteLine($"Hash:        {hash}");
                Console.WriteLine($"Value (hex): {value}");
                Console.WriteLine($"Gas:         {gas}");
                Console.WriteLine($"Gas Price:   {gasPrice}");
                Console.WriteLine($"Nonce:       {nonce}");
                Console.WriteLine($"Block Number:{blockNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static async Task<bool> RebuildETHLedger(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    List<SmartCrypto.V2Transactions> v2transactionsList,
                                                    short institutionCode,
                                                    short brandCode,
                                                    string currency,
                                                    string xrpLedgerApiUrl,
                                                    List<SmartCrypto.Wallet> ethuniquewallets)
        {
            // What we need to do now, is to reduce financeviewmodel.v2transactionsList
            // down to only those with Currency = ETH and with a distinct TO_ADDRESS
            int miniscule = 0;
            double minisculeAmount = 0;

            List<SmartCrypto.LedgerTransactionView> tempLedger = new List<SmartCrypto.LedgerTransactionView>();

            string Ledger = currency;
            string SOURCE = "";
            string DESTINATION = "";
            string WALLET = "";
            List<ETHLedgerTransaction> ethtrans = new List<ETHLedgerTransaction>();

#if CRYPTOS
            ourviewmodel.wherewereat = "ETH Ledger Scan";
#endif

            foreach (SmartCrypto.Wallet wollet in ethuniquewallets)

            {

                if (!await ETHLedgerScan(ourviewmodel,
                                        financeviewmodel,
                                        xrpLedgerApiUrl,
                                        wollet.CRYPTO_DESTINATION,//trans.TO.ADDRESS,
                                        ethtrans,
                                        currency))
                {
                    return false;
                }
            }

            ourviewmodel.wherewereat = "ETH Ledger";

            foreach (SmartCrypto.V2Transactions item in v2transactionsList)
            {
                if (item.NETWORK.NETWORK_NAME == "ethereum")
                {
                    if (item.NETWORK.HASH != "")//.Contains("7191db0ea9c111e7c3e660b5b3eb881d24486d952b408bb67ff722fe4c070189"))
                    {
                        Console.WriteLine("Here");

                        string txHash = "0x" + item.NETWORK.HASH;
                        string apiKey = @"T23AITUIC41I6PKFWIMAK8QN8GSRQ4VSZN";

                        await LookupTransaction(financeviewmodel, txHash, apiKey);

                        string FROM = financeviewmodel.from.ToString();
                        List<SmartCrypto.Wallet> source = new List<SmartCrypto.Wallet>(
                        from Wally in ethuniquewallets
                        where Wally.CRYPTO_DESTINATION == FROM
                        select Wally);
                        if (source.Count > 0)
                        {
                            SOURCE = source.First().CRYPTO_WALLET_NAME;// + "(" + currency + ")";
                        
                        }
                        else
                        {
                            SOURCE = "Unknown";
                        }

                        List<SmartCrypto.Wallet> destination = new List<SmartCrypto.Wallet>(
                        from Wally in ethuniquewallets
                        where Wally.CRYPTO_DESTINATION == item.TO.ADDRESS
                        select Wally);
                        if (destination.Count > 0)
                        {
                            DESTINATION = destination.First().CRYPTO_WALLET_NAME;// + "(" + currency + ")";
                            WALLET = destination.First().CRYPTO_WALLET_NAME;
                        }
                        else
                        {
                            DESTINATION = "Coinbase";
                            WALLET = "?";
                        }
                        DateTime transactionDate = item.CREATED_AT;

                        // Eliminate amounts < 1
                        if (Convert.ToDouble(item.AMOUNT.AMOUNT) >= 1.0 ||
                            Convert.ToDouble(item.AMOUNT.AMOUNT) <= -1.0 )
                        {
                            // Check to see if we already have it as we sometimes
                            // come at ETH Ledger from two different angles
                            List<SmartFinance.CryptoLedgers> isittherePLO = new List<SmartFinance.CryptoLedgers>(
                               from Trans in financeviewmodel.PLO.crypto_ledgersList
                               where Trans.USERNAME == ourviewmodel.UserName &&
                               Trans.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                               Trans.INSTITUTION_CODE == institutionCode &&
                               Trans.BRAND_CODE == brandCode &&
                               Trans.LEDGER_NAME == Ledger &&
                               Trans.WALLET == WALLET &&
                               Trans.ACCOUNT == item.UUID &&
                               Trans.TRANSACTION_DATE == transactionDate &&
                               Trans.AMOUNT == Convert.ToDouble(item.AMOUNT.AMOUNT) &&
                               Trans.SOURCE == SOURCE &&
                               Trans.DESTINATION == item.TO.ADDRESS
                               select Trans);
                            if (isittherePLO.Count == 0)
                            {
                                // Is it in Changes??
                                List<SmartFinance.CryptoLedgers> isittherePLO1 = new List<SmartFinance.CryptoLedgers>(
                                   from Trans in financeviewmodel.PLO.crypto_ledgers_changesList
                                   where Trans.USERNAME == ourviewmodel.UserName &&
                                   Trans.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                   Trans.INSTITUTION_CODE == institutionCode &&
                                   Trans.BRAND_CODE == brandCode &&
                                   Trans.LEDGER_NAME == Ledger &&
                                   Trans.WALLET == WALLET &&
                                   Trans.ACCOUNT == item.UUID &&
                                   Trans.TRANSACTION_DATE == transactionDate &&
                                   Trans.AMOUNT == Convert.ToDouble(item.AMOUNT.AMOUNT) &&
                                   Trans.SOURCE == SOURCE &&
                                   Trans.DESTINATION == item.TO.ADDRESS
                                   select Trans);
                                if (isittherePLO1.Count == 0)
                                {
                                    SmartFinance.CryptoLedgers ledger = new SmartFinance.CryptoLedgers()
                                    {
                                        USERNAME = ourviewmodel.UserName,
                                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                                        INSTITUTION_CODE = institutionCode,
                                        BRAND_CODE = brandCode,
                                        LEDGER_NAME = Ledger,
                                        WALLET = WALLET,
                                        ACCOUNT = item.UUID,
                                        TRANSACTION_DATE = transactionDate,
                                        AMOUNT = Convert.ToDouble(item.AMOUNT.AMOUNT),
                                        SOURCE = SOURCE,
                                        DESTINATION = DESTINATION,
                                        //CURRENCY_DISPLAY = item.Currency,
                                        //FEE = item.Fee,
                                        TRANSACTION_TYPE = item.TYPE,
                                        ////FROM_ADDRESS = DESTINATION,
                                        TO_ADDRESS = item.TO.ADDRESS,
                                        NETWORK_NAME = item.NETWORK.NETWORK_NAME,
                                        HASH = item.NETWORK.HASH,
                                        Updated = false
                                    };
                                    financeviewmodel.PLO.crypto_ledgers_changesList.Add(ledger);
                                }
                            }
                        }
                        else
                        {
                            miniscule++;
                            minisculeAmount += Convert.ToDouble(item.AMOUNT.AMOUNT);
                        }


                       // The ToLower because Etherium decrees them thus
                        //List<SmartCrypto.LedgerTransactionView> isitthere2 = new List<SmartCrypto.LedgerTransactionView>(
                        //from Trans in tempLedger
                        //where Trans.HASH.ToLower() == item.NETWORK.HASH
                        //select Trans);
                        //if (isitthere2.Count == 0)
                        //{
                        SmartCrypto.LedgerTransactionView view = new SmartCrypto.LedgerTransactionView()
                        {
                            LEDGER_NAME = Ledger,
                            ACCOUNT = item.UUID,
                            TRANSACTION_DATE = transactionDate,
                            CRYPTO_CURRENCY_ORDINAL = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, item.AMOUNT.CURRENCY),
                            CURRENCY_DISPLAY = item.AMOUNT.CURRENCY,
                            AMOUNT = Convert.ToDouble(item.AMOUNT.AMOUNT),
                            TRANSACTION_TYPE = item.TYPE,
                            SOURCE = SOURCE,
                            DESTINATION = DESTINATION,
                            ////FROM_ADDRESS = DESTINATION,
                            TO_ADDRESS = item.TO.ADDRESS,
                            HASH = item.NETWORK.HASH,
                            FEE = item.BUYSELL.FEE.AMOUNT,
                            NETWORK_NAME = item.NETWORK.NETWORK_NAME,
                            WALLET = WALLET  // I know this isn't right

                        };
                        // Eliminate amounts < 1
                        if (Convert.ToDouble(item.AMOUNT.AMOUNT) >= 1.0 ||
                            Convert.ToDouble(item.AMOUNT.AMOUNT) <= -1.0)
                        {
                            tempLedger.Add(view);
                        }
                        else
                        {
                            miniscule++;
                            minisculeAmount += Convert.ToDouble(item.AMOUNT.AMOUNT);
                        }
                    }
                }
            }

            if (tempLedger.Count > 0)
            {
                tempLedger = new List<SmartCrypto.LedgerTransactionView>
                (from Transactions in tempLedger
                 orderby Transactions.TRANSACTION_DATE ascending
                 select Transactions).ToList();
            }
            financeviewmodel.FinanceETHCryptoLedger = tempLedger;

            return true;
        }

        internal static bool HashesMatch(string etherscanHash, string coinbaseHash)
        {
            return string.Equals(
                etherscanHash?.Trim(),
                coinbaseHash?.Trim(),
                StringComparison.OrdinalIgnoreCase
            );
        }

        internal static async Task<bool> RebuildXRPLedger(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            List<SmartCrypto.V2Transactions> v2transactionsList,
                                                            short institutionCode,
                                                            short brandCode,
                                                            string currency,
                                                            //string networkName,
                                                            string xrpLedgerApiUrl,
                                                            List<SmartCrypto.Wallet> uniquewallets)
        {
            // What we need to do now, is to reduce financeviewmodel.v2transactionsList
            // down to only those with Currency = XRP and with a distinct TO_ADDRESS



            int miniscule = 0;
            double minisculeAmount = 0;

            List<SmartCrypto.LedgerTransactionView> tempLedger = new List<SmartCrypto.LedgerTransactionView>();

            string Ledger = currency;
            string SOURCE = "";
            string DESTINATION = "";
            string WALLET = "";

            foreach (SmartCrypto.Wallet wollet in uniquewallets)

            {
                List<XRPLedgerTransaction> xrptrans = new List<XRPLedgerTransaction>();

                if (!await XRPLedgerScan(ourviewmodel,
                                        financeviewmodel,
                                        xrpLedgerApiUrl,
                                        wollet.CRYPTO_DESTINATION,//trans.TO.ADDRESS,
                                        xrptrans,
                                        currency))
                {
                    return false;
                }
                else
                {
                    if (xrptrans.Count > 0)
                    {
                        foreach (XRPLedgerTransaction item in xrptrans)
                        {
                            DateTime transactionDate = SmartDanV2025.ConvertLedgerDateToDateTime(item.Date, 2000);

                            List<SmartCrypto.Wallet> source = new List<SmartCrypto.Wallet>(
                                from Wally in uniquewallets
                                where Wally.CRYPTO_DESTINATION == item.Account
                                select Wally);
                            if (source.Count > 0)
                            {
                                SOURCE = source.First().CRYPTO_WALLET_NAME + "(" + currency + ")";
                            }
                            else
                            {
                                List<SmartCrypto.V2Transactions> sourceX = new List<SmartCrypto.V2Transactions>(
                                from Trans in v2transactionsList
                                where Trans.NETWORK.HASH == item.Hash
                                select Trans);
                                if (sourceX.Count > 0)
                                {
                                    SOURCE = sourceX.First().EXCHANGE;// "Coinbase";
                                }
                                else
                                {
                                    SOURCE = "Unknown";
                                }
                            }

                            List<SmartCrypto.Wallet> destination = new List<SmartCrypto.Wallet>(
                                from Wally in uniquewallets
                                where Wally.CRYPTO_DESTINATION == item.Destination
                                select Wally);
                            if (destination.Count > 0)
                            {
                                DESTINATION = destination.First().CRYPTO_WALLET_NAME + "(" + currency + ")";
                                WALLET = destination.First().CRYPTO_WALLET_NAME;
                            }
                            else
                            {
                                DESTINATION = "Coinbase";
                                WALLET = "?";
                            }

                            // Eliminate amounts < 1
                            if (item.Amount >= 1.0m ||
                                item.Amount <= -1.0m)
                            {
                                // Check to see if we already have it as we sometimes
                                // come at XRP Ledger from two different angles
                                List<SmartFinance.CryptoLedgers> isittherePLO = new List<SmartFinance.CryptoLedgers>(
                                   from Trans in financeviewmodel.PLO.crypto_ledgersList
                                   where Trans.USERNAME == ourviewmodel.UserName &&
                                   Trans.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                   Trans.INSTITUTION_CODE == institutionCode &&
                                   Trans.BRAND_CODE == brandCode &&
                                   Trans.LEDGER_NAME == Ledger &&
                                   Trans.WALLET == WALLET &&
                                   Trans.ACCOUNT == item.Account &&
                                   Trans.TRANSACTION_DATE == transactionDate &&
                                   Trans.AMOUNT == Convert.ToDouble(item.Amount) &&
                                   Trans.SOURCE == SOURCE &&
                                   Trans.DESTINATION == DESTINATION
                                   select Trans);
                                if (isittherePLO.Count == 0)
                                {
                                    // Is it in Changes??
                                    List<SmartFinance.CryptoLedgers> isittherePLO1 = new List<SmartFinance.CryptoLedgers>(
                                       from Trans in financeviewmodel.PLO.crypto_ledgers_changesList
                                       where Trans.USERNAME == ourviewmodel.UserName &&
                                       Trans.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                       Trans.INSTITUTION_CODE == institutionCode &&
                                       Trans.BRAND_CODE == brandCode &&
                                       Trans.LEDGER_NAME == Ledger &&
                                       Trans.WALLET == WALLET &&
                                       Trans.ACCOUNT == item.Account &&
                                       Trans.TRANSACTION_DATE == transactionDate &&
                                       Trans.AMOUNT == Convert.ToDouble(item.Amount) &&
                                       Trans.SOURCE == SOURCE &&
                                       Trans.DESTINATION == DESTINATION
                                       select Trans);
                                    if (isittherePLO1.Count == 0)
                                    {
                                        SmartFinance.CryptoLedgers ledger = new SmartFinance.CryptoLedgers()
                                        {
                                            USERNAME = ourviewmodel.UserName,
                                            CUBEFACE_CODE = SmartParametersV2016.Finance,
                                            INSTITUTION_CODE = institutionCode,
                                            BRAND_CODE = brandCode,
                                            LEDGER_NAME = Ledger,
                                            WALLET = WALLET,
                                            ACCOUNT = item.Account,
                                            TRANSACTION_DATE = transactionDate,
                                            AMOUNT = Convert.ToDouble(item.Amount),
                                            SOURCE = SOURCE,
                                            DESTINATION = DESTINATION,
                                            CURRENCY_DISPLAY = item.Currency,
                                            FEE = item.Fee,
                                            TRANSACTION_TYPE = item.TransactionType,
                                            //FROM_ADDRESS = DESTINATION,
                                            TO_ADDRESS = item.Destination,
                                            NETWORK_NAME = item.NetworkName,
                                            HASH = item.Hash,
                                            Updated = false
                                        };
                                        financeviewmodel.PLO.crypto_ledgers_changesList.Add(ledger);
                                    }
                                }
                            }
                            else
                            {
                                miniscule++;
                                minisculeAmount += Convert.ToDouble(item.Amount);
                            }

#if CRYPTOS
                            List<SmartCrypto.LedgerTransactionView> isitthere2 = new List<SmartCrypto.LedgerTransactionView>(
                            from Trans in tempLedger
                            where Trans.HASH == item.Hash
                            select Trans);
                            if (isitthere2.Count == 0)
                            {
                                SmartCrypto.LedgerTransactionView view = new SmartCrypto.LedgerTransactionView()
                                {
                                    LEDGER_NAME = Ledger,
                                    ACCOUNT = item.Account,
                                    TRANSACTION_DATE = transactionDate,
                                    CRYPTO_CURRENCY_ORDINAL = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, item.Currency),
                                    CURRENCY_DISPLAY = item.Currency,
                                    AMOUNT = Convert.ToDouble(item.Amount),
                                    TRANSACTION_TYPE = item.TransactionType,
                                    SOURCE = SOURCE,
                                    DESTINATION = DESTINATION,
                                    //FROM_ADDRESS = DESTINATION,
                                    TO_ADDRESS = item.Destination,
                                    HASH = item.Hash
                                };
                                // Eliminate amounts < 1
                                if (item.Amount >= 1.0m ||
                                    item.Amount <= -1.0m)
                                {
                                    tempLedger.Add(view);
                                }
                                else
                                {
                                    miniscule++;
                                    minisculeAmount += Convert.ToDouble(item.Amount);
                                }
                            }
#endif
                        }
                    }
                }
            }

#if CRYPTOS
            if (tempLedger.Count > 0)
            {
                tempLedger = new List<SmartCrypto.LedgerTransactionView>
                (from Transactions in tempLedger
                 orderby Transactions.TRANSACTION_DATE ascending
                 select Transactions).ToList();
            }
            financeviewmodel.FinanceXRPCryptoLedger = tempLedger;
#endif
            return true;
        }

        internal static async Task<bool> RebuildPLOTransactions(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    char categoryCode,
                                                    short institutionCode,
                                                    short brandCode,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                   string header = "")
        {
            // The way this works, is that for each TC there is a T
            // The way this works is that each T may have one or more TCs
            // So, work through all NEW Transactions and check that they
            // do not exist ALREADY in the PLO TC list. If they do, then they
            // won't need a T because that will have already been added if
            // it needed to be.
            // So, if the TC IS new, but BEFORE you add it into the changes
            // check to see if it's T already exists in the PLO T list.
            // If the T doesn't exist then check to see if it already exists
            // in the T changes and add it in if it doesn'. Finally, add the
            // TC into the TC changes WITHOUT checking (see below).
            // 
            // Why don't we need to look in the TC changes before we add it? 
            // All transactions are unique aren't they and as we
            // work through the list the TC changes should fill up with new ones,
            // but no TransactionCategory should ever be checked against a previous
            // one as that would be like having:
            // Daniel F 2000 20001 Coinbase f06123 GBP 2025-04-1 buy 1,000 XRP
            // Daniel F 2000 20001 Coinbase f06123 GBP 2025-04-1 buy 1,000 XRP
            // and failing the 2nd one because the first is aready there!! when
            // Dan DID indeed purchase two lots of XRP on 1st April 2025

            if (financeviewmodel.FinanceCryptoTransactions.Count > 0)//v2transactionsList.Count > 0)
            {
                // Fix this then increment it to ensure uniqueness
                financeviewmodel.sequence_no = SmartDanV2025.UnixSequenceNo();

                // Find all the TransactionsCategories we haven't got into tc_changes
                // Then work through tc_changes building a disctinct CategoryTypes
                // Make sure you have a distinct Accounts and
                // Also a distinct Addresses ...

                // Form the TransactionsCategories tc_changes List
                foreach (SmartFinance.CryptoTransactionsView tran in financeviewmodel.FinanceCryptoTransactions)
                {
                    if (tran.Processed)
                    {
                        continue;
                    }
                    tran.Processed = true;
                    string description = "";
                    string cryptoType = "";
                    int isPaidIn = 0;
                    switch (tran.TYPE)
                    {
                        case "fiat_deposit":
                            isPaidIn = 1;
                            tran.TYPE = "fiat";
                            break;
                        case "buy":
                            isPaidIn = 0;
                            break;
                        case "sell":
                            isPaidIn = 1;
                            break;
                        case "trade":
                            isPaidIn = 1;
                            break;
                        case "send":
                            // It costs us to send
                            isPaidIn = 0;
                            break;
                        case "interest":
                            isPaidIn = 1;
                            break;
                        default:
                            break;
                    }
                    if (tran.TYPE.Length > 0)
                    {
                        description = Char.ToUpper(tran.TYPE[0]) + tran.TYPE.Substring(1);
                        cryptoType = description;
                    }
                    // Work out the Header StatementDate and StatementNo
                    DateTime startDate = tran.ACCOUNT_CREATED;      // Example start date
                    DateTime endDate = tran.CREATED_AT;             // Example end date
                    int diff = SmartDanV2025.GetMonthsDifference(startDate, endDate);
                    short statementNo = Convert.ToInt16(diff + 1);
                    if (statementNo == 0)
                    {
#if WINFORMS || WPF  || WINUI || ANDROID
                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Statement No", "Is 0 for: " + tran.CREATED_AT.ToString());
#endif
#if WINFORMS
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Crypto Statement No. is 0 for: " + tran.CREATED_AT.ToString());
#endif
                    }
                    DateTime statementDate = SmartDanV2025.GetFirstOfNextMonth(endDate);
                    // DONT DO THE HEADER HERE!! We now find DISTINCT HEADERS from tc_changes!

                    short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionCode(financeviewmodel,
                                                                                    transaction_groupsFound,
                                                                                    transaction_typesFound,
                                                                                    isPaidIn,
                                                                                    cryptoType,
                                                                                    false,
                                                                                    institutionCode,
                                                                                    brandCode,
                                                                                    categoryCode);
                    if (transCodes.Length != 2)
                    {
#if WINFORMS || WPF  || WINUI || ANDROID
                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Trans Lookup", "Trans type  failed: " + tran.TYPE);
#endif
#if WINFORMS
#endif
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                            meterActivity,
#endif
                            ourviewmodel, "Crypto Trans Lookup: Trans type  failed: " + tran.TYPE);
                        //#if ANDROIDX
                        //                      await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                        //#endif
                        // Carry on
                    }
                    else
                    {
                        if (transCodes[0] == 0 && transCodes[1] == 0)
                        {
#if WPF  || WINUI || ANDROID
                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Crypto Trans Lookup", "Trans type  failed: " + tran.TYPE);
#endif
#if WINFORMS
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Crypto Trans Lookup: Trans type  failed: " + tran.TYPE);
#endif
                            //#if ANDROIDX
                            //                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                            //#endif

                            // Carry on - we have others to do
                        }
                    }
                    // THIS should be GBP!
                    short nativeOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, tran.NATIVE_CURRENCY);
                    // This could be any fuckin BitCoin
                    short cryptoOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, tran.CRYPTO_CURRENCY);

                    description += " " + tran.NATIVE_CURRENCY;

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
                        tran.UDPRN,
                        statementDate,
                        statementNo,
                        tran.CREATED_AT,
                        isPaidIn,                                   // True or false for Crypto
                        transCodes,
                        description,                                // Description
                        cryptoType,                                 // Crypto Type
                        Convert.ToDouble(tran.CRYPTO_AMOUNT),       // Value Amount
                        cryptoOrdinal,                              // Value Ordinal
                        Convert.ToDouble(tran.NATIVE_AMOUNT),       // Value Amount
                        nativeOrdinal,                              // Value Ordinal
                        0,                                          // Crypto Balance 
                        0,                                          // Crypto Balance Ordinal
                        0);

                    // Only add it in if we can't find it
                    if (transactionscategories_found.Count == 0)
                    {
                        // Now! We are about to add it in, but does it have a header?
                        // Lets go and see in the Transactions list ...
                        SmartFinance.Transactions transHeader = new SmartFinance.Transactions()
                        {
                            USERNAME = tran.USERNAME,
                            CUBEFACE_CODE = SmartParametersV2016.Finance,
                            INSTITUTION_CODE = institutionCode,
                            BRAND_CODE = brandCode,
                            SORTCODE = tran.EXCHANGE,
                            ACCOUNT_NO = tran.UUID,
                            UDPRN = tran.UDPRN,
                            STATEMENT_DATE = statementDate,
                            STATEMENT_NO = statementNo
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
                                Header.UDPRN == transHeader.UDPRN &&
                                Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                                Header.STATEMENT_NO == transHeader.STATEMENT_NO
                         select Header).ToList();
                        if (transheaders_found.Count == 0)
                        {
                            // Not there? Is it already in our changes?
                            // Don't want to add it in twice!!
                            transheaders_found =
                               new List<SmartFinance.Transactions>
                                (from Header in financeviewmodel.PLO.transactions_changesList
                                 where Header.USERNAME == transHeader.USERNAME &&
                                        Header.CUBEFACE_CODE == transHeader.CUBEFACE_CODE &&
                                        Header.INSTITUTION_CODE == transHeader.INSTITUTION_CODE &&
                                        Header.BRAND_CODE == transHeader.BRAND_CODE &&
                                        Header.SORTCODE == transHeader.SORTCODE &&
                                        Header.ACCOUNT_NO == transHeader.ACCOUNT_NO &&
                                        Header.UDPRN == transHeader.UDPRN &&
                                        Header.STATEMENT_DATE == transHeader.STATEMENT_DATE &&
                                        Header.STATEMENT_NO == transHeader.STATEMENT_NO

                                 select Header).ToList();
                            if (transheaders_found.Count == 0)
                            {
                                // Add it in
                                transHeader.RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                                transHeader.Updated = false; // Its new
                                financeviewmodel.PLO.transactions_changesList.Add(transHeader);
                            }
                        }


                        // Now finish off the TransactionsCategories
                        // Why bother looking in the changes? 
                        // All transactions are unique aren't they and as we
                        // work through the list the changes MIGHT fill up,
                        // but no TransactionCategory is EVER going to be like
                        // the previous one

                        // Still want to add it in even if we can't analyze the transaction ...
                        int random1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                        int random2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);

                        financeviewmodel.sequence_no++;
                        // Use common template
                        SmartFinance.TransactionsCategories transactionCategory =
                            SmartDanV2025.TransactionCategory_Template(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    brandCode,
                                                                    tran.EXCHANGE,
                                                                    tran.UUID,
                                                                    tran.UDPRN,
                                                                    statementDate,
                                                                    statementNo,
                                                                    financeviewmodel.sequence_no,
                                                                    random1,
                                                                    tran.CREATED_AT,
                                                                    isPaidIn,
                                                                    transCodes,
                                                                    description,                                    // Description
                                                                    cryptoType,                                      // The Crypto buy/sell/etc
                                                                    Convert.ToDouble(tran.CRYPTO_AMOUNT),           // Amount
                                                                    cryptoOrdinal,                                  // Ordinal
                                                                    Convert.ToDouble(tran.NATIVE_AMOUNT),           // The value of payment
                                                                    nativeOrdinal,                                  // Should be 1 for GBP
                                                                    0,                                              // So we can work out the FEE
                                                                    0,                                              // Zero 
                                                                    0,
                                                                    random2,
                                                                    false);             // Its a new
                        financeviewmodel.PLO.transactionscategories_changesList.Add(transactionCategory);
                    }
                }
            }
            return true;
        }

        internal static void RebuildCryptoAccounts(FinanceViewModel financeviewmodel)
        {
            financeviewmodel.v2accountsList =
                new List<SmartCrypto.V2Accounts>
                (from Accounts in financeviewmodel.v2accountsList
                 orderby Accounts.CREATED_AT
                 select Accounts);
            if (financeviewmodel.v2accountsList.Count > 0)
            {
                List<SmartFinance.CryptoAccountsView> tempAccounts = new List<SmartFinance.CryptoAccountsView>();
                foreach (SmartCrypto.V2Accounts acct in financeviewmodel.v2accountsList)
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
                        SmartFinance.CryptoAccountsView acctsView = new SmartFinance.CryptoAccountsView()
                        {
                            USERNAME = acct.USERNAME,
                            EXCHANGE = acct.EXCHANGE,
                            UUID = acct.ID,
                            //ACTIVE = acct.ACTIVE,
                            UDPRN = acct.UDPRN,
                            //AVAILABLE_BALANCE_CURRENCY = acct.AVAILABLE_BALANCE.CURRENCY,
                            //AVAILABLE_BALANCE_VALUE = acct.AVAILABLE_BALANCE.VALUE,
                            CREATED_AT = acct.CREATED_AT,
                            CURRENCY = acct.CURRENCY.CODE,
                            //DEFAULT = acct.DEFAULT,
                            //DELETED_AT = acct.DELETED_AT,
                            //HOLD_CURRENCY = acct.HOLD.CURRENCY,
                            //HOLD_VALUE = acct.HOLD.VALUE,
                            NAME = acct.NAME,
                            //PLATFORM = acct.PLATFORM,
                            //READY = acct.READY,
                            //RETAIL_PORTFOLIO_ID = acct.RETAIL_PORTFOLIO_ID,
                            TYPE = acct.TYPE,
                            UPDATED_AT = acct.UPDATED_AT
                        };
                        tempAccounts.Add(acctsView);
                    }
                }
                financeviewmodel.FinanceCryptoAccounts = tempAccounts;
            }
            return;
        }

        internal static void RebuildCryptoAddresses(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    short institutionCode,
                                                    short brandCode)
        {
            // Here, you may have found Addresses from V2 AND V3 so you need
            // to reduce them to Uniqueness, please.
            // Don't know why this should happen (!!!) but its because
            // Coinbase has both V2 and V3 stuff and we need to suck it ALL in,
            // in case we lose/miss something, then get rid of duplicate stuff
            financeviewmodel.v2addressesList =
                new List<SmartCrypto.V2Addresses>
                (financeviewmodel.v2addressesList.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.EXCHANGE,
                    key.UUID,
                    key.CREATED_AT,
                    key.ID,
                    key.DESTINATION_TAG,
                    key.UDPRN,       // !!!
                    key.NETWORK,
                    key.RESOURCE
                }));


            financeviewmodel.v2addressesList =
                new List<SmartCrypto.V2Addresses>
                (from Addresses in financeviewmodel.v2addressesList
                 orderby Addresses.CREATED_AT
                 select Addresses);
            if (financeviewmodel.v2addressesList.Count > 0)
            {
                List<SmartFinance.CryptoAddressesView> tempAddresses = new List<SmartFinance.CryptoAddressesView>();

                foreach (SmartCrypto.V2Addresses addr in financeviewmodel.v2addressesList)
                {
                    SmartFinance.CryptoAddressesView addressView = new SmartFinance.CryptoAddressesView()
                    {
                        USERNAME = addr.USERNAME,
                        EXCHANGE = addr.EXCHANGE,
                        UUID = addr.UUID,           // This is the Account UUID
                        ADDRESS = addr.ADDRESS,
                        ADDRESS_INFO_ADDRESS = addr.ADDRESS_INFO.ADDRESS,
                        ADDRESS_LABEL = addr.ADDRESS_LABEL,
                        CALLBACK_URL = addr.CALLBACK_URL,
                        CREATED_AT = addr.CREATED_AT,
                        DEFAULT_RECEIVE = addr.DEFAULT_RECEIVE,
                        DEPOSIT_URI = addr.DEPOSIT_URI,
                        DESTINATION_TAG = addr.DESTINATION_TAG,
                        ID = addr.ID,
                        INLINE_WARNING_TEXT = addr.INLINE_WARNING.TEXT,
                        INLINE_WARNING_TOOLTIP = addr.INLINE_WARNING.TOOLTIP,
                        NAME = addr.NAME,
                        NETWORK = addr.NETWORK,
                        QR_CODE_IMAGE_URL = addr.QR_CODE_IMAGE_URL,
                        RECEIVE_SUBTITLE = addr.RECEIVE_SUBTITLE,
                        RESOURCE = addr.RESOURCE,
                        RESOURCE_PATH = addr.RESOURCE_PATH,
                        SHARE_ADDRESS_COPY_LINE1 = addr.SHARE_ADDRESS_COPY.LINE1,
                        SHARE_ADDRESS_COPY_LINE2 = addr.SHARE_ADDRESS_COPY.LINE2,
                        UPDATED_AT = addr.UPDATED_AT,
                        URI_SCHEME = addr.URI_SCHEME
                    };
                    // Is it 
                    tempAddresses.Add(addressView);
                }
                financeviewmodel.FinanceCryptoAddresses = tempAddresses;
            }
            return;
        }

        internal static void RebuildCryptoTransactions(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    List<SmartCrypto.V2Transactions> v2transactionsList,
                                                    string header = "",
                                                    short institutionCode = 0,
                                                    short brandCode = 0)
        {
            double TotalCostDouble = 0;
            double TotalValueDouble = 0;

            try
            {
                string fieldName = "CREATED_AT";
                if (header == "Activity")
                {
                    fieldName = "TYPE";
                }
                else
                {
                    if (header == "Crypto")
                    {
                        fieldName = "AMOUNT";
                    }
                }
                switch (fieldName)
                {
                    case "CREATED_AT":
#if CRYPTOS
                        if (financeviewmodel.createdat)
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.CREATED_AT
                             select Transactions).ToList();
                        }
                        else
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.CREATED_AT descending
                             select Transactions).ToList();
                        }
                        financeviewmodel.createdat = !financeviewmodel.createdat;
#endif
                        break;
                    case "TYPE":
#if CRYPTOS
                        if (financeviewmodel.types)
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.TYPE, Transactions.CREATED_AT ascending
                             select Transactions).ToList();
                        }
                        else
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.TYPE descending, Transactions.CREATED_AT descending
                             select Transactions).ToList();
                        }
                        financeviewmodel.types = !financeviewmodel.types;
#endif
                        break;
                    case "AMOUNT":
#if CRYPTOS
                        if (financeviewmodel.amount)
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.AMOUNT.CURRENCY, Transactions.CREATED_AT ascending
                             select Transactions).ToList();
                        }
                        else
                        {
                            financeviewmodel.v2transactionsList = new List<SmartCrypto.V2Transactions>
                            (from Transactions in financeviewmodel.v2transactionsList
                             orderby Transactions.AMOUNT.CURRENCY descending, Transactions.CREATED_AT descending
                             select Transactions).ToList();
                        }
                        financeviewmodel.amount = !financeviewmodel.amount;
#endif
                        break;
                    default:
                        break;
                }
                if (v2transactionsList.Count > 0)
                {
                    List<SmartFinance.CryptoTransactionsView> tempTransactions = new List<SmartFinance.CryptoTransactionsView>();
                    List<SmartCrypto.V2CryptoTotals> tempCryptoTotals = new List<SmartCrypto.V2CryptoTotals>();

                    foreach (SmartCrypto.V2Transactions tran in v2transactionsList)
                    {

                        bool doit = true;
                        switch (tran.TYPE)
                        {
                            case "fiat":
                            case "fiat_deposit":
                                doit = financeviewmodel.transfiat;
                                break;
                            case "buy":
                                doit = financeviewmodel.buy;
                                break;
                            case "sell":
                                doit = financeviewmodel.sell;
                                break;
                            case "trade":
                                doit = financeviewmodel.trade;
                                break;
                            case "send":
                                doit = financeviewmodel.send;
                                break;
                            case "interest":
                                doit = financeviewmodel.interest;
                                break;
                            default:
                                break;
                        }
                        if (doit)
                        {
                            short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, tran.AMOUNT.CURRENCY);
                            if (currencyOrdinal > 0)
                            {
                                if (tran.NETWORK.HASH == "")
                                {
#if CRYPTOS
                                    Console.WriteLine("Its Empty");
#endif
                                }
                                SmartFinance.CryptoTransactionsView cryptoView = new SmartFinance.CryptoTransactionsView()
                                {
                                    USERNAME = tran.USERNAME,
                                    CUBEFACE_CODE = SmartParametersV2016.Finance,
                                    INSTITUTION_CODE = institutionCode,
                                    BRAND_CODE = brandCode,
                                    EXCHANGE = tran.EXCHANGE,
                                    UUID = tran.UUID,
                                    UDPRN = tran.UDPRN,
                                    CREATED_AT = tran.CREATED_AT,
                                    TYPE = tran.TYPE,
                                    CRYPTO_CURRENCY = tran.AMOUNT.CURRENCY,
                                    CRYPTO_CURRENCY_ORDINAL = currencyOrdinal,
                                    CURRENCY_DISPLAY = "",
                                    CRYPTO_AMOUNT = Convert.ToDouble(tran.AMOUNT.AMOUNT),
                                    NATIVE_AMOUNT = Convert.ToDouble(tran.NATIVE_AMOUNT.AMOUNT),
                                    NATIVE_CURRENCY = tran.NATIVE_AMOUNT.CURRENCY,
                                    FEE_CURRENCY = tran.BUYSELL.FEE.CURRENCY,
                                    FEE_AMOUNT = Convert.ToDouble(tran.BUYSELL.FEE.AMOUNT),
                                    TO_ADDRESS = tran.TO.ADDRESS,
                                    HASH = tran.NETWORK.HASH,
                                    NETWORK_NAME = tran.NETWORK.NETWORK_NAME,
                                    ACCOUNT_CREATED = tran.ACCOUNT_CREATED

                                };
                                cryptoView.CRYPTO_RATE = (cryptoView.CRYPTO_AMOUNT / cryptoView.NATIVE_AMOUNT);//.ToString("F3");
                                cryptoView.TOTAL_COST = (cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT).ToString("F2");

                                tempTransactions.Add(cryptoView);

                                if (!(tran.TYPE == "fiat" ||
                                    tran.TYPE == "send"))
                                {
                                    short cryptoCurrencyOrdinal = cryptoView.CRYPTO_CURRENCY_ORDINAL;
                                    string cryptoCurrency = cryptoView.CRYPTO_CURRENCY;
                                    string cryptoFeeCurrency = cryptoView.FEE_CURRENCY;
                                    double cryptoRate = 0;
                                    if (cryptoCurrency == "MATIC")
                                    {
                                        cryptoCurrency = "POL";
                                        cryptoCurrencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, cryptoCurrency);
                                    }
                                    if (cryptoFeeCurrency == "MATIC")
                                    {
                                        cryptoFeeCurrency = "POL";
                                    }

                                    cryptoRate = CryptoBollocksV2025.FindRate(financeviewmodel, cryptoCurrencyOrdinal);
                                    
                                    List<SmartCrypto.CurrencyTotals> IsItThere1 = new List<SmartCrypto.CurrencyTotals>(
                                    from Totals in financeviewmodel.FinanceCurrencyTotals1
                                    where Totals.CRYPTO_CURRENCY == cryptoCurrency &&
                                        Totals.NATIVE_CURRENCY == cryptoView.NATIVE_CURRENCY &&
                                        Totals.FEE_CURRENCY == cryptoFeeCurrency
                                    select Totals);


                                    if (IsItThere1.Count > 0)
                                    {
                                        int index = IsItThere1[0].INDEX;
                                        financeviewmodel.FinanceCurrencyTotals1[index].CRYPTO_AMOUNT += cryptoView.CRYPTO_AMOUNT;
                                        financeviewmodel.FinanceCurrencyTotals1[index].NATIVE_AMOUNT += cryptoView.NATIVE_AMOUNT;
                                        financeviewmodel.FinanceCurrencyTotals1[index].FEE_AMOUNT += cryptoView.FEE_AMOUNT;
                                    }
                                    else
                                    {

                                        SmartCrypto.CurrencyTotals xyz = new SmartCrypto.CurrencyTotals()
                                        {
                                            INDEX = financeviewmodel.totalsIndex1,
                                            CRYPTO_CURRENCY_ORDINAL = cryptoCurrencyOrdinal,
                                            CRYPTO_CURRENCY = cryptoCurrency,
                                            CRYPTO_AMOUNT = cryptoView.CRYPTO_AMOUNT,
                                            NATIVE_CURRENCY = cryptoView.NATIVE_CURRENCY,
                                            NATIVE_AMOUNT = cryptoView.NATIVE_AMOUNT,
                                            FEE_CURRENCY = cryptoFeeCurrency,
                                            FEE_AMOUNT = cryptoView.FEE_AMOUNT,
                                            RATE = cryptoRate,
                                            VALUE = (cryptoView.CRYPTO_AMOUNT * cryptoRate).ToString("F3")
                                        };

                                        financeviewmodel.FinanceCurrencyTotals1.Add(xyz);
                                        financeviewmodel.totalsIndex1++;
                                    }

                                    List<SmartCrypto.CurrencyTotals> IsItThere2 = new List<SmartCrypto.CurrencyTotals>(
                                        from Totals in financeviewmodel.FinanceCurrencyTotals2
                                        where Totals.CRYPTO_CURRENCY == cryptoCurrency &&
                                            Totals.NATIVE_CURRENCY == cryptoView.NATIVE_CURRENCY
                                        select Totals);

                                    if (IsItThere2.Count > 0)
                                    {
                                        int index = IsItThere2[0].INDEX;
                                        financeviewmodel.FinanceCurrencyTotals2[index].CRYPTO_AMOUNT += cryptoView.CRYPTO_AMOUNT;
                                        financeviewmodel.FinanceCurrencyTotals2[index].NATIVE_AMOUNT += cryptoView.NATIVE_AMOUNT;
                                    }
                                    else
                                    {
                                        SmartCrypto.CurrencyTotals xyz = new SmartCrypto.CurrencyTotals()
                                        {
                                            INDEX = financeviewmodel.totalsIndex2,
                                            CRYPTO_CURRENCY = cryptoCurrency,
                                            CRYPTO_AMOUNT = cryptoView.CRYPTO_AMOUNT,
                                            NATIVE_CURRENCY = cryptoView.NATIVE_CURRENCY,
                                            NATIVE_AMOUNT = cryptoView.NATIVE_AMOUNT,
                                            RATE = cryptoRate,
                                            VALUE = (cryptoView.CRYPTO_AMOUNT * cryptoRate).ToString("F3")
                                        };
                                        financeviewmodel.FinanceCurrencyTotals2.Add(xyz);
                                        financeviewmodel.totalsIndex2++;
                                    }
                                }

                                // Try and calculate the Total Cost ??
                                // How accurate is this????
                                TotalCostDouble += cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT;

                                bool found_it = false;
                                foreach (SmartCrypto.V2CryptoTotals totals in tempCryptoTotals.ToList())
                                {
                                    if (totals.CRYPTO_CURRENCY == cryptoView.CRYPTO_CURRENCY)
                                    {
                                        totals.CRYPTO_AMOUNT += cryptoView.CRYPTO_AMOUNT;
                                        totals.VALUE += cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT;
                                        found_it = true;
                                        break;
                                    }
                                }
                                if (!found_it)
                                {
                                    SmartCrypto.V2CryptoTotals totals = new SmartCrypto.V2CryptoTotals()
                                    {
                                        CRYPTO_CURRENCY_ORDINAL = cryptoView.CRYPTO_CURRENCY_ORDINAL,
                                        CRYPTO_CURRENCY = cryptoView.CRYPTO_CURRENCY,
                                        CRYPTO_AMOUNT = cryptoView.CRYPTO_AMOUNT,
                                        VALUE = cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT
                                    };
                                    totals.RATE = CryptoBollocksV2025.FindRate(financeviewmodel, totals.CRYPTO_CURRENCY_ORDINAL);
                                    
                                    tempCryptoTotals.Add(totals);

                                    // Now we have to store the Currencies and Rates for
                                    // when we open the Program again, and DON'T do a scrape
                                    // We need to see something!!
                                    List<SmartFinance.CryptoCurrencyRates> what = new List<SmartFinance.CryptoCurrencyRates>(
                                        from Currency in financeviewmodel.PLO.crypto_currenciesList
                                        where Currency.CRYPTO_ORDINAL == totals.CRYPTO_CURRENCY_ORDINAL
                                        select Currency);
                                    if (what.Count == 0)
                                    {
                                        // Is it already IN changes?
                                        List<SmartFinance.CryptoCurrencyRates> whatnow = new List<SmartFinance.CryptoCurrencyRates>(
                                        from Currency in financeviewmodel.PLO.crypto_currenciesList
                                        where Currency.CRYPTO_ORDINAL == totals.CRYPTO_CURRENCY_ORDINAL
                                        select Currency);
                                        if (whatnow.Count == 0)
                                        {
                                            // No
                                            SmartFinance.CryptoCurrencyRates rate = new SmartFinance.CryptoCurrencyRates()
                                            {
                                                USERNAME = ourviewmodel.UserName,
                                                CUBEFACE_CODE = SmartParametersV2016.Finance,
                                                CRYPTO_ORDINAL = totals.CRYPTO_CURRENCY_ORDINAL,
                                                CRYPTO_RATE = totals.RATE,
                                                CRYPTO_RATE_DATE = DateTime.UtcNow,
                                                Updated = false                 // Its new
                                            };
                                            financeviewmodel.PLO.crypto_currencies_changesList.Add(rate);
                                        }
                                    }
                                    else
                                    {
                                        // Its there, so check to see if the rate has changed and
                                        // an update needed
                                        if (what.First().CRYPTO_RATE != totals.RATE)
                                        {
                                            SmartFinance.CryptoCurrencyRates currency = new SmartFinance.CryptoCurrencyRates()
                                            {
                                                CUBEFACE_CODE = SmartParametersV2016.Finance,
                                                CRYPTO_ORDINAL = totals.CRYPTO_CURRENCY_ORDINAL,
                                                CRYPTO_RATE = totals.RATE,
                                                CRYPTO_RATE_DATE = DateTime.UtcNow,
                                                Updated = true              // Wow a real update!!!
                                            };
                                            financeviewmodel.PLO.crypto_currencies_changesList.Add(currency);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    financeviewmodel.FinanceCryptoTransactions = tempTransactions;
                    tempCryptoTotals =
                        new List<SmartCrypto.V2CryptoTotals>
                        (from Currencies in tempCryptoTotals
                         orderby Currencies.CRYPTO_CURRENCY
                         select Currencies);
                    financeviewmodel.FinanceCryptoCurrencyRates = tempCryptoTotals;
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                return;
            }
            financeviewmodel.TotalCost = TotalCostDouble.ToString("F3");
            foreach (SmartCrypto.V2CryptoTotals v2crypto in financeviewmodel.FinanceCryptoCurrencyRates)
            {
                TotalValueDouble += v2crypto.CRYPTO_AMOUNT * v2crypto.RATE;
            }
#if WINFORMS || WPF  || WINUI
            financeviewmodel.TotalValue = TotalValueDouble.ToString("F3");
#endif
#if ANDROIDX
            financeviewmodel.TotalValue.Text = TotalValueDouble.ToString("F3");
#endif

            financeviewmodel.FeeWidth = 0;
            financeviewmodel.FinanceCurrencyTotals = financeviewmodel.FinanceCurrencyTotals2.ToList();
            return;
        }

        internal static void RebuildPLOAccounts(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                short institutionCode,
                                                short brandCode,
                                                char categoryCode,
                                                string header = "")
        {
            List<SmartFinance.CryptoAccounts> temp = new List<SmartFinance.CryptoAccounts>();
            foreach (SmartFinance.CryptoAccountsView crypa in financeviewmodel.FinanceCryptoAccounts)
            {
                short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, crypa.CURRENCY);
                // Only do this if we have found a genuine currency
                if (currencyOrdinal == 0)
                {
#if CRYPTOS
                    Console.WriteLine("stop");
#endif
                }
                
                // Is it in PLO already?
                List<SmartFinance.CryptoAccounts> crypaFound =
                    new List<SmartFinance.CryptoAccounts>(
                        from CrypAcc in financeviewmodel.PLO.crypto_accountsList
                        where CrypAcc.USERNAME == crypa.USERNAME &&
                            CrypAcc.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                            CrypAcc.INSTITUTION_CODE == institutionCode &&
                            CrypAcc.BRAND_CODE == brandCode &&
                            CrypAcc.SORTCODE == crypa.EXCHANGE &&
                            CrypAcc.ACCOUNT_NO == crypa.UUID &&
                            CrypAcc.UDPRN == crypa.UDPRN
                        select CrypAcc);
                if (crypaFound.Count == 0)
                {
                    // Is it in the changes list?
                    List<SmartFinance.CryptoAccounts> crypaFound1 =
                    new List<SmartFinance.CryptoAccounts>(
                        from CrypAcc in financeviewmodel.PLO.crypto_accounts_changesList
                        where CrypAcc.USERNAME == crypa.USERNAME &&
                            CrypAcc.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                            CrypAcc.INSTITUTION_CODE == institutionCode &&
                            CrypAcc.BRAND_CODE == brandCode &&
                            CrypAcc.SORTCODE == crypa.EXCHANGE &&
                            CrypAcc.ACCOUNT_NO == crypa.UUID &&
                            CrypAcc.UDPRN == crypa.UDPRN
                        select CrypAcc);
                    if (crypaFound1.Count == 0)
                    {
                        SmartFinance.CryptoAccounts ca = new SmartFinance.CryptoAccounts()
                        {
                            USERNAME = crypa.USERNAME,
                            CUBEFACE_CODE = SmartParametersV2016.Finance,
                            INSTITUTION_CODE = institutionCode,
                            BRAND_CODE = brandCode,
                            SORTCODE = crypa.EXCHANGE,
                            ACCOUNT_NO = crypa.UUID,
                            UDPRN = crypa.UDPRN,
                            CREATED_AT = crypa.CREATED_AT,
                            TYPE = crypa.TYPE,
                            AVAILABLE_BALANCE_VALUE = crypa.AVAILABLE_BALANCE_VALUE,
                            AVAILABLE_BALANCE_CURRENCY = crypa.AVAILABLE_BALANCE_CURRENCY,
                            DEFAULT = crypa.DEFAULT,
                            DELETED_AT = crypa.DELETED_AT,
                            HOLD_VALUE = crypa.HOLD_VALUE,
                            HOLD_CURRENCY = crypa.HOLD_CURRENCY,
                            NAME = crypa.NAME,
                            //PLATFORM = crypa.PLATFORM,
                            READY = crypa.READY,
                            RETAIL_PORTFOLIO_ID = crypa.RETAIL_PORTFOLIO_ID,
                            UPDATED_AT = crypa.UPDATED_AT,
                            Updated = false
                        };
                        financeviewmodel.PLO.crypto_accounts_changesList.Add(ca);
                    }
                    else
                    {
                        Console.WriteLine("Here");
                    }
                }
                bool status = Convert.ToBoolean(crypa.ACTIVE);
                char STATUS = 'C';  // Closed
                if (status)
                {
                    STATUS = 'O';   // Open
                }

                SmartFinance.Accounts fca = new SmartFinance.Accounts()
                {
                    USERNAME = crypa.USERNAME,
                    CUBEFACE_CODE = SmartParametersV2016.Finance,
                    INSTITUTION_CODE = institutionCode,
                    BRAND_CODE = brandCode,
                    SORTCODE = crypa.EXCHANGE,
                    ACCOUNT_NO = crypa.UUID,
                    UDPRN = crypa.UDPRN,
                    ACCOUNT_CREATED = crypa.CREATED_AT,
                    CATEGORY_CODE = categoryCode,
                    CURRENCY_ORDINAL = currencyOrdinal,
                    ACCOUNT_TITLE = crypa.NAME,
                    ACCOUNT_BALANCE = 0,//Convert.ToInt32(crypa.AVAILABLE_BALANCE_VALUE.Replace(".", "")),
                    STATUS = STATUS,
                    Updated = false
                };
                // Is it in PLO already?
                List<SmartFinance.Accounts> fcaFound =
                    new List<SmartFinance.Accounts>(
                        from CrypAcc in financeviewmodel.PLO.finance_accountsList
                        where CrypAcc.USERNAME == fca.USERNAME &&
                            CrypAcc.CUBEFACE_CODE == fca.CUBEFACE_CODE &&
                            CrypAcc.INSTITUTION_CODE == fca.INSTITUTION_CODE &&
                            CrypAcc.BRAND_CODE == fca.BRAND_CODE &&
                            CrypAcc.SORTCODE == fca.SORTCODE &&
                            CrypAcc.ACCOUNT_NO == fca.ACCOUNT_NO &&
                            CrypAcc.UDPRN == fca.UDPRN &&
                            CrypAcc.CURRENCY_ORDINAL == fca.CURRENCY_ORDINAL
                        select CrypAcc);
                if (crypaFound.Count == 0)
                {
                    // Is it in the changes list?
                    fcaFound =
                    new List<SmartFinance.Accounts>(
                        from CrypAcc in financeviewmodel.PLO.finance_accounts_changesList
                        where CrypAcc.USERNAME == fca.USERNAME &&
                            CrypAcc.CUBEFACE_CODE == fca.CUBEFACE_CODE &&
                            CrypAcc.INSTITUTION_CODE == fca.INSTITUTION_CODE &&
                            CrypAcc.BRAND_CODE == fca.BRAND_CODE &&
                            CrypAcc.SORTCODE == fca.SORTCODE &&
                            CrypAcc.ACCOUNT_NO == fca.ACCOUNT_NO &&
                            CrypAcc.UDPRN == fca.UDPRN &&
                            CrypAcc.CURRENCY_ORDINAL == fca.CURRENCY_ORDINAL

                        select CrypAcc);
                    if (fcaFound.Count == 0)
                    {
                        financeviewmodel.PLO.finance_accounts_changesList.Add(fca);
                    }
                }
            }
            return;
        }

        internal static void RebuildPLOAddresses(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    short institutionCode,
                                                    short brandCode)
        {
            foreach (SmartCrypto.V2Addresses addr in financeviewmodel.v2addressesList)
            {

                short currencyOrdinal = SmartSpikeV2017.Lookup_Currency_OrdinalOld(ourviewmodel.currenciesList, "GBP");

                List<SmartFinance.CryptoAddresses> addressFound = new List<SmartFinance.CryptoAddresses>(
                    from Addr in financeviewmodel.PLO.crypto_addressesList
                    where Addr.USERNAME == addr.USERNAME &&
                    Addr.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                    Addr.INSTITUTION_CODE == institutionCode &&
                    Addr.BRAND_CODE == brandCode &&
                    Addr.SORTCODE == addr.EXCHANGE &&
                    Addr.ACCOUNT_NO == addr.UUID &&
                    //Addr.CURRENCY_ORDINAL == currencyOrdinal &&
                    Addr.UDPRN == addr.UDPRN
                    select Addr);
                if (addressFound.Count == 0)
                {
                    SmartFinance.CryptoAddresses cryptoAddress = new SmartFinance.CryptoAddresses()
                    {
                        USERNAME = addr.USERNAME,
                        CUBEFACE_CODE = SmartParametersV2016.Finance,
                        INSTITUTION_CODE = institutionCode,
                        BRAND_CODE = brandCode,
                        SORTCODE = addr.EXCHANGE,
                        ACCOUNT_NO = addr.UUID,
                        //CURRENCY_ORDINAL = currencyOrdinal,
                        UDPRN = addr.UDPRN,
                        ADDRESS = addr.ADDRESS,
                        ADDRESS_INFO_ADDRESS = addr.ADDRESS_INFO.ADDRESS,
                        ADDRESS_LABEL = addr.ADDRESS_LABEL,
                        CALLBACK_URL = addr.CALLBACK_URL,
                        CREATED_AT = addr.CREATED_AT,
                        DEFAULT_RECEIVE = addr.DEFAULT_RECEIVE,
                        DEPOSIT_URI = addr.DEPOSIT_URI,
                        DESTINATION_TAG = addr.DESTINATION_TAG,
                        ID = addr.ID,
                        INLINE_WARNING_TEXT = addr.INLINE_WARNING.TEXT,
                        INLINE_WARNING_TOOLTIP = addr.INLINE_WARNING.TOOLTIP,
                        NAME = addr.NAME,
                        NETWORK = addr.NETWORK,
                        QR_CODE_IMAGE_URL = addr.QR_CODE_IMAGE_URL,
                        RECEIVE_SUBTITLE = addr.RECEIVE_SUBTITLE,
                        RESOURCE = addr.RESOURCE,
                        RESOURCE_PATH = addr.RESOURCE_PATH,
                        SHARE_ADDRESS_COPY_LINE1 = addr.SHARE_ADDRESS_COPY.LINE1,
                        SHARE_ADDRESS_COPY_LINE2 = addr.SHARE_ADDRESS_COPY.LINE2,
                        UPDATED_AT = addr.UPDATED_AT,
                        URI_SCHEME = addr.URI_SCHEME,
                        Updated = false
                    };
                    financeviewmodel.PLO.crypto_addresses_changesList.Add(cryptoAddress);
                }
            }
            return;
        }

        internal static void RebuildCurrencies(FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel.v2exchangeratesList.Count > 0)
            {
                List<SmartCrypto.V2CryptoTotals> temp = new List<SmartCrypto.V2CryptoTotals>();

                foreach (SmartCrypto.V2CryptoTotals totals in financeviewmodel.FinanceCryptoCurrencyRates)
                {
                    totals.RATE = CryptoBollocksV2025.FindRate(financeviewmodel, totals.CRYPTO_CURRENCY_ORDINAL);                    
                    temp.Add(totals);
                }
                financeviewmodel.FinanceCryptoCurrencyRates = temp;
            }
            return;
        }

        internal static string generateToken(FinanceViewModel financeviewmodel,
                                                string name,
                                                string secret,
                                                string uri)
        {
            var privateKeyBytes = Convert.FromBase64String(secret); // Assuming PEM is base64 encoded
            using var key = ECDsa.Create();
            key.ImportECPrivateKey(privateKeyBytes, out _);

            Dictionary<string, object> payload = new Dictionary<string, object>
             {
                 { "sub", name },
                 { "iss", "coinbase-cloud" },
                 { "nbf", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                 { "exp", Convert.ToInt64((DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                 { "uri", uri }
             };

            Dictionary<string, object> extraHeaders = new Dictionary<string, object>
             {
                 { "kid", name },
                 // add nonce to prevent replay attacks with a random 10 digit number
                 { "nonce", randomHex(financeviewmodel, 10) },
                 { "typ", "JWT"}
             };

            string encodedToken = JWT.Encode(payload, key, JwsAlgorithm.ES256, extraHeaders);

            // print token
#if CRYPTOS
            //Console.WriteLine(encodedToken);
#endif
            return encodedToken;
        }

        internal static bool isTokenValid(string token, string tokenId, string secret)
        {
            if (token == null)
            {
                return false;
            }
            ECDsa key = ECDsa.Create();
            // Crypto is not available in UWP without
            // AN ENORMOUS AMOUNT OF FARTING ABOUT
            key?.ImportECPrivateKey(Convert.FromBase64String(secret), out _);

            ECDsaSecurityKey securityKey = new ECDsaSecurityKey(key) { KeyId = tokenId };

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                return true;
            }
            catch
            {
                return false;
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
        internal static void AnalyzeExchangeRates(
                                                MainViewModel ourviewmodel,
                                                string workingcurrency,
                                                string exchangeratesData,
                                                List<SmartCrypto.V2ExchangeRates> exchangeratesList)
        {
            if (string.IsNullOrEmpty(exchangeratesData))
                return;

            try
            {
                using JsonDocument doc = JsonDocument.Parse(exchangeratesData);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("data", out JsonElement data))
                    return;

                // Clear previous
                exchangeratesList.Clear();

                // "data" is an object
                if (data.TryGetProperty("currency", out JsonElement currencyElement))
                {
                    string currency = currencyElement.GetString();

                    // These are all based to the USD!
                    if (currency != workingcurrency)
                    {
                        return; // !!
                    }
                }

                if (data.TryGetProperty("rates", out JsonElement rates))
                {
                    // "rates" is an object: { "GBP": "0.79", "EUR": "0.92", ... }
                    foreach (JsonProperty exchangerateItem in rates.EnumerateObject())
                    {
                        SmartCrypto.V2ExchangeRates v2exchangerate = new SmartCrypto.V2ExchangeRates();

                        v2exchangerate.NAMEX = exchangerateItem.Name;
                        v2exchangerate.RATE = exchangerateItem.Value.ToString();

                        v2exchangerate.NAME_ORDINAL =
                            SmartSpikeV2017.Lookup_Currency_OrdinalOld(
                                ourviewmodel.currenciesList,
                                v2exchangerate.NAMEX);

                        if (v2exchangerate.NAME_ORDINAL > 0)
                        {
                            exchangeratesList.Add(v2exchangerate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
            }
        }


    // Doesn't appear to be needed ...??

    //internal static void AnalyzeCurrencies(string currenciesData, List<SmartCrypto.V2Currencies> v2currenciesList)
    //{
    //    dinamic jsonResponse = JObject.Parse(currenciesData);
    //    foreach (dinamic statement in jsonResponse)
    //    {
    //        string statement_name = statement.Name;
    //        if (statement_name == "data")   // We're not interested in pagination as of Phase ONE
    //        {
    //            foreach (dinamic transactions in statement)
    //            {
    //                foreach (dinamic transaction in transactions)
    //                {
    //                    SmartCrypto.V2Currencies v2currency = new SmartCrypto.V2Currencies();
    //                    foreach (dinamic currencyItem in transaction)
    //                    {
    //                        string currencyName = currencyItem.Name;
    //                        switch (currencyName)
    //                        {
    //                            case "id":
    //                                v2currency.ID = currencyItem.Value.ToString();
    //                                break;
    //                            case "min_size":
    //                                v2currency.MIN_SIZE = currencyItem.Value.ToString();
    //                                break;
    //                            case "name":
    //                                v2currency.NAME = currencyItem.Value.ToString();
    //                                break;
    //                            default:
    //                                break;
    //                        }
    //                    }
    //                    if (v2currency.ID != null)
    //                    {
    //                        v2currenciesList.Add(v2currency);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return;
    //}

    
    internal static void AnalyzeUserV2(
                                string responseAddress,
                                List<SmartCrypto.V2User> userList,
                                string Username,
                                string AccessId)
        {
            SmartCrypto.V2User cryptoUser = new SmartCrypto.V2User();

            using JsonDocument jsonDoc = JsonDocument.Parse(responseAddress);

            foreach (JsonProperty statement in jsonDoc.RootElement.EnumerateObject())
            {
                if (statement.Name == "data") // We're not interested in pagination
                {
                    foreach (JsonElement transactions in statement.Value.EnumerateArray())
                    {
                        foreach (JsonProperty individual in transactions.EnumerateObject())
                        {
                            string column = individual.Name;
                            JsonElement value = individual.Value;

                            switch (column)
                            {
                                case "bitcoin_unit":
                                    cryptoUser.BITCOIN_UNIT = value.GetString() ?? "";
                                    break;
                                case "country":
                                    cryptoUser.COUNTRY = value.GetString() ?? "";
                                    break;
                                case "created_at":
                                    if (value.ValueKind != JsonValueKind.Null)
                                    {
                                        cryptoUser.CREATED_AT = value.GetDateTime();
                                    }
                                    else
                                    {
                                        cryptoUser.CREATED_AT = SmartParametersV2016.defaultDate;
                                    }
                                    break;
                                case "email":
                                    cryptoUser.EMAIL = value.GetString() ?? "";
                                    break;
                                case "id":
                                    cryptoUser.ID = value.GetString() ?? "";
                                    break;
                                case "name":
                                    cryptoUser.NAME = value.GetString() ?? "";
                                    break;
                                case "native_currency":
                                    cryptoUser.NATIVE_CURRENCY = value.GetString() ?? "";
                                    break;
                                case "resource":
                                    cryptoUser.RESOURCE = value.GetString() ?? "";
                                    break;
                                case "resource_path":
                                    cryptoUser.RESOURCE_PATH = value.GetString() ?? "";
                                    break;
                                case "time_zone":
                                    if (value.ValueKind != JsonValueKind.Null)
                                    {
                                        try
                                        {
                                            string tz = value.GetString() ?? "";
                                            tz = tz.Replace("Atlantic Time (Canada)", "Atlantic Standard Time");
                                            cryptoUser.TIME_ZONE = TimeZoneInfo.FindSystemTimeZoneById(tz);
                                        }
                                        catch
                                        {
                                            cryptoUser.TIME_ZONE = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
                                        }
                                    }
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(cryptoUser.ID))
                        {
                            cryptoUser.USERNAME = Username;
                            cryptoUser.EMAIL = AccessId; // Looks like original code overwrites EMAIL here
                            userList.Add(cryptoUser);
                        }
                    }
                }
            }
        }

    
        internal static void AnalyzeAccountV2(
                                            string accountsData,
                                            List<SmartCrypto.V2Accounts> v2accountsList,
                                            string Username,
                                            string Exchange,
                                            string AccessId)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(accountsData);
                JsonElement root = doc.RootElement;

                SmartCrypto.V2Accounts v2acc = new SmartCrypto.V2Accounts();
                v2acc.PORTFOLIO_ID = "";

                foreach (JsonProperty individual in root.EnumerateObject())
                {
                    string column = individual.Name;

                    switch (column)
                    {
                        case "allow_deposits":
                            v2acc.ALLOW_DEPOSITS = individual.Value.ToString();
                            break;

                        case "allow_withdrawals":
                            v2acc.ALLOW_WITHDRAWALS = individual.Value.ToString();
                            break;

                        case "balance":
                            foreach (JsonProperty balanceItem in individual.Value.EnumerateObject())
                            {
                                var balance = v2acc.BALANCE;
                                switch (balanceItem.Name)
                                {
                                    case "amount":
                                        balance.AMOUNT = balanceItem.Value.ToString();
                                        break;
                                    case "currency":
                                        balance.CURRENCY = balanceItem.Value.ToString();
                                        break;
                                }
                                v2acc.BALANCE = balance;
                            }
                            break;

                        case "created_at":
                            if (individual.Value.ValueKind != JsonValueKind.Null)
                            {
                                v2acc.CREATED_AT = individual.Value.GetDateTime();
                            }
                            else
                            {
                                v2acc.CREATED_AT = SmartParametersV2016.defaultDate;
                            }
                            break;

                        case "currency":
                            foreach (JsonProperty currencyItem in individual.Value.EnumerateObject())
                            {
                                var currency = v2acc.CURRENCY;
                                switch (currencyItem.Name)
                                {
                                    case "asset_id":
                                        currency.ASSET_ID = currencyItem.Value.ToString();
                                        break;
                                    case "code":                                        
                                        currency.CODE = currencyItem.Value.ToString();
                                        break;
                                    case "color":
                                        currency.COLOR = currencyItem.Value.ToString();
                                        break;
                                    case "exponent":
                                        currency.EXPONENT = currencyItem.Value.ToString();
                                        break;
                                    case "name":
                                        currency.NAME = currencyItem.Value.ToString();
                                        break;

                                    case "rewards":
                                        if (currencyItem.Value.ValueKind == JsonValueKind.Object)
                                        {
                                            foreach (JsonProperty rewardItem in currencyItem.Value.EnumerateObject())
                                            {
                                                var rewards = v2acc.CURRENCY.REWARDS;
                                                switch (rewardItem.Name)
                                                {
                                                    case "apy":
                                                        rewards.APY = rewardItem.Value.ToString();
                                                        break;
                                                    case "formatted_apy":
                                                        rewards.FORMATTED_APY = rewardItem.Value.ToString();
                                                        break;
                                                    case "label":
                                                        rewards.LABEL = rewardItem.Value.ToString();
                                                        break;
                                                }
                                                currency.REWARDS = rewards;
                                            }
                                        }
                                        break;

                                    case "slug":
                                        currency.SLUG = currencyItem.Value.ToString();
                                        break;
                                    case "type":
                                        currency.TYPE = currencyItem.Value.ToString();
                                        break;
                                }
                                v2acc.CURRENCY = currency;
                            }
                            break;

                        case "id":
                            v2acc.ID = individual.Value.ToString();
                            break;

                        case "name":
                            v2acc.NAME = individual.Value.ToString();
                            break;

                        case "primary":
                            v2acc.PRIMARY = individual.Value.ToString();
                            break;

                        case "resource":
                            v2acc.RESOURCE = individual.Value.ToString();
                            break;

                        case "resource_path":
                            v2acc.RESOURCE_PATH = individual.Value.ToString();
                            break;

                        case "type":
                            v2acc.TYPE = individual.Value.ToString();
                            break;

                        case "updated_at":
                            if (individual.Value.ValueKind != JsonValueKind.Null)
                            {
                                v2acc.UPDATED_AT = individual.Value.GetDateTime();
                            }
                            else
                            {
                                v2acc.UPDATED_AT = SmartParametersV2016.defaultDate;
                            }
                            break;

                        default:
                            Console.Write(column);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(v2acc.ID))
                {
                    v2acc.USERNAME = Username;
                    v2acc.EXCHANGE = Exchange;
                    v2acc.UDPRN = AccessId;

                    v2accountsList.Add(v2acc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static void AnalyzeAddressV2(
                                    string responseAddress,
                                    List<SmartCrypto.V2Addresses> addressesList,
                                    string Username,
                                    string Exchange,
                                    string AccessId,
                                    string Uuid)
        {
            using JsonDocument jsonDoc = JsonDocument.Parse(responseAddress);
            JsonElement root = jsonDoc.RootElement;

            if (!root.TryGetProperty("data", out JsonElement dataElement) ||
                dataElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }
            foreach (JsonElement transactions in dataElement.EnumerateArray())
            {
                if (transactions.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }
                foreach (JsonElement transaction in transactions.EnumerateArray())
                {
                    if (transaction.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }
                    SmartCrypto.V2Addresses addr = new SmartCrypto.V2Addresses();

                    foreach (JsonProperty individual in transaction.EnumerateObject())
                    {
                        string column = individual.Name;
                        JsonElement value = individual.Value;

                        switch (column)
                        {
                            case "address":
                                addr.ADDRESS = value.GetString();
                                break;

                            case "address_info":
                                if (value.ValueKind == JsonValueKind.Object)
                                {
                                    foreach (JsonProperty addressItem in value.EnumerateObject())
                                    {
                                        var address = addr.ADDRESS_INFO;
                                        if (addressItem.Name == "address")
                                        {
                                            address.ADDRESS = addressItem.Value.GetString();
                                        }
                                        addr.ADDRESS_INFO = address;
                                    }
                                }
                                break;

                            case "address_label":
                                addr.ADDRESS_LABEL = value.GetString();
                                break;

                            case "callback_url":
                                addr.CALLBACK_URL = value.GetString();
                                break;

                            case "created_at":
                                addr.CREATED_AT = value.ValueKind != JsonValueKind.Null
                                    ? Convert.ToDateTime(value.GetString())
                                    : SmartParametersV2016.defaultDate;
                                break;

                            case "default_receive":
                                addr.DEFAULT_RECEIVE = value.GetString();
                                break;

                            case "deposit_uri":
                                addr.DEPOSIT_URI = value.GetString();
                                break;

                            case "destination_tag":
                                addr.DESTINATION_TAG = value.GetString();
                                break;

                            case "id":
                                addr.ID = value.GetString();
                                break;

                            case "inline_warning":
                                if (value.ValueKind == JsonValueKind.Object)
                                {
                                    foreach (JsonProperty inlineItem in value.EnumerateObject())
                                    {
                                        var inline = addr.INLINE_WARNING;
                                        switch (inlineItem.Name)
                                        {
                                            case "text":
                                                inline.TEXT = inlineItem.Value.GetString();
                                                break;
                                            case "tooltip":
                                                inline.TOOLTIP = inlineItem.Value.GetString();
                                                break;
                                        }
                                        addr.INLINE_WARNING = inline;
                                    }
                                }
                                break;

                            case "name":
                                addr.NAME = value.GetString();
                                break;

                            case "network":
                                addr.NETWORK = value.GetString();
                                break;

                            case "qr_code_image_url":
                                addr.QR_CODE_IMAGE_URL = value.GetString();
                                break;

                            case "receive_subtitle":
                                addr.RECEIVE_SUBTITLE = value.GetString();
                                break;

                            case "resource":
                                addr.RESOURCE = value.GetString();
                                break;

                            case "resource_path":
                                addr.RESOURCE_PATH = value.GetString();
                                break;

                            case "share_address_copy":
                                if (value.ValueKind == JsonValueKind.Object)
                                {
                                    foreach (JsonProperty shareItem in value.EnumerateObject())
                                    {
                                        var share = addr.SHARE_ADDRESS_COPY;
                                        switch (shareItem.Name)
                                        {
                                            case "line1":
                                                share.LINE1 = shareItem.Value.GetString();
                                                break;
                                            case "line2":
                                                share.LINE2 = shareItem.Value.GetString();
                                                break;
                                        }
                                        addr.SHARE_ADDRESS_COPY = share;
                                    }
                                }
                                break;

                            case "updated_at":
                                addr.UPDATED_AT = value.ValueKind != JsonValueKind.Null
                                    ? Convert.ToDateTime(value.GetString())
                                    : SmartParametersV2016.defaultDate;
                                break;

                            case "uri_scheme":
                                addr.URI_SCHEME = value.GetString();
                                break;

                            default:
                                if (column != "warnings" && column != "launch_warning")
                                {
                                    Console.Write(column);
                                }
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(addr.ID))
                    {
                        addr.USERNAME = Username;
                        addr.EXCHANGE = Exchange;
                        addr.UDPRN = AccessId;
                        addr.UUID = Uuid;

                        addressesList.Add(addr);
                    }
                }
            }
            return;
        }
        internal static void AnalyzeTransactionV2(
                                string transactionsData,
                                List<SmartCrypto.V2Transactions> transactionsidList,
                                string Username,
                                string Exchange,
                                string AccessId,
                                string Uuid,
                                DateTime AccountCreated)
        {
            SmartCrypto.V2Transactions v2trans = new SmartCrypto.V2Transactions();
            // BUYSELL -> FEE
            var buySell = v2trans.BUYSELL;
            var fee = buySell.FEE;
            fee.AMOUNT = "0";
            fee.CURRENCY = "";
            buySell.FEE = fee;
            v2trans.BUYSELL = buySell;
            // TO
            var to = v2trans.TO;
            to.ADDRESS = "";
            v2trans.TO = to;
            // NETWORK
            var network = v2trans.NETWORK;
            network.HASH = "";
            network.NETWORK_NAME = "";
            network.STATUS = "";
            network.STATUS_DESCRIPTION = "";
            network.TRANSACTION_URL = "";
            network.TRANSACTION_FEE = "";
            v2trans.NETWORK = network;
            v2trans.IDEM = "";
            v2trans.DESCRIPTION = "";

            using JsonDocument jsonDoc = JsonDocument.Parse(transactionsData);

            foreach (JsonProperty individual in jsonDoc.RootElement.EnumerateObject())
            {
                string column = individual.Name;
                JsonElement value = individual.Value;

                switch (column)
                {
                    case "amount":
                        foreach (JsonProperty amountItem in value.EnumerateObject())
                        {
                            var amount = v2trans.AMOUNT;
                            switch (amountItem.Name)
                            {
                                case "amount":
                                    amount.AMOUNT = amountItem.Value.GetRawText().Trim('"');
                                    break;
                                case "currency":
                                    amount.CURRENCY = amountItem.Value.GetString() ?? "";
                                    break;
                            }
                            v2trans.AMOUNT = amount;
                        }
                        break;

                    case "buy":
                    case "trade":
                    case "sell":
                        foreach (JsonProperty amountItem in value.EnumerateObject())
                        {
                            switch (amountItem.Name)
                            {
                                case "fee":
                                    foreach (JsonProperty feeItem in amountItem.Value.EnumerateObject())
                                    {
                                        var buysellx = v2trans.BUYSELL;
                                        var feex = buysellx.FEE;
                                        switch (feeItem.Name)
                                        {
                                            case "amount":
                                                feex.AMOUNT = feeItem.Value.GetRawText().Trim('"');
                                                break;
                                            case "currency":
                                                feex.CURRENCY = feeItem.Value.GetString() ?? "";
                                                break;
                                        }
                                        buysellx.FEE = feex;
                                        v2trans.BUYSELL = buysellx;
                                    }
                                    break;

                                case "id":
                                    var buysellid = v2trans.BUYSELL;
                                    buysellid.ID = amountItem.Value.GetString() ?? "";
                                    v2trans.BUYSELL = buysellid;
                                    break;
                                case "payment_method_name":
                                    var buysellpayment = v2trans.BUYSELL;
                                    buysellpayment.PAYMENT_METHOD_NAME = amountItem.Value.GetString() ?? "";
                                    v2trans.BUYSELL = buysellpayment;
                                    break;
                                case "subtotal":
                                    foreach (JsonProperty subtotalItem in amountItem.Value.EnumerateObject())
                                    {
                                        var buysell = v2trans.BUYSELL;
                                        var subtotal = buysell.SUBTOTAL;
                                        switch (subtotalItem.Name)
                                        {
                                            case "amount":
                                                subtotal.AMOUNT = subtotalItem.Value.GetRawText().Trim('"');
                                                break;
                                            case "currency":
                                                subtotal.CURRENCY = subtotalItem.Value.GetString() ?? "";
                                                break;
                                        }
                                        buysell.SUBTOTAL = subtotal;
                                        v2trans.BUYSELL = buysell;
                                    }
                                    break;
                                case "total":
                                    
                                    foreach (JsonProperty totalItem in amountItem.Value.EnumerateObject())
                                    {
                                        var buysell = v2trans.BUYSELL;
                                        var total = buysell.TOTAL;
                                        switch (totalItem.Name)
                                        {
                                            case "amount":
                                                total.AMOUNT = totalItem.Value.GetRawText().Trim('"');
                                                break;
                                            case "currency":
                                                total.CURRENCY = totalItem.Value.GetString() ?? "";
                                                break;
                                        }
                                        buysell.TOTAL = total;
                                        v2trans.BUYSELL = buysell;
                                    }
                                    break;
                            }
                        }
                        break;

                    case "created_at":
                        if (value.ValueKind != JsonValueKind.Null)
                        {
                            v2trans.CREATED_AT = value.GetDateTime();
                        }
                        else
                        {
                            v2trans.CREATED_AT = SmartParametersV2016.defaultDate;
                        }
                        break;

                    case "id":
                        v2trans.ID = value.GetString() ?? "";
                        break;

                    case "idem":
                        v2trans.IDEM = value.GetString() ?? "";
                        break;

                    case "native_amount":
                        foreach (JsonProperty amountItem in value.EnumerateObject())
                        {
                            var native = v2trans.NATIVE_AMOUNT;
                            switch (amountItem.Name)
                            {
                                case "amount":
                                    native.AMOUNT = amountItem.Value.GetRawText().Trim('"');
                                    break;
                                case "currency":
                                    native.CURRENCY = amountItem.Value.GetString() ?? "";
                                    break;
                            }
                            v2trans.NATIVE_AMOUNT = native;
                        }
                        break;

                    case "network":
                        foreach (JsonProperty networkItem in value.EnumerateObject())
                        {
                            var tnetwork = v2trans.NETWORK;
                            switch (networkItem.Name)
                            {
                                case "hash":
                                    tnetwork.HASH = networkItem.Value.GetString() ?? "";
                                    break;
                                case "network_name":
                                    tnetwork.NETWORK_NAME = networkItem.Value.GetString() ?? "";
                                    break;
                                case "status":
                                    tnetwork.STATUS = networkItem.Value.GetString() ?? "";
                                    break;
                                case "status_description":
                                    tnetwork.STATUS_DESCRIPTION = networkItem.Value.GetString() ?? "";
                                    break;
                                case "transaction_url":
                                    tnetwork.TRANSACTION_URL = networkItem.Value.GetString() ?? "";
                                    break;
                                case "transaction_fee":
                                    tnetwork.TRANSACTION_FEE = networkItem.Value.GetString() ?? "";
                                    break;
                            }
                            v2trans.NETWORK = tnetwork;
                        }
                        break;

                    case "resource":
                        v2trans.RESOURCE = value.GetString() ?? "";
                        break;
                    case "resource_path":
                        v2trans.RESOURCE_PATH = value.GetString() ?? "";
                        break;
                    case "status":
                        v2trans.STATUS = value.GetString() ?? "";
                        break;

                    case "to":
                        foreach (JsonProperty toItem in value.EnumerateObject())
                        {
                            var transto = v2trans.TO;
                            switch (toItem.Name)
                            {
                                case "address":
                                    transto.ADDRESS = toItem.Value.GetString() ?? "";
                                    break;
                                case "resource":
                                    transto.RESOURCE = toItem.Value.GetString() ?? "";
                                    break;
                            }
                            v2trans.TO = transto;
                        }
                        break;

                    case "type":
                        v2trans.TYPE = value.GetString() ?? "";
                        break;

                    case "description":
                        v2trans.DESCRIPTION = value.GetString() ?? "";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(v2trans.ID))
            {
                v2trans.USERNAME = Username;
                v2trans.EXCHANGE = Exchange;
                v2trans.UUID = Uuid;
                v2trans.UDPRN = AccessId;
                v2trans.ACCOUNT_CREATED = AccountCreated;

                transactionsidList.Add(v2trans);
            }
        }
    
        
        internal static void AnalyzeAccountV3(
                                string accountsData,
                                List<SmartCrypto.V3Accounts> uuidList,
                                string Username,
                                string Exchange,
                                string Udprn)
        {
            SmartCrypto.V3Accounts v3acc = new SmartCrypto.V3Accounts();

            using JsonDocument jsonDoc = JsonDocument.Parse(accountsData);

            foreach (JsonProperty individual in jsonDoc.RootElement.EnumerateObject())
            {
                string column = individual.Name;
                JsonElement value = individual.Value;

                switch (column)
                {
                    case "active":
                        v3acc.ACTIVE = value.GetString() ?? "";
                        break;

                    case "available_balance":
                        foreach (JsonProperty balanceItem in value.EnumerateObject())
                        {
                            var available = v3acc.AVAILABLE_BALANCE;
                            switch (balanceItem.Name)
                            {
                                case "value":
                                    available.VALUE = balanceItem.Value.GetRawText().Trim('"');
                                    break;
                                case "currency":
                                    available.CURRENCY = balanceItem.Value.GetString() ?? "";
                                    break;
                            }
                            v3acc.AVAILABLE_BALANCE = available;
                        }
                        break;

                    case "created_at":
                        v3acc.CREATED_AT = value.ValueKind != JsonValueKind.Null
                            ? value.GetDateTime()
                            : SmartParametersV2016.defaultDate;
                        break;

                    case "currency":
                        v3acc.CURRENCY = value.GetString() ?? "";
                        break;

                    case "default":
                        v3acc.DEFAULT = value.GetString() ?? "";
                        break;

                    case "deleted_at":
                        v3acc.DELETED_AT = value.ValueKind != JsonValueKind.Null
                            ? value.GetDateTime()
                            : SmartParametersV2016.defaultDate;
                        break;

                    case "hold":
                        foreach (JsonProperty holdItem in value.EnumerateObject())
                        {
                            var hold = v3acc.HOLD;
                            switch (holdItem.Name)
                            {
                                case "value":
                                    hold.VALUE = holdItem.Value.GetRawText().Trim('"');
                                    break;
                                case "currency":
                                    hold.CURRENCY = holdItem.Value.GetString() ?? "";
                                    break;
                            }
                            v3acc.HOLD = hold;
                        }
                        break;

                    case "name":
                        v3acc.NAME = value.GetString() ?? "";
                        break;

                    case "platform":
                        v3acc.PLATFORM = value.GetString() ?? "";
                        break;

                    case "ready":
                        v3acc.READY = value.GetString() ?? "";
                        break;

                    case "retail_portfolio_id":
                        v3acc.RETAIL_PORTFOLIO_ID = value.GetString() ?? "";
                        break;

                    case "type":
                        v3acc.TYPE = value.GetString() ?? "";
                        break;

                    case "updated_at":
                        v3acc.UPDATED_AT = value.ValueKind != JsonValueKind.Null
                            ? value.GetDateTime()
                            : SmartParametersV2016.defaultDate;
                        break;

                    case "uuid":
                        v3acc.UUID = value.GetString() ?? "";
                        break;

                    default:
                        Console.Write(column);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(v3acc.UUID))
            {
                v3acc.USERNAME = Username;
                v3acc.EXCHANGE = Exchange;
                v3acc.UDPRN = Udprn;
                uuidList.Add(v3acc);
            }
        }
    
        
    internal static void AnalyzeAccountV3Old(
                                    string accountsData,
                                    List<SmartCrypto.V3Accounts> uuidList,
                                    string Username,
                                    string Exchange,
                                    string Udprn)
        {
            using JsonDocument jsonDoc = JsonDocument.Parse(accountsData);

            foreach (JsonProperty statement in jsonDoc.RootElement.EnumerateObject())
            {
                if (statement.Name == "accounts")
                {
                    foreach (JsonElement transactions in statement.Value.EnumerateArray())
                    {
                        foreach (JsonElement transaction in transactions.EnumerateArray())
                        {
                            SmartCrypto.V3Accounts v3acc = new SmartCrypto.V3Accounts();

                            foreach (JsonProperty individual in transaction.EnumerateObject())
                            {
                                string column = individual.Name;
                                JsonElement value = individual.Value;

                                switch (column)
                                {
                                    case "active":
                                        v3acc.ACTIVE = value.GetString() ?? "";
                                        break;

                                    case "available_balance":
                                        foreach (JsonProperty balanceItem in value.EnumerateObject())
                                        {
                                            var availbalance = v3acc.AVAILABLE_BALANCE;
                                            switch (balanceItem.Name)
                                            {
                                                case "value":
                                                    availbalance.VALUE = balanceItem.Value.GetRawText().Trim('"');
                                                    break;
                                                case "currency":
                                                    availbalance.CURRENCY = balanceItem.Value.GetString() ?? "";
                                                    break;
                                            }
                                            v3acc.AVAILABLE_BALANCE = availbalance;
                                        }
                                        break;

                                    case "created_at":
                                        v3acc.CREATED_AT = value.ValueKind != JsonValueKind.Null
                                            ? value.GetDateTime()
                                            : SmartParametersV2016.defaultDate;
                                        break;

                                    case "currency":
                                        v3acc.CURRENCY = value.GetString() ?? "";
                                        break;

                                    case "default":
                                        v3acc.DEFAULT = value.GetString() ?? "";
                                        break;

                                    case "deleted_at":
                                        v3acc.DELETED_AT = value.ValueKind != JsonValueKind.Null
                                            ? value.GetDateTime()
                                            : SmartParametersV2016.defaultDate;
                                        break;

                                    case "hold":
                                        foreach (JsonProperty holdItem in value.EnumerateObject())
                                        {
                                            var hold = v3acc.HOLD;
                                            switch (holdItem.Name)
                                            {
                                                case "value":
                                                    hold.VALUE = holdItem.Value.GetRawText().Trim('"');
                                                    break;
                                                case "currency":
                                                    hold.CURRENCY = holdItem.Value.GetString() ?? "";
                                                    break;
                                            }
                                            v3acc.HOLD = hold;
                                        }
                                        break;

                                    case "name":
                                        v3acc.NAME = value.GetString() ?? "";
                                        break;

                                    case "platform":
                                        v3acc.PLATFORM = value.GetString() ?? "";
                                        break;

                                    case "ready":
                                        v3acc.READY = value.GetString() ?? "";
                                        break;

                                    case "retail_portfolio_id":
                                        v3acc.RETAIL_PORTFOLIO_ID = value.GetString() ?? "";
                                        break;

                                    case "type":
                                        v3acc.TYPE = value.GetString() ?? "";
                                        break;

                                    case "updated_at":
                                        v3acc.UPDATED_AT = value.ValueKind != JsonValueKind.Null
                                            ? value.GetDateTime()
                                            : SmartParametersV2016.defaultDate;
                                        break;

                                    case "uuid":
                                        v3acc.UUID = value.GetString() ?? "";
                                        break;

                                    default:
                                        Console.Write(column);
                                        break;
                                }
                            }

                            if (!string.IsNullOrEmpty(v3acc.UUID))
                            {
                                v3acc.USERNAME = Username;
                                v3acc.EXCHANGE = Exchange;
                                v3acc.UDPRN = Udprn;
                                uuidList.Add(v3acc);
                            }
                        }
                    }
                }
            }
        }
        // Crypto ends
    }
}