//using Java.Util.Logging;
using System;
using System.Globalization;
using System.Reflection;

#if WINUI
using System.Collections.Generic;
#endif

namespace SmartCubeMobile
{
    public class SmartBouncer
    {
        internal static List<T> LookLoadSingle<T>(string tableName,
                                            string[] rays,
                                            MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel)
        {
            List<T> status = new List<T>();
            switch (tableName)
            {
                case "LOOKUP_CONDITIONS_PLANS":
                    status = (List<T>)Convert.ChangeType(SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.ConditionsPlansView>(ourviewmodel, rays, ""), typeof(List<T>), null);
                    break;
                case "LOOKUP_TARIFF_CODE":
                    // It doesn't matter if these have rolled over, because when we select them
                    // we never apply the TARIFF.WITHDRAWN_DATE > '2015/19/09' selection test
                    status = (List<T>)Convert.ChangeType(SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.TariffCodesNamesView>(ourviewmodel, rays, ""), typeof(List<T>), null);
                    break;
                case "LOOKUP_TARIFF_NAMES":
                    // It doesn't matter if these have rolled over, because when we select them
                    // we never apply the TARIFF.WITHDRAWN_DATE > '2015/19/09' selection test
                    status = (List<T>)Convert.ChangeType(SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.TariffCodesNamesView>(ourviewmodel, rays, ""), typeof(List<T>), null);
                    break;
                case "LOAD_UNIT_RATES":
                    // It DOES matter if these have rolled over, because when we selected the
                    // original set of TARIFFs we DID apply the TARIFF.WITHDRAWN_DATE > '2015/19/09' selection test
                    int ray_count = 0;
                    foreach (string ray in rays)
                    {
                        switch (ray_count)
                        {
                            case 0:
                                if (utilityviewmodel.withdrawn_date != SmartTimeV2016.ConvertDateTime(SmartParametersV2016.sensibleStartingDate))
                                {
                                    utilityviewmodel.withdrawn_date = SmartTimeV2016.ConvertDateTime(rays[0]);
                                }
                                break;
                            case 1:
                                //if (utilityviewmodel.unitrates_updatedx != SmartParametersV2016.defaultDate)
                                //{
                                // Try and find the latest UPDATED date
                                // Because they are sorted by Updated descending in procedure LOAD_UNIT_RATES
                                utilityviewmodel.unitrates_updatedx = SmartTimeV2016.ConvertDateTime(rays[1]);
                                //}
                                break;
                            case 2:
                                // Wow! Was this a pain in the arse or what??!?
                                // I have NO IDEA where this code disappeared to ..

                                // I have NO IDEA how this next VERY IMPORTANT statement
                                // went missing (!!) but here it is - back in!  Making
                                // sure we return more than ONE Unit Rate record!!!

                                // I have NO IDEA why this LOAD_UNIT_RATES gets called
                                // 2 or 3 times in WINFORMS - hence the test on ray.Length
                                // below.  One day - when I am old and grey - I will fix this
                                // But I suspect its WINFORM BOLLOCKS as usual
                                // It was - I had screwed up Consumers with lots of table errors!

                                string[] bobbins = rays[2].Split(SmartParametersV2016.unitSeparator);
                                status = (List<T>)Convert.ChangeType(SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.UnitRates>(ourviewmodel, bobbins, ""), typeof(List<T>), null);
                                break;
                            default:
                                break;
                        }
                        ray_count++;
                    }
                    break;
#if WINFORMS
                case "LOOKUP_UNIT_RATES":
                    // It DOES matter if these have rolled over, because when we selected the
                    // original set of TARIFFs we DID apply the TARIFF.WITHDRAWN_DATE > '2015/19/09' selection test
                    //var sqlite_row = new T();
                    //status = (List<T>)Convert.ChangeType(SmartPhyllV2020.I_Dont_Work_For_Foreigners_Single<SmartUtility.UnitRates>(sqlite_row, false, SmartParametersV2016.commachar));//(ref errorMessage, rays, ""), typeof(List<T>), null);//////
                    break;
#endif
                default:
                    break;
            }
            return status;
        }

        internal static void DataMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList)
        {
            switch (tableName)
            {
                case "BUTTONS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Buttons>(viewmodel, ray, multipleList);
                    break;
                case "CUBEFACES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Cubefaces>(viewmodel, ray, multipleList);
                    break;
                case "CULTURES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Cultures>(viewmodel, ray, multipleList);
                    break;
                case "CURRENCIES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Currencies>(viewmodel, ray, multipleList);
                    break;
                case "HANDLERS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Handlers>(viewmodel, ray, multipleList);
                    break;
                case "SCHEMAS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Schemas>(viewmodel, ray, multipleList);
                    break;
                case "SQLITESCHEMAS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.SQLiteSchemas>(viewmodel, ray, multipleList);
                    break;
                case "SQLITETABLES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.SQLiteTables>(viewmodel, ray, multipleList);
                    break;
                case "SQLITEFIELDS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.SQLiteFields>(viewmodel, ray, multipleList);
                    break;
                case "TOOLTIPS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartData.Tooltips>(viewmodel, ray, multipleList);
                    break;
                default:
                    break;
            }
            return;
        }
        internal static void MainMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList)
        {
            switch (tableName)
            {
                case "POSTCODES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUsers.InternalPostcodes>(viewmodel, ray, multipleList);
                    break;
                case "EXCHANGE_RATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUsers.ExchangeRates>(viewmodel, ray, multipleList);
                    break;
                case "EXTERNAL_RESOURCES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUsers.ExternalResources>(viewmodel, ray, multipleList);
                    break;
                case "VAT_RATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUsers.VatRates>(viewmodel, ray, multipleList);
                    //if (ray_count == 0)
                    //{
                    //    // This entire program needs at least ONE Vat Rate to work
                    //    // even though that may be 0.00%
                    //    return; // new List<object>();  // <= This is our way of 'crashing'
                    //}
                    break;
                default:
                    break;
            }
            return;
        }
        internal static void FinanceUpperCaseMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList)
        {
            switch (tableName)
            {
                case "BRAND_ACCOUNTS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.BrandAccounts>(viewmodel, ray, multipleList);
                    break;
                case "BRAND_AREAS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.BrandAreas>(viewmodel, ray, multipleList);
                    break;
                case "BRAND_CONNECTION":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.BrandConnection>(viewmodel, ray, multipleList);
                    break;
                case "BRAND_MATRIX":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.BrandMatrix>(viewmodel, ray, multipleList);
                    break;
                //case "BRANDORDINALS":
                //    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.BrandOrdinals>(viewmodel, ray, multipleList);
                //    break;
                case "BRANDS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Brands>(viewmodel, ray, multipleList);
                    break;
                case "CATEGORY_CODES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CategoryCodes>(viewmodel, ray, multipleList);
                    break;
                case "INSTITUTION_INFO":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.InstitutionInfo>(viewmodel, ray, multipleList);
                    break;
                case "INSTITUTIONS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Institutions>(viewmodel, ray, multipleList);
                    break;
                //case "OPEN_BANKING":
                //    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Banking>(viewmodel, ray, multipleList);
                //    break;
                case "TEMPLATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Templates>(viewmodel, ray, multipleList);
                    break;
                case "TRANSACTION_FLOWS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Transaction_Flows>(viewmodel, ray, multipleList);
                    break;
                case "TRANSACTION_GROUPS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Transaction_Groups>(viewmodel, ray, multipleList);
                    break;
                case "TRANSACTION_TYPES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.Transaction_Types>(viewmodel, ray, multipleList);
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void UtilityUpperCaseMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList)
        {
            switch (tableName)
            {
                //
                // Utilites Section
                //
                case "SINGLE_CONDITIONS_DATES":
                case "CONDITIONS_DATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ConditionsDates>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_CONDITIONS_GROUPS":
                case "CONDITIONS_GROUPS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ConditionsGroups>(viewmodel, ray, multipleList);
                    break;
                case "CONDITIONS_LIMITS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ConditionsLimits>(viewmodel, ray, multipleList);
                    break;
                case "CONDITIONS_AREAS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ConditionsAreas>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_CONDITIONS_PLANS":
                case "CONDITIONS_PLANS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ConditionsPlans>(viewmodel, ray, multipleList);
                    break;
                case "PAYMENT_METHODS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.PaymentMethods>(viewmodel, ray, multipleList);
                    break;
                case "PAYMENT_PLANS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.PaymentPlans>(viewmodel, ray, multipleList);
                    break;
                case "PRE_CONDITIONS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.PreConditions>(viewmodel, ray, multipleList);
                    break;
                case "POST_GROUPINGS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Groupings>(viewmodel, ray, multipleList);
                    break;
                case "POST_GROUPS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Groups>(viewmodel, ray, multipleList);
                    break;
                case "POST_CONDITIONS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Conditions>(viewmodel, ray, multipleList);
                    break;
                case "POST_LIMITS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Limits>(viewmodel, ray, multipleList);
                    break;
                case "POST_SELECT":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Select>(viewmodel, ray, multipleList);
                    break;
                case "POST_CODES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Post_Codes>(viewmodel, ray, multipleList);
                    break;
                case "SUPPLY_AREAS":
                    // MARKET_ID Never sent
                    // SUPPLIER  Never sent
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.SupplyAreas>(viewmodel, ray, multipleList);
                    break;
                case "SUPPLIERS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Suppliers>(viewmodel, ray, multipleList);
                    break;
                case "BRANDS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Brands>(viewmodel, ray, multipleList);
                    break;
                case "DISTRIBUTOR_INFO":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.DistributorInfo>(viewmodel, ray, multipleList);
                    break;
                case "SUPPLIER_TYPES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.SupplierTypes>(viewmodel, ray, multipleList);
                    break;
                case "SUPPLIER_INFO":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.SupplierInfo>(viewmodel, ray, multipleList);
                    break;
                case "BRAND_MATRIX":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.BrandMatrix>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_TARIFFS":
                case "TARIFFS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Tariffs>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_TARIFF_MATRIX":
                case "TARIFF_MATRIX":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.TariffMatrix>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_TARIFF_HISTORY":
                case "TARIFF_HISTORY":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.TariffHistory>(viewmodel, ray, multipleList);
                    break;
                case "TARIFF_PLANS":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.TariffPlans>(viewmodel, ray, multipleList);
                    break;
                case "RESOURCE_CODES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ResourceCodes>(viewmodel, ray, multipleList);
                    break;
                case "RESOURCE_TYPES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ResourceTypes>(viewmodel, ray, multipleList);
                    break;
                case "SINGLE_UNIT_RATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.UnitRates>(viewmodel, ray, multipleList);
                    break;
                case "UNIT_RATES":
                    // When we load SMARTUTILITY, the procedure 'UNIT_RATES' which is
                    // executed on the server is a special - it stores the 'withdrawn_date'
                    // which is generated by SmartDBServerV2020 based on today's date
                    // as the first and ONLY record.  This is
                    // because the Unit_Rates table is also AREA_CODE, RESOURCE_CODE and RESOURCE_TYPE
                    // specific and at 'load time' we simply don't know these things

                    // Now ... this failed because there was no Culture supplied
                    // as a FormatProvider so "05/01/2016" would succeed but
                    // "19/09/2016" would fail (becuase this fucking Convert shit
                    // assumes this to be mm/dd/yyyy and that is illegal whereas
                    // 05/01/2016 would be seen as 1st May 2016 and succeed)
                    // I have taken the try/catch out PROVIDED SmartDBServerV2023
                    // ALWAYS sends the date in dd/MM/yyyy format (SmartParametersV2016.standardFormat)
                    // and PROVIDED we always supply en-GB as a culture!!
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.WithdrawnDate>(viewmodel, ray, multipleList);

                    //withdrawn_date = (SmartTimeV2016.ConvertDateTime(ray[0]));
                    break;

                case "GAS_CONVERSION":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GasConversion>(viewmodel, ray, multipleList);
                    break;
                case "TEMPLATES":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.Templates>(viewmodel , ray, multipleList);
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void UsersMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList,
                                            string username)
        {
            switch (tableName)
            {
                case "Consumers":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUsers.ConsumersSQLite>(viewmodel, ray, multipleList, username);
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void ProfileMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList,
                                            string username)
        {
            switch (tableName)
            {
                case "Cubefaces":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartProfile.CubefacesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Profiles":       // Ignored
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartProfile.ProfilesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                //case "ProfilesCube":   // Ignored
                //    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartProfile.ProfilesCubeSQLite>(viewmodel, ray, multipleList, username);
                //    break;
                case "Groups":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartProfile.GroupsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "AddressesView":
                    // You are SO FUCKING CLEVER, Ray ... so FUCKING CLEVER
                    // You have balls the size of flying saucers my friend, you really do ..
                    //list_accounts_row.CLOSED = Convert.ToBoolean(items[3]); // Stop here =;-)
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartProfile.AddressesViewSQLite>(viewmodel, ray, multipleList, username);
                    break;
                default:
                    break;
            }
            return;
        }
        internal static void FinanceLowerCaseMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList,
                                            string username)
        {
            switch (tableName)
            {
                case "Accounts":
                    // You are SO FUCKING CLEVER, Ray ... so FUCKING CLEVER
                    // You have balls the size of flying saucers my friend, you really do ..
                    //list_accounts_row.CLOSED = Convert.ToBoolean(items[3]); // Stop here =;-)
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.AccountsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoAccounts":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoAccountsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoAddresses":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoAddressesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoCurrencyRates":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoCurrencyRatesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoTransactions":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoTransactionsViewSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoLedgers":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoLedgersSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoWallets":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoWalletsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CryptoWalletTotals":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CryptoWalletTotalsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Categories":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CategoriesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "CategoryTypes":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.CategoryTypesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Connections":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.ConnectionsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Logins":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.LoginsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Switches":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.SwitchesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Transactions":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.TransactionsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "TransactionsCategories":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartFinance.TransactionsCategoriesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                default:
                    break;
            }
            return;
        }

        internal static void UtilityLowerCaseMultiple(object viewmodel,
                                            string tableName,
                                            string[] ray,
                                            List<object> multipleList,
                                            string username)
        {
            switch (tableName)
            {
                case "Accounts":
                    // You are SO FUCKING CLEVER, Ray ... so FUCKING CLEVER
                    // You have balls the size of flying saucers my friend, you really do ..
                    //list_accounts_row.CLOSED = Convert.ToBoolean(items[3]); // Stop here =;-)
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.AccountsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "AccChargesCredits":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.AccChargesCreditsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "BankDetails":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.BankDetailsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Bills":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.BillsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "BillsResource":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.BillsResourceSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "ECosts":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ECostsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "EDiscounts":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.EDiscountsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "EReadings":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.EReadingsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "EStandingCharges":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.EStandingChargesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "EUnitCharges":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.EUnitChargesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "EUsage":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.EUsageSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GCosts":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GCostsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GDiscounts":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GDiscountsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GReadings":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GReadingsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GStandingCharges":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GStandingChargesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GUnitCharges":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GUnitChargesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "GUsage":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.GUsageSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Logins":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.LoginsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Meters":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.MetersSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Payments":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.PaymentsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Resources":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ResourcesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "ResourcesTypes":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.ResourcesTypesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "SupChargesCredits":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.SupChargesCreditsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Switches":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.SwitchesSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "TariffDetails":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.TariffDetailsSQLite>(viewmodel, ray, multipleList, username);
                    break;
                case "Unallocated":
                    SmartPhyllV2020.BuildSQLiteRecordsMultiple<SmartUtility.UnallocatedSQLite>(viewmodel    , ray, multipleList, username);
                    break;
                default:
                    break;
            }
            return;
        }
    }
}