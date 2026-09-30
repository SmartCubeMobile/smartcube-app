using System.Globalization;

#if WINFORMS
using MoreLinq;
using System.Windows;
#endif

#if WPF
using MoreLinq;
using System.Windows;
using static SmartCubeMobile.SmartFinance;
#endif

#if WINUI
using MoreLinq;         // <= THANK FUCK FOR THIS -- I have had **MAJOR BUGS*** in my code!!!!
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.UI.Xaml;
#endif

#if ANDROIDX
using System.Runtime.InteropServices.JavaScript;
using static System.Linq.Queryable;
using static System.Linq.Enumerable;
using MoreLinq;
using Android.Views;
#endif

#if SMARTMAUI

#endif

namespace SmartCubeMobile
{
    internal class SmartSpikeFinanceV2017
    {
#if ANDROIDX
        internal static string[] GetBrandsFromConnections(List<SmartFinance.Connections> items,
                                short institutionCode)
        {
            return items
            .Where(b => b.INSTITUTION_CODE == institutionCode)
            .Select(b => b.BrandName)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();
        }
        //internal List<BrandItem> GetBrandsWithAvailableMethods(FinanceViewModel financeviewmodel,
        //                        short institutionCode)
        //{
        //    return financeviewmodel.CreatedBrands
        //        .Where(b =>
        //            b.InstitutionCode == institutionCode &&
        //            ConnectionsGetAvailableMethods(financeviewmodel, institutionCode, b.BRAND_CODE).Any())
        //        .Select(b => new BrandItem
        //        {
        //            InstitutionCode = b.InstitutionCode,
        //            BRAND_CODE = b.BRAND_CODE,
        //            BrandName = b.BrandName
        //        })
        //        .OrderBy(b => b.BrandName)
        //        .ToList();
        //}

        internal List<int> ConnectionsGetAvailableMethods(FinanceViewModel financeviewmodel,
                                                        short institutionCode,
                                                        short brandCode)
        {
            var allMethods = financeviewmodel.PLO.brand_connectionList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    c.BRAND_CODE == brandCode)
                .Select(c => c.LOGIN_METHOD)
                .Distinct();
            var usedMethods = financeviewmodel.ConnectionsViewList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    c.BRAND_CODE == brandCode &&
                    !c.IsPendingCompletion &&              // 🔥 ONLY COMMITTED
                    c.LOGIN_METHOD != 0)
                .Select(c => c.LOGIN_METHOD)
                .Distinct();
            return allMethods
                .Except(usedMethods)
                .OrderBy(m => m)
                .ToList();
        }


        //internal List<int> ConnectionsGetAvailableMethodsNew(FinanceViewModel financeviewmodel,
        //                                                short institutionCode,
        //                                                short brandCode)
        //{
        //    foreach (SmartFinance.BrandConnection bc in financeviewmodel.PLO.brand_connectionList)
        //    {
        //        foreach (SmartFinance.Connections fc in financeviewmodel.ConnectionsViewList)
        //        {
        //            if (bc.INSTITUTION_CODE == fc.INSTITUTION_CODE &&
        //                bc.BRAND_CODE == fc.BRAND_CODE &&
        //                bc.LOGIN_METHOD == fc.LOGIN_METHOD)
        //            {
        //                // Well we match
        //            }
        //        }


        //    }
        //    List<SmartFinance.BrandConnection> allMethods =
        //        new List<SmartFinance.BrandConnection>(
        //            from Connection in financeviewmodel.PLO.brand_connectionList
        //            where Connection.INSTITUTION_CODE == institutionCode
        //            select Connection).DistinctBy(key => new
        //            {
        //                key.CUBEFACE_CODE,
        //                key.INSTITUTION_CODE,
        //                key.BRAND_CODE,
        //                key.LOGIN_METHOD
        //            }).ToList();

        //    List<SmartFinance.Connections> usedMethods =
        //        new List<SmartFinance.Connections>(
        //            from Connection in financeviewmodel.ConnectionsViewList
        //            where Connection.INSTITUTION_CODE == institutionCode &&
        //            Connection.LOGIN_METHOD > 0 &&
        //            Connection.IsPendingCompletion == false // 🔥 ONLY COMMITTED
        //            select Connection).DistinctBy(key => new
        //            {
        //                key.CUBEFACE_CODE,
        //                key.INSTITUTION_CODE,
        //                key.BRAND_CODE,
        //                key.LOGIN_METHOD
        //            }).ToList();


        //    List<(short cube, short inst, short brand, int login)> usedKeys =
        //       new List<(short cube, short inst, short brand, int login)>(from
        //                 UM in usedMethods select UM.USERNAME).ToList();
            
                                                 
                                                    

        //}
#endif

#if WINFORMS
        internal static List<SmartUsers.Consumers> Finance_Configure_Consumers(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)
        {
            List<SmartUsers.Consumers> consumers_found =
                new List<SmartUsers.Consumers>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 where Consumer.Include &&  // Much easier, wouldn't you agree Ray??!?                       
                       Cubeface.FACE_ACTIVE &&
                        Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
                 select Consumer);
            return consumers_found;
        }

        internal static List<SmartProfile.Cubefaces> Finance_Configure_Cubefaces(MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                new List<SmartProfile.Cubefaces>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 where Consumer.Include &&
                        Cubeface.FACE_ACTIVE &&
                        Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
                 select Cubeface);
            return cubefaces_found;
        }

        internal static List<SmartFinance.Accounts> Finance_Configure_Accounts(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Accounts> accounts_found =
                new List<SmartFinance.Accounts>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 join Category in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 join CategoryTypes in financeviewmodel.PLO.finance_categorytypesList
                 on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 equals new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE }
                 join Account in financeviewmodel.PLO.finance_accountsList
                 on new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE, CategoryTypes.CURRENCY_ORDINAL }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
                 where Consumer.Include &&
                        Cubeface.FACE_ACTIVE &&
                        Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         Category.CATEGORY_CODE == financeviewmodel.category_code
                 orderby Account.ACCOUNT_CREATED descending    // *not* a Key? It is now!!
                 select Account);
            return accounts_found;
        }

        internal static List<SmartFinance.Transactions> Finance_Configure_Transactions(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel)
        {
            //
            // This needs re-doing!!!!!!!!!! It certainly fucking does ...
            List<SmartFinance.Accounts> accounts_found = Finance_Configure_Accounts(ourviewmodel,
                                                                                    financeviewmodel);

            accounts_found = new List<SmartFinance.Accounts>
                (accounts_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.CUBEFACE_CODE,
                    key.INSTITUTION_CODE,
                    key.BRAND_CODE,
                    key.ACCOUNT_CREATED,
                    key.CATEGORY_CODE,
                    key.SORTCODE,
                    key.ACCOUNT_NO
                }));
            List<SmartFinance.Transactions> transactions_found =
                    new List<SmartFinance.Transactions>();
            if (accounts_found.Count > 0)
            {
                // Need to put the Address UDPRN matching Account.UDPRN 
                transactions_found =
                    new List<SmartFinance.Transactions>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                     on new { Consumer.USERNAME }
                     equals new { Cubeface.USERNAME }
                     join Category in financeviewmodel.PLO.finance_categoriesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                     join Account in accounts_found
                     on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE }
                     join Transactions in financeviewmodel.PLO.transactionsList
                     on new
                     {
                         Account.USERNAME,
                         Account.CUBEFACE_CODE,
                         Account.INSTITUTION_CODE,
                         Account.BRAND_CODE,
                         Account.SORTCODE,
                         Account.ACCOUNT_NO
                     }
                     equals new
                     {
                         // Transactions doesn't have a CATEGORY_CODE
                         Transactions.USERNAME,
                         Transactions.CUBEFACE_CODE,
                         Transactions.INSTITUTION_CODE,
                         Transactions.BRAND_CODE,
                         Transactions.SORTCODE,
                         Transactions.ACCOUNT_NO
                     }
                     
                     orderby Transactions.USERNAME ascending,
                            Transactions.SORTCODE ascending,
                             Transactions.ACCOUNT_NO ascending,
                             Transactions.STATEMENT_DATE ascending,
                             Transactions.STATEMENT_NO ascending
                             
                     where Consumer.Include &&
                            Cubeface.FACE_ACTIVE &&
                            Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                            Category.CATEGORY_CODE == financeviewmodel.category_code
                     select Transactions);
            }
            return transactions_found;
        }

        internal static List<SmartFinance.TransactionsCategories> Finance_Configure_BankTransactions(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel)
        {
            //
            // This needs re-doing!!!!!!!!!! It certainly fucking does ...
            List<SmartFinance.Accounts> accounts_found = Finance_Configure_Accounts(ourviewmodel,
                                                                                    financeviewmodel);

            accounts_found = new List<SmartFinance.Accounts>
                (accounts_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.CUBEFACE_CODE,
                    key.INSTITUTION_CODE,
                    key.BRAND_CODE,
                    key.ACCOUNT_CREATED,
                    key.CATEGORY_CODE,
                    key.SORTCODE,
                    key.ACCOUNT_NO
                }));


            List<SmartFinance.TransactionsCategories> transactionscategories_found =
                    new List<SmartFinance.TransactionsCategories>();


            if (accounts_found.Count > 0)
            {
                // Need to put the Address UDPRN matching Account.UDPRN 
                transactionscategories_found =
                    new List<SmartFinance.TransactionsCategories>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                     on new { Consumer.USERNAME }
                     equals new { Cubeface.USERNAME }
                     join Category in financeviewmodel.PLO.finance_categoriesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                     join Account in accounts_found
                     on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE }
                     join Transactions in financeviewmodel.PLO.transactionsList
                     on new
                     {
                         Account.USERNAME,
                         Account.CUBEFACE_CODE,
                         Account.INSTITUTION_CODE,
                         Account.BRAND_CODE,
                         Account.SORTCODE,
                         Account.ACCOUNT_NO
                     }
                     equals new
                     {
                         // Transactions doesn't have a CATEGORY_CODE
                         Transactions.USERNAME,
                         Transactions.CUBEFACE_CODE,
                         Transactions.INSTITUTION_CODE,
                         Transactions.BRAND_CODE,
                         Transactions.SORTCODE,
                         Transactions.ACCOUNT_NO
                     }
                     join TransactionsCategories in financeviewmodel.PLO.transactionscategoriesList
                     on new
                     {
                         Transactions.USERNAME,
                         Transactions.CUBEFACE_CODE,
                         Transactions.INSTITUTION_CODE,
                         Transactions.BRAND_CODE,
                         Transactions.STATEMENT_DATE,
                         Transactions.STATEMENT_NO,
                         Transactions.SORTCODE,
                         Transactions.ACCOUNT_NO
                     }
                     equals new
                     {
                         TransactionsCategories.USERNAME,
                         TransactionsCategories.CUBEFACE_CODE,
                         TransactionsCategories.INSTITUTION_CODE,
                         TransactionsCategories.BRAND_CODE,
                         TransactionsCategories.STATEMENT_DATE,
                         TransactionsCategories.STATEMENT_NO,
                         TransactionsCategories.SORTCODE,
                         TransactionsCategories.ACCOUNT_NO
                     }
                     orderby TransactionsCategories.USERNAME ascending,
                             TransactionsCategories.SORTCODE ascending,
                             TransactionsCategories.ACCOUNT_NO ascending,
                             TransactionsCategories.TRANSACTION_DATE ascending,
                             TransactionsCategories.SEQUENCE_NO ascending
                     where Consumer.Include &&
                            Cubeface.FACE_ACTIVE &&
                            Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                            Category.CATEGORY_CODE == financeviewmodel.category_code
                     select TransactionsCategories);
            }            
            return transactionscategories_found;
        }

        internal static List<SmartFinance.Categories> Finance_Configure_Categories(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Categories> categories_found =
                new List<SmartFinance.Categories>();
            if (financeviewmodel.category_code == SmartParametersV2016.defaultChar)
            {
                categories_found = new List<SmartFinance.Categories>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 join Categories in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                 where Consumer.Include &&
                        Cubeface.FACE_ACTIVE &&
                        Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
                 select Categories);
            }
            else
            {
                categories_found = new List<SmartFinance.Categories>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 join Categories in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                 where Consumer.Include &&
                        Cubeface.FACE_ACTIVE &&
                        Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         Categories.CATEGORY_CODE == financeviewmodel.category_code
                 select Categories);
            }
            return categories_found;
        }

        internal static List<SmartFinance.CategoryTypes> Finance_Configure_CategoryTypes(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.CategoryTypes> categorytypes_found = new List<SmartFinance.CategoryTypes>();
            if (financeviewmodel.category_code == SmartParametersV2016.defaultChar)
            {
                categorytypes_found = new List<SmartFinance.CategoryTypes>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 join Categories in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                 join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                 on new { Categories.USERNAME, Categories.CUBEFACE_CODE, Categories.CATEGORY_CODE }
                 equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                 where Consumer.Include &&
                         Cubeface.FACE_ACTIVE &&
                         Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
                 select CategoryType);
            }
            else
            {
                categorytypes_found = new List<SmartFinance.CategoryTypes>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 join Categories in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                 join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                 on new { Categories.USERNAME, Categories.CUBEFACE_CODE, Categories.CATEGORY_CODE }
                 equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                 where Consumer.Include &&
                         Cubeface.FACE_ACTIVE &&
                         Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         CategoryType.CATEGORY_CODE == financeviewmodel.category_code
                 select CategoryType);
            }
            return categorytypes_found;
        }

        internal static List<SmartFinance.Connections> Finance_Configure_Connections(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Connections> connections_found =
                                new List<SmartFinance.Connections>
            (from Consumer in ourviewmodel.Hamas.consumersList
             join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
             on new { Consumer.USERNAME }
             equals new { Cubeface.USERNAME }
             join Connection in financeviewmodel.PLO.finance_connectionsList
             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
             equals new { Connection.USERNAME, Connection.CUBEFACE_CODE }
             where Consumer.Include &&
                    Cubeface.FACE_ACTIVE &&
                    Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
             select Connection);
            return connections_found;
        }
        internal static List<SmartFinance.Logins> Finance_Configure_Logins(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Logins> logins_found =
                                new List<SmartFinance.Logins>
            (from Consumer in ourviewmodel.Hamas.consumersList
             join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
             on new { Consumer.USERNAME }
             equals new { Cubeface.USERNAME }
             join Login in financeviewmodel.PLO.finance_loginsList
             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
             equals new { Login.USERNAME, Login.CUBEFACE_CODE }
             where Consumer.Include &&
                    Cubeface.FACE_ACTIVE &&
                    Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code
             select Login);
            return logins_found;
        }
#endif


#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI || CRYPTO
        internal static string GetCultureView(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string ConvertFromSymbol,
                                            string ConvertToSymbol,
                                            decimal value)
        {
            if (financeviewmodel.CurrencyOrdinal == 0)
            {
                ConvertToSymbol = ConvertFromSymbol;
            }
            NumberFormatInfo nfi;
            List<SmartData.CultureView> cviews_found =
             new List<SmartData.CultureView>(from cview in ourviewmodel.cultureviewList
                                             where cview.ISOCURRENCYSYMBOL == ConvertToSymbol
                                             select cview);
            if (cviews_found.Count > 0)
            {
                nfi = financeviewmodel.CultureINF.NumberFormat.Clone() as NumberFormatInfo;
                nfi.CurrencySymbol = cviews_found.First().SYMBOL;
                return (value).ToString("c", nfi);
            }
            return (value).ToString();
        }

        internal static short GetCurrencyOrdinal(MainViewModel ourviewmodel,
                                                    string ConvertToSymbol)
        {
            short ordinal = 0;
            if (!string.IsNullOrEmpty(ConvertToSymbol))
            {
                List<SmartData.CultureView> cultureviews_found =
                new List<SmartData.CultureView>(from Culture in
                                                ourviewmodel.cultureviewList
                                                where Culture.SYMBOL == ConvertToSymbol
                                                select Culture);
                if (cultureviews_found.Count > 0)
                {
                    ordinal = cultureviews_found.First().CURRENCY_ORDINAL;
                }
            }
            return ordinal;
        }

        
#endif
        // All my Ball Breaking stuff starts here
        internal static List<SmartFinance.InstitutionInfo> Finance_Lookup_INSTITUTION_INFOS(List<SmartFinance.Institutions> INSTITUTIONSList,
                                                    List<SmartFinance.InstitutionInfo> INSTITUTIONINFOList)
        {
            List<SmartFinance.InstitutionInfo> institutions_found =
                new List<SmartFinance.InstitutionInfo>
                (from InstitutionInfo in INSTITUTIONINFOList
                 join Institution in INSTITUTIONSList
                 on new { InstitutionInfo.CUBEFACE_CODE, InstitutionInfo.INSTITUTION_CODE }
                 equals new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 where //Institution.INSTITUTION_CODE == institution_code &&
                        Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                 select InstitutionInfo).ToList();
            return institutions_found.Distinct().ToList();
        }

        //internal static string Finance_Lookup_INSTITUTION_NAME(List<SmartFinance.InstitutionInfo> INSTITUTIONINFOList,
        //                        short InstitutionCode)
        //{
        //    SmartFinance.InstitutionInfo institutions_found =
        //        //new SmartFinance.InstitutionInfo
        //        (from InstitutionInfo in INSTITUTIONINFOList
        //         where InstitutionInfo.INSTITUTION_CODE == InstitutionCode
        //                select InstitutionInfo);
        //    return institutions_found.INSTITUTION_NAME;
        //}

#if WPF  || WINUI || SMARTMAUI
        internal static Visibility Finance_Lookup_Category(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandAccounts> BrandAccountsList,
                                                    short Institution_Code,
                                                    short Brand_Code,
                                                    char CategoryCode)
#endif
#if ANDROIDX
        internal static ViewStates Finance_Lookup_Category(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandAccounts> BrandAccountsList,
                                                    short Institution_Code,
                                                    short Brand_Code,
                                                    char CategoryCode)
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            List<SmartFinance.BrandAccounts> brandAccounts_found =
                new List<SmartFinance.BrandAccounts>
                (from Institution in InstitutionsList
                 join Brand in BrandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 join BrandAccount in BrandAccountsList
                 on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
                 equals new { BrandAccount.CUBEFACE_CODE, BrandAccount.INSTITUTION_CODE, BrandAccount.BRAND_CODE }
                 where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Institution.INSTITUTION_CODE == Institution_Code &&
                        Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Brand.BRAND_CODE == Brand_Code &&
                        BrandAccount.CATEGORY_CODE == CategoryCode
                 select BrandAccount).ToList();
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (brandAccounts_found.Count > 0)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;    // So it keeps the spacing
#endif
#if ANDROIDX
            if (brandAccounts_found.Count > 0)
            {
                return ViewStates.Visible;
            }

            return ViewStates.Invisible;    // So it keeps the spacing
#endif

        }
#endif

#if WPF  || WINUI || SMARTMAUI
        internal static Visibility FinanceLookupParameterVisibility(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
                                                    short InstitutionCode,
                                                    short BrandCode,
                                                    int LoginMethod,
                                                    short Ordinal)
#endif
#if ANDROIDX
        internal static ViewStates FinanceLookupParameterVisibility(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
                                                    short InstitutionCode,
                                                    short BrandCode,
                                                    short LoginMethod,
                                                    short Ordinal)
#endif
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
        {
            List<SmartFinance.BrandConnection> brandConnection_found =
                new List<SmartFinance.BrandConnection>
                (from Institution in InstitutionsList
                 join Brands in BrandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brands.CUBEFACE_CODE, Brands.INSTITUTION_CODE }
                 join BrandConnection in BrandConnectionList
                 on new { Brands.CUBEFACE_CODE, Brands.INSTITUTION_CODE, Brands.BRAND_CODE }
                 equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE, BrandConnection.BRAND_CODE }
                 where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Institution.INSTITUTION_CODE == InstitutionCode &&
                        Brands.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Brands.BRAND_CODE == BrandCode &&
                        BrandConnection.LOGIN_METHOD == LoginMethod &&
                        BrandConnection.ORDINAL == Ordinal
                 select BrandConnection).ToList();
#if WPF  || WINUI || SMARTMAUI
            if (brandConnection_found.Count > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;    // So it keeps the spacing
#endif
#if ANDROIDX
            if (brandConnection_found.Count > 0)
            {
                return ViewStates.Visible;
            }
            return ViewStates.Invisible;    // So it keeps the spacing
#endif
        }
#endif
        //#if WINFORMS || WPF  || WINUI
        //        internal static Visibility FinanceLookupParameterVisibilityOld(List<SmartFinance.Institutions> InstitutionsList,
        //                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
        //                                                    short InstitutionCode,
        //                                                    short Ordinal)
        //#endif
        //#if ANDROIDX
        //        internal static ViewStates FinanceLookupParameterVisibility(List<SmartFinance.Institutions> InstitutionsList,
        //                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
        //                                                    short InstitutionCode,
        //                                                    short Ordinal)
        //#endif
        //        {
        //            List<SmartFinance.BrandConnection> brandConnection_found =
        //                new List<SmartFinance.BrandConnection>
        //                (from Institution in InstitutionsList
        //                 join BrandConnection in BrandConnectionList
        //                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
        //                 equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE }
        //                 where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
        //                        Institution.INSTITUTION_CODE == Institution_Code &&
        //                        BrandConnection.ORDINAL == Ordinal
        //                 select BrandConnection).ToList();
        //#if WINFORMS || WPF  || WINUI
        //            if (brandConnection_found.Count > 0)
        //            {
        //                return Visibility.Visible;
        //            }
        //            return Visibility.Collapsed;    // So it keeps the spacing
        //#endif
        //#if ANDROIDX
        //            if (brandConnection_found.Count > 0)
        //            {
        //                return ViewStates.Visible;
        //            }
        //            return ViewStates.Invisible;    // So it keeps the spacing
        //#endif
        //        }

        internal static bool FinanceLookupLoginMethodValidity(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
                                                    short InstitutionCode,
                                                    short BrandCode,
                                                    int Method)
        {
            List<SmartFinance.BrandConnection> brandConnection_found =
                new List<SmartFinance.BrandConnection>
                (from Institution in InstitutionsList
                 join Brand in BrandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 join BrandConnection in BrandConnectionList
                 on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
                 equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE, BrandConnection.BRAND_CODE }
                 where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Institution.INSTITUTION_CODE == InstitutionCode &&
                        Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Brand.BRAND_CODE == BrandCode &&
                        BrandConnection.LOGIN_METHOD == Method
                 select BrandConnection).ToList();
            if (brandConnection_found.Count > 0)
            {
                return true;    // Its available
            }
            return false;
        }
        internal static bool FinanceLookupParameterValidity(List<SmartFinance.Institutions> InstitutionsList,
                                                    List<SmartFinance.Brands> BrandsList,
                                                    List<SmartFinance.BrandConnection> BrandConnectionList,
                                                    short InstitutionCode,
                                                    short BrandCode,
                                                    int Method,
                                                    short Ordinal,
                                                    string text)
        {
            List<SmartFinance.BrandConnection> brandConnection_found =
                new List<SmartFinance.BrandConnection>
                (from Institution in InstitutionsList
                 join Brand in BrandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 join BrandConnection in BrandConnectionList
                 on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
                 equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE, BrandConnection.BRAND_CODE }
                 where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Institution.INSTITUTION_CODE == InstitutionCode &&
                        Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                        Brand.BRAND_CODE == BrandCode &&
                        BrandConnection.LOGIN_METHOD == Method &&
                        BrandConnection.ORDINAL == Ordinal
                 select BrandConnection).ToList();
            if (brandConnection_found.Count > 0)
            {
                int length = brandConnection_found.First().PARAMETER_LENGTH;
                if (text.Length > length)
                {
                    return false;
                }
                // Check for None, AlphaNumeric, Alpha or Numeric
                string parameterType = brandConnection_found.First().PARAMETER_TYPE;
                if (!string.IsNullOrEmpty(parameterType))
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return false;
                    }
                }
                else
                {
                    // String doesn't have a type, it can even be empty ...
                    return true;
                }
                switch (parameterType)
                {
                    case "AN":
                        // Check if all characters are alphanumeric (letters or digits only)                        
                        return text.All(char.IsLetterOrDigit);
                    case "A":
                        // Check if all characters are alpha (letters only)                        
                        return text.All(char.IsLetter);
                    case "N":
                        // Check if all characters are numeric (digits only)
                        return text.All(char.IsDigit);
                    default:
                        break;
                }
            }
            return false;
        }

        internal static bool Finance_Lookup_FinanceLoginsOurs(List<SmartFinance.Logins> LoginsList,
                                                    SmartFinance.Logins newLogin)
        {
            List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>
                (from Login in LoginsList
                 where Login.CUBEFACE_CODE == newLogin.CUBEFACE_CODE &&
                        Login.INSTITUTION_CODE == newLogin.INSTITUTION_CODE &&
                        Login.BRAND_CODE == newLogin.BRAND_CODE &&
                        Login.LOGIN_METHOD == newLogin.LOGIN_METHOD &&
                        Login.ACTIVE_FLAG == newLogin.ACTIVE_FLAG
                 select Login).ToList();
            if (logins_found.Count > 0)
            {
                // Cannot have any matching Categories IF the
                // Institution and Brand codes are the same
                foreach (SmartFinance.Logins log in logins_found)
                {
                    if (log.BANKSCHECKED)
                    {
                        if (newLogin.BANKSCHECKED)
                        {
                            return true;
                        }
                    }
                    if (log.SAVINGSCHECKED)
                    {
                        if (newLogin.SAVINGSCHECKED)
                        {
                            return true;
                        }
                    }
                    if (log.INVESTMENTSCHECKED)
                    {
                        if (newLogin.INVESTMENTSCHECKED) 
                        {
                            return true;
                        }
                    }
                    if (log.CRYPTOSCHECKED)
                    {
                        if (newLogin.CRYPTOSCHECKED)
                        {
                            return true;
                        }
                    }
                    if (!log.BANKSCHECKED &&
                        !newLogin.BANKSCHECKED &&
                        !log.SAVINGSCHECKED &&
                        !newLogin.SAVINGSCHECKED &&
                        !log.INVESTMENTSCHECKED &&
                        !newLogin.INVESTMENTSCHECKED &&
                        !log.CRYPTOSCHECKED &&
                        !newLogin.CRYPTOSCHECKED)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        internal static bool Finance_Lookup_FinanceLogins(List<SmartFinance.Logins> LoginsList,
                                                    SmartFinance.Logins newLogin)
        {
            List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>
                (from Login in LoginsList
                 where Login.CUBEFACE_CODE == newLogin.CUBEFACE_CODE &&
                        Login.INSTITUTION_CODE == newLogin.INSTITUTION_CODE &&
                        Login.BRAND_CODE == newLogin.BRAND_CODE &&
                        Login.LOGIN_METHOD == newLogin.LOGIN_METHOD &&
                        Login.ACTIVE_FLAG == newLogin.ACTIVE_FLAG
                 select Login).ToList();
            if (logins_found.Count > 0)
            {
                // Cannot have any matching Categories IF the
                // Institution and Brand codes are the same
                foreach (SmartFinance.Logins log in logins_found)
                {
                    if (log.BANKSCHECKED)
                    {
                        if (newLogin.BANKSCHECKED)
                        {
                            return false;
                        }
                    }
                    if (log.SAVINGSCHECKED)
                    {
                        if (newLogin.SAVINGSCHECKED)
                        {
                            return false;
                        }
                    }
                    if (log.INVESTMENTSCHECKED)
                    {
                        if (newLogin.INVESTMENTSCHECKED)
                        {
                            return false;
                        }
                    }
                    if (!log.BANKSCHECKED &&
                        !newLogin.BANKSCHECKED &&
                        !log.SAVINGSCHECKED && 
                        !newLogin.SAVINGSCHECKED &&
                        !log.INVESTMENTSCHECKED &&
                        !newLogin.INVESTMENTSCHECKED)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static SmartFinance.Logins Finance_Lookup_Something(FinanceViewModel financeviewmodel,
                                                                    List<SmartFinance.Logins> LoginsList)
        {
            List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>
                (from Login in LoginsList
                 where //Login.CUBEFACE_CODE == financeviewmodel.SelectedLogin.CUBEFACE_CODE &&
                        Login.INSTITUTION_CODE == financeviewmodel.SelectedLogin.INSTITUTION_CODE &&
                        Login.BRAND_CODE == financeviewmodel.SelectedLogin.BRAND_CODE &&
                        Login.LOGIN_METHOD== financeviewmodel.SelectedLogin.LOGIN_METHOD
                 select Login).ToList();
            if (logins_found.Count > 0)
            {
                return logins_found.First();
            }
            return null;
        }

        internal static SmartFinance.Connections Finance_Lookup_Connection(FinanceViewModel financeviewmodel,
                                                                    List<SmartFinance.Connections> ConnectionsList,
                                                                    char CubefaceCode,
                                                                    short InstitutionCode,
                                                                    short BrandCode,
                                                                    int LoginMethod)
        {
            List<SmartFinance.Connections> connections_found =
               new List<SmartFinance.Connections>();
            if (InstitutionCode != 0 &&
                BrandCode != 0 &&
                LoginMethod != 0)
            {
                connections_found =
                new List<SmartFinance.Connections>
                (from Connection in ConnectionsList
                 where Connection.CUBEFACE_CODE == CubefaceCode &&
                       Connection.INSTITUTION_CODE == InstitutionCode &&
                       Connection.BRAND_CODE == BrandCode &&
                       Connection.LOGIN_METHOD == LoginMethod
                 select Connection).ToList();
                if (connections_found.Count > 0)
                {
                    return connections_found.First();
                }
            }
            else
            {
                connections_found =
                new List<SmartFinance.Connections>
                (from Connection in ConnectionsList
                 where Connection.CUBEFACE_CODE == CubefaceCode
                 select Connection).ToList();
                if (connections_found.Count > 0)
                {
                    return connections_found.First();
                }
            }
            return new SmartFinance.Connections();
        }

        internal static string FinanceFindParameter1(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            short InstitutionCode,
                                            short BrandCode,
                                            int LoginMethod)
        {
            string Parameter1 = "";
            List<SmartFinance.Connections> conns_found =
                                        new List<SmartFinance.Connections>
            (from Consumer in ourviewmodel.Hamas.consumersList
                join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                on new { Consumer.USERNAME }
                equals new { Cubeface.USERNAME }
                join Connection in financeviewmodel.ConnectionsViewList
                on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                equals new { Connection.USERNAME, Connection.CUBEFACE_CODE }
                where Consumer.Include &&
                    Cubeface.FACE_ACTIVE &&
                    Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                    Connection.INSTITUTION_CODE == InstitutionCode &&
                    Connection.BRAND_CODE == BrandCode &&
                    Connection.LOGIN_METHOD == LoginMethod
                select Connection).ToList();
            if (conns_found.Count > 0)
            {
                Parameter1 = conns_found.First().Parameter1;
            }
            return Parameter1;
        }
        // Ends here

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        internal static short LookupCurrencyOrdinal(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string SortCode,
                                                string AccountNo)
        {
            short currencyOrdinal = 0;
            //string accountCurrency = "";
            List<SmartFinance.Accounts> accountsFound = new List<SmartFinance.Accounts>
                (from Account in financeviewmodel.PLO.finance_accountsList
                 where Account.SORTCODE == SortCode &&
                 Account.ACCOUNT_NO == AccountNo
                 select Account);
            if (accountsFound.Count > 0)
            {
                currencyOrdinal = accountsFound.First().CURRENCY_ORDINAL;
                //accountCurrency = SmartSpikeV2017.Lookup_Currency_Symbol(ourviewmodel.currenciesList,
                //                                                         accountsFound.First().CURRENCY_ORDINAL);
            }
            return currencyOrdinal;
        }
#endif

        // All this just to get the Next Finance COnnection date ...
        internal static DateTime FinanceNextConnection(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found = SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code,
                                                                        ourviewmodel.UserName);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().NEXT_CONNECTION;
            }
            return SmartParametersV2016.defaultDate;
        }

        //    // Its different for Finance.  Whereas with Utility you ALWAYS know the address
        //    // ('cos a meter has to be located physically SOMEWHERE) that's not the case
        //    // with Finance, 'cos you might not know the address until you decode a statement
        //    // and you might not always have a statement to decode ....
        internal static List<SmartProfile.AddressesView> Finance_Configure_Addresses(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
            // There IS A JOIN to Cubefaces here, because addresses
            // HERE are limited to the Finance Cubeface - in fact you WANT
            // addresses to be visible across ALL Cubefaces so that you
            // can link Finance AND Utility AND Insurance et. al.
            //
            // However HERE we just want the Addresses for which our
            // Accounts apply to (assuming of course that we can always
            // stick a UDPRN in!)
            List<SmartProfile.AddressesView> addresses_found = new List<SmartProfile.AddressesView>();

            // Include all CategoryTypes Currency ordinals
            addresses_found = new List<SmartProfile.AddressesView>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
             join Category in financeviewmodel.PLO.finance_categoriesList
             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
             equals new { Category.USERNAME, Category.CUBEFACE_CODE }
             join CategoryTypes in financeviewmodel.PLO.finance_categorytypesList
             on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
             equals new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE }
             join Account in financeviewmodel.PLO.finance_accountsList
             on new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE, CategoryTypes.CURRENCY_ORDINAL }
             equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
             //join Address in ourviewmodel.PLO.finance_addressesList
             //on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.UDPRN }
             //equals new { Address.USERNAME, Address.CUBEFACE_CODE, Address.UDPRN }
             join AddressView in ourviewmodel.SmartProfile.addressesviewList
             on new { Account.USERNAME, Account.UDPRN }
             equals new { AddressView.USERNAME, AddressView.UDPRN }
             orderby AddressView.ADDRESS_CREATED descending
             select AddressView);

            // UDPRN not guaranteed unique!
            addresses_found = new List<SmartProfile.AddressesView>
                (addresses_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.UDPRN       // !!!
                }));
            //addresses_found = new List<SmartUsers.Addresses>
            //    (from Address in addresses_found
            //     orderby Address.ADDRESS_CREATED descending
            //     select Address);
            return addresses_found;
        }


        // This is only called from SmartTest in SmartDashboard btw
        internal static List<SmartProfile.AddressesView> Finance_Find_Addresses(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            string udprn = "")
        {
            // There IS A JOIN to Cubefaces here, because addresses
            // HERE are limited to the Finance Cubeface - in fact you WANT
            // addresses to be visible across ALL Cubefaces so that you
            // can link Finance AND Utility AND Insurance et. al.
            //
            // However HERE we just want the Addresses for which our
            // Accounts apply to (assuming of course that we can always
            // stick a UDPRN in!)
            List<SmartProfile.AddressesView> addresses_found = new List<SmartProfile.AddressesView>();

            if (string.IsNullOrEmpty(udprn))
            {
                // Remove restriction on CategoryType CURRENCY_ORDINAL
                addresses_found = new List<SmartProfile.AddressesView>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Category in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 join CategoryTypes in financeviewmodel.PLO.finance_categorytypesList
                 on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 equals new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE }
                 join Account in financeviewmodel.PLO.finance_accountsList
                 on new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE, CategoryTypes.CURRENCY_ORDINAL }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
                 //join Address in financeviewmodel.PLO.finance_addressesList
                 //on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.UDPRN }
                 //equals new { Address.USERNAME, Address.CUBEFACE_CODE, Address.UDPRN }
                 join AddressView in ourviewmodel.SmartProfile.addressesviewList
                 on new { Account.USERNAME, Account.UDPRN }
                 equals new { AddressView.USERNAME, AddressView.UDPRN }
                 where Category.CHECKED != "" //Category.CATEGORY_CODE == financeviewmodel.category_code
                 orderby AddressView.ADDRESS_CREATED descending

                 select AddressView);

                // UDPRN not guaranteed unique!
                addresses_found = new List<SmartProfile.AddressesView>
                (addresses_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.UDPRN       // !!!
                }));
            }
            else
            {
                // Remove restriction on CategoryType CURRENCY_ORDINAL
                addresses_found = new List<SmartProfile.AddressesView>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Category in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 join CategoryTypes in financeviewmodel.PLO.finance_categorytypesList
                 on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 equals new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE }
                 join Account in financeviewmodel.PLO.finance_accountsList
                 on new { CategoryTypes.USERNAME, CategoryTypes.CUBEFACE_CODE, CategoryTypes.CATEGORY_CODE, CategoryTypes.CURRENCY_ORDINAL }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
                 //join Address in financeviewmodel.PLO.finance_addressesList
                 //on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.UDPRN }
                 //equals new { Address.USERNAME, Address.CUBEFACE_CODE, Address.UDPRN }
                 join AddressView in ourviewmodel.SmartProfile.addressesviewList
                 on new { Account.USERNAME, Account.UDPRN }
                 equals new { AddressView.USERNAME, AddressView.UDPRN }
                 where Category.CHECKED != "" && //Category.CATEGORY_CODE == financeviewmodel.category_code &&
                        Account.UDPRN == udprn
                 orderby AddressView.ADDRESS_CREATED descending
                 select AddressView);

                // UDPRN not guaranteed unique!
                addresses_found = new List<SmartProfile.AddressesView>
                (addresses_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.UDPRN       // !!!
                }));
            }
            //addresses_found = new List<SmartUsers.Addresses>
            //    (from Address in addresses_found
            //     orderby Address.ADDRESS_CREATED descending
            //     select Address);
            return addresses_found;
        }

        internal static List<SmartFinance.Categories> Finance_Find_CategoriesX(MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        char categoryCode)
        {
            List<SmartFinance.Categories> categories_found =
                    new List<SmartFinance.Categories>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Categories in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                 where //Categories.CHECKED != "" &&
                        Categories.CATEGORY_CODE == categoryCode
                 select Categories);
            return categories_found;
        }
        internal static List<SmartFinance.CategoryTypes> Finance_Find_CategoryTypes(MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        char categoryCode,
                                                                        short Ordinal = 0)
        {
            List<SmartFinance.CategoryTypes> categorytypes_found =
                    new List<SmartFinance.CategoryTypes>();
            // We only restrict on Subscriber if we're looking up Accounts
            if (Ordinal == 0)
            {
                categorytypes_found =
                    new List<SmartFinance.CategoryTypes>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Categories in financeviewmodel.PLO.finance_categoriesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                     join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                     on new { Categories.USERNAME, Categories.CUBEFACE_CODE, Categories.CATEGORY_CODE }
                     equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                     where //Categories.CHECKED != "" &&    // They might not necessarily be checked if we're doing a global scrape
                            Categories.CATEGORY_CODE == categoryCode
                     select CategoryType);
            }
            else
            {
                categorytypes_found =
                    new List<SmartFinance.CategoryTypes>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Categories in financeviewmodel.PLO.finance_categoriesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Categories.USERNAME, Categories.CUBEFACE_CODE }
                     join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                     on new { Categories.USERNAME, Categories.CUBEFACE_CODE, Categories.CATEGORY_CODE }
                     equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                     where //Categories.CHECKED != "" && // They might not necessarily be checked if we're doing a global scrape
                            Categories.CATEGORY_CODE == categoryCode &&
                            CategoryType.CURRENCY_ORDINAL == Ordinal
                     select CategoryType);

            }
            return categorytypes_found;
        }

        internal static string Finance_Lookup_Account_Currency(List<SmartData.CultureView> cultureviewList,
                                                                string currency_symbol)
        {
            List<string> isoConvertToSymbol_found =
                new List<string>
                (from CultureView in cultureviewList
                 where CultureView.SYMBOL == currency_symbol
                 select CultureView.ISOCURRENCYSYMBOL);
            if (isoConvertToSymbol_found.Count > 0)
            {
                return isoConvertToSymbol_found.First();
            }
            return SmartParametersV2016.defaultISOConvertToSymbol;
        }

        internal static CultureInfo Finance_Lookup_INSTITUTION_Culture(List<SmartData.Cultures> culturesList,
                                                    List<SmartFinance.Institutions> INSTITUTIONSList,
                                                    short institution_code)
        {
            List<string> cultureinfo_found =
                new List<string>
                (from Cultures in culturesList
                 join Institution in INSTITUTIONSList
                 on new { Cultures.CULTURE_CODE }
                 equals new { Institution.CULTURE_CODE }
                 where Institution.INSTITUTION_CODE == institution_code
                 select Cultures.CULTUREINFO);
            if (cultureinfo_found.Count > 0)
            {
                return new CultureInfo(cultureinfo_found.First());
            }
            return SmartParametersV2016.defaultCulture;
        }

        internal static List<SmartFinance.BrandAreas> Finance_Find_BrandAreas(FinanceViewModel financeviewmodel,
                                                                                            short area_code)
        {
            List<SmartFinance.BrandAreas> brand_areas_found =
                new List<SmartFinance.BrandAreas>
                (from Brand_Area in financeviewmodel.PLO.brand_areasList
                 where (Brand_Area.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         Brand_Area.AREA_CODE == area_code)
                 select Brand_Area);
            return brand_areas_found;
        }

        public static bool Find_Resource_Accounts(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        short institution_code,
                                                        short brand_code,
                                                        string sortcode,
                                                        string account_no,
                                                        string udprn,
                                                        short currencyOrdinal)
        {
            List<SmartFinance.Accounts> accounts_found =
                Finance_Lookup_Accounts(ourviewmodel,
                financeviewmodel,
                //SmartParametersV2016.defaultDate,
                institution_code,
                brand_code,
                sortcode,
                account_no,
                udprn,
                currencyOrdinal);

            if (accounts_found.Count > 0)
            {
                financeviewmodel.v2accounts_record = accounts_found;
                return true;
            }
            return false;
        }

        // You can come here from several different routes:
        //  Change to providers
        //  Change to accounts
        //  Change to transactions
        //  By submitting and scraping new bank transactions
        //  No, fuck off not HERE - you mean Bank Transactions you dork

        internal static List<SmartFinance.Brands> Finance_Find_BrandURL(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    short institution_code,
                                                                    short brand_code)
        {
            List<SmartFinance.Brands> brands_found =
                new List<SmartFinance.Brands>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Institution in financeviewmodel.PLO.institutionsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Institution.CUBEFACE_CODE }
                 join Brand in financeviewmodel.PLO.brandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 where Brand.INSTITUTION_CODE == institution_code &&
                         Brand.BRAND_CODE == brand_code &&
                         Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                         Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag// Noo Active?? Yes, its fucking missing
                 select Brand);
            return brands_found.Distinct().ToList();
            
        }

        internal static List<SmartFinance.BrandConnection> Finance_Find_BrandConnections(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        short institution_code,
                                        short brand_code,
                                        string category_code)
        {
            List<SmartFinance.BrandConnection> brand_connections_found =
                new List<SmartFinance.BrandConnection>();

            //if (financeviewmodel.PLO.brand_connectionList.Count > 0)
            //{
            //    if (category_code == "")
            //    {
            //        brand_connections_found =
            //            new List<SmartFinance.BrandConnection>
            //            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
            //             join Institution in financeviewmodel.PLO.institutionsList
            //            on new { Cubeface.CUBEFACE_CODE }
            //            equals new { Institution.CUBEFACE_CODE }
            //             join BrandConnection in financeviewmodel.PLO.brand_connectionList
            //             on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
            //             equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE }
            //             where BrandConnection.INSTITUTION_CODE == institution_code &&
            //                   BrandConnection.BRAND_CODE == brand_code
            //             orderby BrandConnection.ORDINAL ascending
            //             select BrandConnection);
            //    }
            //    else
            //    {
            //        brand_connections_found =
            //            new List<SmartFinance.BrandConnection>
            //            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
            //             join Institution in financeviewmodel.PLO.institutionsList
            //            on new { Cubeface.CUBEFACE_CODE }
            //            equals new { Institution.CUBEFACE_CODE }
            //             join BrandConnection in financeviewmodel.PLO.brand_connectionList
            //             on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
            //             equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.INSTITUTION_CODE }
            //             where BrandConnection.INSTITUTION_CODE == institution_code &&
            //                   BrandConnection.BRAND_CODE == brand_code &&
            //                   BrandConnection.CATEGORY_CODE.Contains(category_code)
            //             orderby BrandConnection.ORDINAL ascending
            //             select BrandConnection);
            //    }
            //}
            return brand_connections_found;
        }

        internal static List<SmartFinance.TransactionsCategories> InitialTransactionsAddresses(FinanceViewModel financeviewmodel,
                                                List<SmartFinance.TransactionsCategories> bank_transactions_found,
                                                string udprn)
        {
            //List<SmartFinance.TransactionsCategories> transactionscategories_found = new List<SmartFinance.TransactionsCategories>();


            // Reduce em down by UDPRN if we want selected Addresses
            if (!string.IsNullOrEmpty(udprn))
            {
                return new List<SmartFinance.TransactionsCategories>
                    (from Transactions in bank_transactions_found
                     where Transactions.UDPRN == udprn
                     select Transactions);
            }
            // Safety first!!
            return bank_transactions_found;
        }

        internal static List<SmartFinance.TransactionsCategories> InitialTransactionsDates(FinanceViewModel financeviewmodel,
            List<SmartFinance.TransactionsCategories> bank_transactions_found,
#if WINFORMS || WPF || SMARTMAUI
                                                    DateTime startDate,
                                                    DateTime endDate
#endif
#if WINUI
                                                    DateTimeOffset startDate,
                                                    DateTimeOffset endDate
#endif
#if ANDROIDX
                                                    DateTime startDate,
                                                    DateTime endDate
#endif
                                                    )
        {
            // Reduce em down by DATE ALWAYS!!
            // No - not always - not when we have ticked an Account! 'Cos we have
            // NO IDEA of the date range of the account
            //if (respect_dates)
            //{
#if WINUI
                startDate = startDate.DateTime;
                endDate = endDate.DateTime;
#endif
            bank_transactions_found = new List<SmartFinance.TransactionsCategories>
                (from Transactions in bank_transactions_found
                 where (Transactions.TRANSACTION_DATE >= startDate &&
                     Transactions.TRANSACTION_DATE <= endDate)
                 select Transactions);
            //}
            //else
            //{
            //    bank_transactions_found = new List<SmartFinance.Transactions>
            //            (from Transactions in bank_transactions_found
            //             where (Transactions.TRANSACTION_DATE >= SmartParametersV2016.defaultDate &&
            //                 Transactions.TRANSACTION_DATE <= SmartParametersV2016.defaultMaxdate)
            //             select Transactions);
            //}
            return bank_transactions_found;
        }

        internal static List<SmartFinance.TransactionsCategories> InitialTransactionsAccounts(FinanceViewModel financeviewmodel,
                                                            List<SmartFinance.TransactionsCategories> bank_transactions_found)
        {
            //// Reduce em down the Accounts by Category - Bank, Savings or Investments
            //bank_transactions_found =
            //    new List<SmartFinance.TransactionsCategories>(
            //        from Transactions in bank_transactions_found
            //        join Accounts in financeviewmodel.subset_accounts_found
            //        on new { Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
            //        equals new { Accounts.SORTCODE, Accounts.ACCOUNT_NO, Accounts.UDPRN }
            //        select Transactions);
            ////return bank_transactions_found;

            bank_transactions_found =
                new List<SmartFinance.TransactionsCategories>(
                    from Accounts in financeviewmodel.selectedAccountsList
                    join Transactions in financeviewmodel.PLO.transactionsList
                    on new { Accounts.SORTCODE, Accounts.ACCOUNT_NO, Accounts.UDPRN }
                    equals new { Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                    join TransactionsCategories in bank_transactions_found
                    on new { Transactions.STATEMENT_DATE, Transactions.STATEMENT_NO, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                    equals new { TransactionsCategories.STATEMENT_DATE, TransactionsCategories.STATEMENT_NO, TransactionsCategories.SORTCODE, TransactionsCategories.ACCOUNT_NO, TransactionsCategories.UDPRN }
                    select TransactionsCategories);
            return bank_transactions_found;
        }

        internal static List<SmartFinance.TransactionsCategories> InitialTransactionsGroups(FinanceViewModel financeviewmodel,
                                                            List<SmartFinance.TransactionsCategories> transactionscategories_found)
        {
            bool suckem = false;
            List<SmartFinance.SelectedTransactionGroups> tgroups = new List<SmartFinance.SelectedTransactionGroups>();

            foreach (FinanceViewModel.TransactionGroupItem tgitem in financeviewmodel.temp_groups)
            {
                if (tgitem.IsChecked == false)
                {
                    suckem = true;
                }
                else
                {
                    SmartFinance.SelectedTransactionGroups abc = new SmartFinance.SelectedTransactionGroups()
                    {
                        TYPE = tgitem.Content
                    };
                    tgroups.Add(abc);
                }
            }

            if (suckem)
            {
                // Reduce em down the Accounts by Category - Bank, Savings or Investments
                transactionscategories_found =
                new List<SmartFinance.TransactionsCategories>(
                    from TransactionsCategories in transactionscategories_found
                    join TGroups in tgroups
                    on new { TransactionsCategories.TYPE }
                    equals new { TGroups.TYPE }
                    select TransactionsCategories);
            }
            return transactionscategories_found;
        }

        //internal static List<T1> InitialTransactionsAccountsGENERIC<T1, T2>(List<T1> subset_accounts_found,
        //                                                    List<T2> transactions_found)
        //{
        //    // Reduce em down the Accounts by Category - Bank, Savings or Investments
        //    transactions_found =
        //        new List<SmartFinance.Transactions>(
        //            from T1 in transactions_found
        //            join T2 in subset_accounts_found
        //            on new { T1.SORTCODE, T1.ACCOUNT_NO, T1.UDPRN }
        //            equals new { T2.SORTCODE, T2.ACCOUNT_NO, T2.UDPRN }
        //            select T1);
        //    return transactions_found as List<T1>;
        //}

        //internal static List<SmartFinance.TransactionsCategories> InitialTransactionsTransactions(FinanceViewModel financeviewmodel,
        //                                                    List<SmartFinance.TransactionsCategories> transactionscategories_found)
        //{
        //    // Reduce em down the Accounts by Category - Bank, Savings or Investments
        //    transactionscategories_found =
        //        new List<SmartFinance.TransactionsCategories>(
        //            from TransactionsCategories in transactionscategories_found
        //            join XYZ in financeviewmodel.FinanceTransactionGroupsList
        //            on new { TransactionsCategories.CREDITDEBIT_INDICATOR }
        //            equals new { XYZ.CREDITDEBIT_INDICATOR }
        //            where XYZ.IsChecked
        //            select TransactionsCategories);
        //    return transactionscategories_found;
        //}

        
        internal static List<SmartFinance.TransactionsCategories>
            Finance_Lookup_BankTransaction(FinanceViewModel financeviewmodel,
                                    string username,
                                    char cubeface_code,
                                    short institution_code,
                                    short brand_code,
                                    string sortcode,
                                    string account_no,
                                    string UDPRN,
                                    DateTime statementDate,
                                    short statementNo,
                                    DateTime transactionDate,
                                    int paid_in,
                                    short[] transCodes,
                                    string description,
                                    string type,            // e.g. 'FIAT'
                                    double cryptoAmount,          // e.g. £1.23
                                    short cryptoAmountOrdinal,    // e.g. GBP
                                    double amount,          // e.g. £1.23
                                    short amountOrdinal,    // e.g. GBP
                                    double balance,         // e.g. 0
                                    short balanceOrdinal,   // e.g. 0
                                    int balancePaidIn)
        {
            // We include the Username because the transactions list MAY
            // (just) include entries from other Users (via MultiUser)
            // and they might (just) contain the same data APART from
            // Username!!
            List<SmartFinance.TransactionsCategories> transactionscategories_found =
                new List<SmartFinance.TransactionsCategories>
                (from TransactionCategory in financeviewmodel.PLO.transactionscategoriesList
                 where
                     TransactionCategory.USERNAME == username &&
                     TransactionCategory.CUBEFACE_CODE == cubeface_code &&
                     TransactionCategory.INSTITUTION_CODE == institution_code &&
                     TransactionCategory.BRAND_CODE == brand_code &&
                     TransactionCategory.SORTCODE == sortcode &&
                     TransactionCategory.ACCOUNT_NO == account_no &&
                     TransactionCategory.UDPRN == UDPRN &&
                     TransactionCategory.STATEMENT_DATE == statementDate &&
                     TransactionCategory.STATEMENT_NO == statementNo &&
                     TransactionCategory.TRANSACTION_DATE == transactionDate &&
                     TransactionCategory.CREDITDEBIT_INDICATOR == paid_in &&
                     TransactionCategory.TRANSGROUP_CODE == transCodes[0] && // Assumes everything HAS a code of course ...
                     TransactionCategory.TRANSACTION_CODE == transCodes[1] && // Assumes everything HAS a code of course ...
                     TransactionCategory.DESCRIPTION == description &&
                     TransactionCategory.TYPE == type &&
                     TransactionCategory.CRYPTO_AMOUNT == cryptoAmount &&
                     TransactionCategory.CRYPTO_CURRENCY_ORDINAL == cryptoAmountOrdinal &&
                     TransactionCategory.AMOUNT == amount &&
                     TransactionCategory.AMOUNT_CURRENCY_ORDINAL == amountOrdinal &&
                     TransactionCategory.BALANCE_AMOUNT == balance && // Might the balance be different??
                     TransactionCategory.BALANCE_CURRENCY_ORDINAL == balanceOrdinal &&
                     TransactionCategory.BALANCE_CREDITDEBIT_INDICATOR == balancePaidIn
                 orderby TransactionCategory.TRANSACTION_DATE descending,
                         TransactionCategory.SEQUENCE_NO descending // Same thing as Login.CREATED desc as we have matched on this field ..
                 select TransactionCategory);
            return transactionscategories_found;
            
        }

        internal static List<SmartFinance.Transaction_Groups> Finance_Find_TransactionGroupsNew(MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    short institution_code,
                                    short brand_code)
        {
            List<SmartFinance.Transaction_Groups> transaction_groupsFound =
                new List<SmartFinance.Transaction_Groups>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Institution in financeviewmodel.PLO.institutionsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Institution.CUBEFACE_CODE }
                 join Brand in financeviewmodel.PLO.brandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 join TransGroup in financeviewmodel.PLO.transaction_groupsList
                 on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
                 equals new { TransGroup.CUBEFACE_CODE, TransGroup.INSTITUTION_CODE, TransGroup.BRAND_CODE }
                 where Institution.INSTITUTION_CODE == institution_code &&
                        Brand.BRAND_CODE == brand_code
                 select TransGroup);
            return transaction_groupsFound;
        }
        internal static List<SmartFinance.Transaction_Types> Finance_Find_TransactionTypesNew(MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    short institution_code,
                                    short brand_code)
        {
            List<SmartFinance.Transaction_Types> transaction_typesFound =
                new List<SmartFinance.Transaction_Types>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Institution in financeviewmodel.PLO.institutionsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Institution.CUBEFACE_CODE }
                 join Brand in financeviewmodel.PLO.brandsList
                 on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                 join TransType in financeviewmodel.PLO.transaction_typesList
                 on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
                 equals new { TransType.CUBEFACE_CODE, TransType.INSTITUTION_CODE, TransType.BRAND_CODE }
                 where Institution.INSTITUTION_CODE == institution_code &&
                        Brand.BRAND_CODE == brand_code
                 select TransType);
            return transaction_typesFound;
        }

        internal static short[] Finance_Lookup_TransactionCode(FinanceViewModel financeviewmodel,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    int creditdebit_indicator,
                                                    string transaction_type,
                                                    bool upperCase,
                                                    short institution_code,
                                                    short brand_code,
                                                    char category_code,
                                                    string description = "")
        {
            short[] transCodes = new short[2] { 0, 0 };

            List<SmartFinance.Transaction_Groups> abcd = new List<SmartFinance.Transaction_Groups>(
                from Groups in transaction_groupsFound
                where Groups.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                        Groups.INSTITUTION_CODE == institution_code &&
                        Groups.BRAND_CODE == brand_code &&
                        Groups.CATEGORY_CODE == category_code &&
                        Groups.CREDITDEBIT_INDICATOR == creditdebit_indicator &&
                        Groups.AMOUNT_TYPE.Contains(upperCase ? transaction_type.ToUpper() : transaction_type)
                select Groups);
            if (abcd.Count > 0)
            {
                transCodes[0] = abcd.First().TRANSGROUP_CODE;
                if (description != "")
                {
                    foreach (SmartFinance.Transaction_Groups ts_group in abcd)
                    {
                        //transaction_description = transaction_description.Replace(desc, "").Trim();
                        transCodes[0] = ts_group.TRANSGROUP_CODE;
                        // Got code 1, can I find code 2?
                        List<SmartFinance.Transaction_Types> xyz = new List<SmartFinance.Transaction_Types>(
                            from Types in transaction_typesFound
                            where //Types.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                                  //Types.INSTITUTION_CODE == institution_code &&
                                  //Types.BRAND_CODE == brand_code &&
                                    Types.CATEGORY_CODE == category_code &&
                                    Types.CREDITDEBIT_INDICATOR == creditdebit_indicator &&
                                    Types.TRANSGROUP_CODE == ts_group.TRANSGROUP_CODE &&
                                    Types.DESCRIPTION.Contains(description)
                            select Types);
                        if (xyz.Count > 0)
                        {
                            transCodes[1] = xyz.First().TRANSACTION_CODE;
                            break;
                        }
                    }
                }
            }

            //foreach (SmartFinance.Transaction_Groups ts_type in transaction_groupsFound)
            //{
            //    //? What's it got to do with Usernames???
            //    // Absolutely Everything??
            //    if (ts_type.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
            //        ts_type.INSTITUTION_CODE == institution_code &&
            //        ts_type.BRAND_CODE == brand_code &&
            //        ts_type.CATEGORY_CODE == category_code &&
            //        ts_type.CREDITDEBIT_INDICATOR == creditdebit_indicator)
            //    {
            //        string amountType = ts_type.AMOUNT_TYPE;
            //        if (upper_case)
            //        {
            //            amountType = amountType.ToUpper();
            //        }
            //        if (transaction_type.Contains(amountType))
            //        {
            //            //transaction_description = transaction_description.Replace(desc, "").Trim();
            //            transCodes[0] = ts_type.TRANSGROUP_CODE;
            //            // Got code 1, can I find code 2?


            //            transCodes[1] = ts_type.TRANSACTION_CODE;
            //            break;
            //        }
            //    }
            //}
            return transCodes;
        }


        internal static short[] Finance_Lookup_TransactionTypes(FinanceViewModel financeviewmodel,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    int creditdebit_indicator,
                                                    string transaction_type,
                                                    bool upperCase,
                                                    short institution_code,
                                                    short brand_code,
                                                    char category_code)
        {
            short[] transCodes = new short[2] { 0, 0 };

            if (transaction_type != "")
            {
                List<SmartFinance.Transaction_Types> xyz = new List<SmartFinance.Transaction_Types>(
                    from Types in transaction_typesFound
                    where Types.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                            Types.INSTITUTION_CODE == institution_code &&
                            Types.BRAND_CODE == brand_code &&
                            Types.CATEGORY_CODE == category_code &&
                            Types.CREDITDEBIT_INDICATOR == creditdebit_indicator &&
                            Types.DESCRIPTION.Contains(transaction_type) ||
                            transaction_type.Contains(Types.DESCRIPTION)
                    select Types);
                if (xyz.Count > 0)
                {
                    transCodes[0] = xyz.First().TRANSGROUP_CODE;
                    transCodes[1] = xyz.First().TRANSACTION_CODE;
                }
                return transCodes;
            }
            return transCodes;
        }
        internal static string Finance_Find_BrandCategories(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        short institution_code)
        {
            List<SmartFinance.BrandAccounts> brandcategories_found = new List<SmartFinance.BrandAccounts>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
             join Institution in financeviewmodel.PLO.institutionsList
             on new { Cubeface.CUBEFACE_CODE }
             equals new { Institution.CUBEFACE_CODE }
             join Brand in financeviewmodel.PLO.brandsList
             on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
             equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
             join BrandAccount in financeviewmodel.PLO.brand_accountsList
             on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
             equals new { BrandAccount.CUBEFACE_CODE, BrandAccount.INSTITUTION_CODE, BrandAccount.BRAND_CODE }
             where Institution.INSTITUTION_CODE == institution_code &&
                      Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                      Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
             select BrandAccount).ToList();
            string maxCategories = "";
            foreach (SmartFinance.BrandAccounts brand_account in brandcategories_found)
            {
                maxCategories += brand_account.CATEGORY_CODE;
            }
            return maxCategories;
        }

        
        
        internal static List<SmartFinance.BrandsView> Finance_Find_InstitutionBrands(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        short institution_code,
                                                        short brand_code)
        //char category_code)
        {
            List<SmartFinance.BrandsView> brands_found = new List<SmartFinance.BrandsView>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
             join Institution in financeviewmodel.PLO.institutionsList
             on new { Cubeface.CUBEFACE_CODE }
             equals new { Institution.CUBEFACE_CODE }
             join Brand in financeviewmodel.PLO.brandsList
             on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
             equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
             join BrandAccount in financeviewmodel.PLO.brand_accountsList
             on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
             equals new { BrandAccount.CUBEFACE_CODE, BrandAccount.INSTITUTION_CODE, BrandAccount.BRAND_CODE }
             join Category in financeviewmodel.PLO.finance_categoriesList
             on new { BrandAccount.CUBEFACE_CODE, BrandAccount.CATEGORY_CODE }
             equals new { Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
             join CategoryCodes in financeviewmodel.PLO.category_codesList
             on new { Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
             equals new { CategoryCodes.CUBEFACE_CODE, CategoryCodes.CATEGORY_CODE }
             where Institution.INSTITUTION_CODE == institution_code &&
                      Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                      Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag //&&
                                                                           //Category.CATEGORY_CODE == category_code
             select new SmartFinance.BrandsView()
             {
                 CUBEFACE_CODE = Cubeface.CUBEFACE_CODE,
                 INSTITUTION_CODE = Institution.INSTITUTION_CODE,
                 BRAND_CODE = Brand.BRAND_CODE,
                 ACTIVE_FLAG = Brand.ACTIVE_FLAG,
                 BRAND_NAME = Brand.BRAND_NAME,
                 CATEGORY_CODE = BrandAccount.CATEGORY_CODE,
                 ORDINAL = CategoryCodes.ORDINAL,
                 DESCRIPTION = Brand.DESCRIPTION
             }).ToList();

            // Don't understand the Category.CHECKED != "" part below??
            // It represents the default Category for the user



            //    List<SmartFinance.Brands> brands_found = new List<SmartFinance.Brands>();
            //if (institution_code > 0 &&
            //    brand_code > 0)
            //{
            //    brands_found = new List<SmartFinance.Brands>
            //        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
            //        join Institution in financeviewmodel.PLO.institutionsList
            //        on new { Cubeface.CUBEFACE_CODE }
            //        equals new { Institution.CUBEFACE_CODE }
            //        join Brand in financeviewmodel.PLO.brandsList
            //        on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
            //        equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
            //        where Institution.INSTITUTION_CODE == institution_code &&
            //                 Brand.BRAND_CODE == brand_code &&
            //                 Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
            //                 Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
            //         select Brand);
            //    //brands_found = new List<SmartFinance.Brands>(brands_found.Distinct());
            //    //if (financeviewmodel.category_code != SmartParametersV2016.defaultChar)
            //    //{
            //    brands_found = new List<SmartFinance.Brands>
            //        (from Brand in brands_found
            //         join BrandAccount in financeviewmodel.PLO.brand_accountsList
            //             on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
            //             equals new { BrandAccount.CUBEFACE_CODE, BrandAccount.INSTITUTION_CODE, BrandAccount.BRAND_CODE }
            //         join Category in financeviewmodel.PLO.finance_categoriesList
            //             on new { BrandAccount.CUBEFACE_CODE, BrandAccount.CATEGORY_CODE }
            //             equals new { Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
            //         where Category.CHECKED != "" //BrandAccount.CATEGORY_CODE == financeviewmodel.category_code
            //         select Brand);
            //    //}
            //}
            return brands_found.Distinct().ToList();
           
        }
        internal static List<SmartFinance.Institutions> Finance_Lookup_Institution(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            short institution_code
                                                                            //char activeFlag,       // Either 'Y' for scrapeables or ALL of them
                                                                            )
        {
            // ACTIVE_CODE ... is for those Brands you can S C R A P E you dickhead
            // These are the ones you want to see in your Institutions dropdown!
            List<SmartFinance.Institutions> institutions_found =
                new List<SmartFinance.Institutions>();

            if (institution_code == 0)
            {
                // Lookup based on parameter name, not the financeviewmodelcode
                institutions_found =
                    new List<SmartFinance.Institutions>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Institution in financeviewmodel.PLO.institutionsList
                    on new { Cubeface.CUBEFACE_CODE }
                    equals new { Institution.CUBEFACE_CODE }
                     where Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                     select Institution);
            }
            else
            {
                // Lookup based on financeviewmodel code and not parameter name
                institutions_found = new List<SmartFinance.Institutions>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Institution in financeviewmodel.PLO.institutionsList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Institution.CUBEFACE_CODE }
                     where Institution.INSTITUTION_CODE == institution_code &&
                           Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                     select Institution);
            }
            return institutions_found.Distinct().ToList();
        }

        internal static List<SmartFinance.Logins> Finance_Find_Logins(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            char activeFlag,       // Either 'Y' for scrapeables or ALL of them
                                                                            short institutionCode,
                                                                            bool filterConnections)
        {
            // All the Logins set-up for this Brand/Provider/Exchange
            List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
             join Institution in financeviewmodel.PLO.institutionsList
             on new { Cubeface.CUBEFACE_CODE }
             equals new { Institution.CUBEFACE_CODE }
             join InstitutionInfo in financeviewmodel.PLO.institution_infoList
             on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
             equals new { InstitutionInfo.CUBEFACE_CODE, InstitutionInfo.INSTITUTION_CODE }
             join Brands in financeviewmodel.PLO.brandsList
             on new { InstitutionInfo.CUBEFACE_CODE, InstitutionInfo.INSTITUTION_CODE }
             equals new { Brands.CUBEFACE_CODE, Brands.INSTITUTION_CODE }
             join Logins in financeviewmodel.PLO.finance_loginsList         //LoginsViewList
             on new { Brands.CUBEFACE_CODE, Brands.INSTITUTION_CODE, Brands.BRAND_CODE }
             equals new { Logins.CUBEFACE_CODE, Logins.INSTITUTION_CODE, Logins.BRAND_CODE }
             where Institution.INSTITUTION_CODE == institutionCode &&
                    Institution.ACTIVE_FLAG == activeFlag &&
                    Brands.ACTIVE_FLAG == activeFlag
             select Logins);
            if (filterConnections)
            {
                // Remove all with LOGIN_METHOD = 0
                logins_found = new List<SmartFinance.Logins>
                (from Login in logins_found
                 where Login.LOGIN_METHOD > 0
                 select Login);
            }
            return logins_found;
        }
        internal static string Finance_Lookup_InstitutionName(
                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                short institution_code)
        {
            SmartFinance.InstitutionInfo institutionInfo =
                (from cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join info in financeviewmodel.PLO.institution_infoList
                     on cubeface.CUBEFACE_CODE equals info.CUBEFACE_CODE
                 join infos in financeviewmodel.PLO.institutionsList
                     on new { info.CUBEFACE_CODE, info.INSTITUTION_CODE }
                     equals new { infos.CUBEFACE_CODE, infos.INSTITUTION_CODE }
                 where info.INSTITUTION_CODE == institution_code &&
                        infos.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                 select info).SingleOrDefault();
            // Return empty if institutionInfo is null
            if (institutionInfo != null)
            {
                return institutionInfo.INSTITUTION_NAME;
            }
            else
            {
                return "";
            }
        }

        internal static string Finance_Lookup_SingleBrandName(
                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                short institution_code,
                                short brand_code)
        {            
            SmartFinance.Brands brandInfo = 
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                join Institution in financeviewmodel.PLO.institutionsList
                on new { Cubeface.CUBEFACE_CODE }
                equals new { Institution.CUBEFACE_CODE }
                join Brand in financeviewmodel.PLO.brandsList
                on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                where Institution.INSTITUTION_CODE == institution_code &&
                    Brand.BRAND_CODE == brand_code &&
                    Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                select Brand).SingleOrDefault();
            // Return empty if brandInfo is null
            if (brandInfo != null)
            {
                return brandInfo.BRAND_NAME;
            }
            else
            {
                return "";
            }
        }

        internal static List<SmartFinance.Brands> Finance_Lookup_BrandName(MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel,
                                                                            //char activeFlag,       // Either 'Y' for scrapeables or ALL of them
                                                                            string brand_name,
                                                                            short institution_code,
                                                                            short brand_code)
        {
            // Both INSTITUTION_CODE and BRAND_CODE *must* be in financeviewmodel ...
            // Its possible for the BRAND_NAME to be the same for two or more
            // different Institutions (rare, I admit, but possible ...)

            // ACTIVE_CODE ... is for those Brands you can S C R A P E you dickhead
            // These are the ones you want to see in your Institutions dropdown!
            List<SmartFinance.Brands> brands_found = new List<SmartFinance.Brands>();

            if (brand_name == "")
            {
                if (brand_code == 0)
                {
                    // Lookup based on financeviewmodel code and not parameter name
                    brands_found = new List<SmartFinance.Brands>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                         join Institution in financeviewmodel.PLO.institutionsList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Institution.CUBEFACE_CODE }
                         join Brand in financeviewmodel.PLO.brandsList
                         on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                         equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                         where Institution.INSTITUTION_CODE == institution_code &&
                                Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                         select Brand);
                }
                else
                {
                    brands_found = new List<SmartFinance.Brands>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                         join Institution in financeviewmodel.PLO.institutionsList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Institution.CUBEFACE_CODE }
                         join Brand in financeviewmodel.PLO.brandsList
                         on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                         equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                         where Institution.INSTITUTION_CODE == institution_code &&
                                 Brand.BRAND_CODE == brand_code &&
                                 Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                                 Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                         select Brand);
                }
            }
            else
            {
                // Lookup based on parameter name, not the financeviewmodel code
                brands_found = new List<SmartFinance.Brands>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Institution in financeviewmodel.PLO.institutionsList
                      on new { Cubeface.CUBEFACE_CODE }
                      equals new { Institution.CUBEFACE_CODE }
                     join Brand in financeviewmodel.PLO.brandsList
                     on new { Institution.CUBEFACE_CODE, Institution.INSTITUTION_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE }
                     where Institution.INSTITUTION_CODE == institution_code &&
                             Brand.BRAND_CODE == brand_code &&
                             Brand.BRAND_NAME == brand_name &&
                             Institution.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                             Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                     select Brand);
            }
            return brands_found.Distinct().ToList();
        }
        internal static List<SmartFinance.Switches> Finance_Find_CategoryLASTDISPLAYANY(MainViewModel ourviewmodel,
                                                                                            FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Switches> switches_found = new List<SmartFinance.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)

                 join Switches in financeviewmodel.PLO.finance_switchesList
                  on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                  equals new { Switches.USERNAME, Switches.CUBEFACE_CODE }
                  // ((resource_codeList.Any((int)RANDOM => (int)RANDOM == Resource.RANDOM)) &&
                  //((resource_codeList.Any(ACCOUNT_NO => ACCOUNT_NO == Resource.ACCOUNT_NO)) &&
                  //(Resource.LAST_DISPLAY == SmartParametersV2016.defaultLastDisplay)
                 select Switches);
            return switches_found;
        }

        internal static List<SmartFinance.BrandMatrix> Finance_Find_BrandMatrix(FinanceViewModel financeviewmodel,
                                                                                                short institution_code,
                                                                                                short brand_code)
        {
            List<SmartFinance.BrandMatrix> brand_matrix_found =
                new List<SmartFinance.BrandMatrix>
                (from Brand_Area in financeviewmodel.PLO.brand_matrixList
                 where (Brand_Area.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         Brand_Area.INSTITUTION_CODE == institution_code &&
                         Brand_Area.BRAND_CODE == brand_code)
                 select Brand_Area);
            return brand_matrix_found;
        }

        internal static List<SmartFinance.Switches> Finance_Lookup_Switches(MainViewModel ourviewmodel,
                                                                                            FinanceViewModel financeviewmodel,
                                                                                            short institution_code,
                                                                                            short brand_code)
        {
            List<SmartFinance.Switches> switches_found = new List<SmartFinance.Switches>();
            switches_found = new List<SmartFinance.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Category in financeviewmodel.PLO.finance_categoriesList
                  on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                  equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 join Account in financeviewmodel.PLO.finance_accountsList
                 on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE }
                 join Switch in financeviewmodel.PLO.finance_switchesList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO }
                 equals new { Switch.USERNAME, Switch.CUBEFACE_CODE, Switch.CATEGORY_CODE, Switch.INSTITUTION_CODE, Switch.BRAND_CODE, Switch.SORTCODE, Switch.ACCOUNT_NO }
                 where Category.CHECKED != "" && //Switch.CATEGORY_CODE == financeviewmodel.category_code &&       // Key
                        Account.INSTITUTION_CODE == institution_code &&                 // KEY
                        Account.BRAND_CODE == brand_code                                // Key
                 orderby Switch.SWITCH_CREATED descending                                  // *not* a Key!
                 select Switch);
            return switches_found;
        }

        internal static List<SmartFinance.Switches> Finance_Consumer_CancelList(MainViewModel ourviewmodel,
                                                                                FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Switches> switches_found =
                new List<SmartFinance.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Switch in financeviewmodel.PLO.finance_switchesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Switch.USERNAME, Switch.CUBEFACE_CODE }
                 join Category in financeviewmodel.PLO.finance_categoriesList
                 on new { Switch.USERNAME, Switch.CUBEFACE_CODE, Switch.CATEGORY_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 where Category.CHECKED != "" //Switch.CATEGORY_CODE == financeviewmodel.category_code
                 orderby Switch.LAST_UPDATE descending
                 select Switch);
            return switches_found;
        }

        internal static List<SmartFinance.Accounts> Finance_Lookup_Accounts(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short institution_code = 0,
                                                                short brand_code = 0,
                                                                string sortcode = "",
                                                                string account_no = "",
                                                                string udprn = "",
                                                                short currencyOrdinal = 0)
        {
            List<SmartFinance.Accounts> accounts_found = new List<SmartFinance.Accounts>();


            List < SmartFinance.Categories>categoryies_found = new List<SmartFinance.Categories>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
            join Category in financeviewmodel.PLO.finance_categoriesList
            on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
            equals new { Category.USERNAME, Category.CUBEFACE_CODE }
            where Category.CHECKED != ""
            select Category);

            List<SmartFinance.CategoryTypes> categorytypes_found = new List<SmartFinance.CategoryTypes>
            (from Category in categoryies_found
            join CategoryType in financeviewmodel.PLO.finance_categorytypesList
            on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
            equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
            select CategoryType);

            List<SmartFinance.Accounts> accs_found = new List<SmartFinance.Accounts>
            (from CategoryType in categorytypes_found
            join Account1 in financeviewmodel.PLO.finance_accountsList
            on new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE, CategoryType.CURRENCY_ORDINAL }
            equals new { Account1.USERNAME, Account1.CUBEFACE_CODE, Account1.CATEGORY_CODE, Account1.CURRENCY_ORDINAL }
            select Account1);

                
            if (institution_code == 0 ||
            brand_code == 0 ||
            //account_created == SmartParametersV2016.defaultDate ||
            string.IsNullOrEmpty(account_no) ||
            string.IsNullOrEmpty(sortcode))
            {
                accounts_found = new List<SmartFinance.Accounts>(
                (from Account1 in accs_found
                    join Address in ourviewmodel.SmartProfile.addressesviewList
                    on new { Account1.UDPRN } equals new { Address.UDPRN }
                    select Account1));
            }
            else
            {
                accounts_found = new List<SmartFinance.Accounts>(
                (from Account1 in accs_found
                    join Address in ourviewmodel.SmartProfile.addressesviewList
                    on new { Account1.UDPRN } equals new { Address.UDPRN }
                    where Account1.INSTITUTION_CODE == institution_code &&
                    Account1.BRAND_CODE == brand_code &&
                    Account1.SORTCODE == sortcode &&
                    Account1.ACCOUNT_NO == account_no &&
                    Account1.UDPRN == udprn &&
                    Account1.CURRENCY_ORDINAL == currencyOrdinal
                    select Account1  ));
            }
            return accounts_found;
        }

        internal static List<SmartFinance.Accounts> Finance_Lookup_AccountsCategory(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short institution_code,
                                                                short brand_code,
                                                                char category_code,
                                                                string sortcode,
                                                                string account_no,
                                                                string udprn,
                                                                short accountOrdinal)
        {
            List<SmartFinance.Accounts> accounts_found =
                new List<SmartFinance.Accounts>();

            // No restriction on CategoryType currencies
            accounts_found = new List<SmartFinance.Accounts>
               (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                join Category in financeviewmodel.PLO.finance_categoriesList
                on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                join CategoryType in financeviewmodel.PLO.finance_categorytypesList
                on new { Category.USERNAME, Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                equals new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE }
                join Account in financeviewmodel.PLO.finance_accountsList
                on new { CategoryType.USERNAME, CategoryType.CUBEFACE_CODE, CategoryType.CATEGORY_CODE, CategoryType.CURRENCY_ORDINAL }
                equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.CATEGORY_CODE, Account.CURRENCY_ORDINAL }
                where Account.INSTITUTION_CODE == institution_code &&
                       Account.BRAND_CODE == brand_code &&
                       Account.CATEGORY_CODE == category_code &&
                       Account.SORTCODE == sortcode &&
                       Account.ACCOUNT_NO == account_no &&
                       Account.UDPRN == udprn &&
                       Account.CURRENCY_ORDINAL == accountOrdinal
                select Account);

            // UDPRN not guaranteed unique!
            accounts_found = new List<SmartFinance.Accounts>
                (accounts_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.INSTITUTION_CODE,
                    key.BRAND_CODE,
                    key.CATEGORY_CODE,
                    key.SORTCODE,
                    key.ACCOUNT_NO,
                    key.UDPRN,       // !!!
                    key.CURRENCY_ORDINAL
                }));
            accounts_found = new List<SmartFinance.Accounts>
                (from Account in accounts_found
                 orderby Account.ACCOUNT_CREATED descending
                 select Account);

            return accounts_found;
        }

        internal static List<SmartFinance.Transaction_Groups> Finance_Lookup_Transaction_Groups(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short institution_code,
                                                                short brand_code)
        {
            List<SmartFinance.Transaction_Groups> tgroups_found =
                new List<SmartFinance.Transaction_Groups>();

            if (institution_code > 0 &&
                brand_code > 0)
            {
                // Don't include CategoryType - its got nothing to do with these
                tgroups_found = new List<SmartFinance.Transaction_Groups>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Category in financeviewmodel.PLO.finance_categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 join Groups in financeviewmodel.PLO.transaction_groupsList
                 on new { Category.CUBEFACE_CODE, Category.CATEGORY_CODE }
                 equals new { Groups.CUBEFACE_CODE, Groups.CATEGORY_CODE }
                 where
                 Groups.INSTITUTION_CODE == institution_code &&
                 Groups.BRAND_CODE == brand_code &&
                 Category.CHECKED != ""
                 select Groups);
            }
            return tgroups_found;
        }

        internal static List<SmartFinance.Categories> Finance_Find_ViewLastChecked(MainViewModel ourviewmodel,
                                                                                        FinanceViewModel financeviewmodel,
                                                                                        List<SmartFinance.Categories> categoriesList)
        {
            List<SmartFinance.Categories> categories_found =
                new List<SmartFinance.Categories>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                on new { Consumer.USERNAME }
                equals new { Cubeface.USERNAME }
                 join Category in categoriesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                 where Consumer.Include &&
                       Cubeface.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                       Category.CHECKED != ""
                 select Category);
            return categories_found;
        }

        internal static List<SmartFinance.Brands> FinanceFindBrandLogins(MainViewModel ourviewmodel,
                                                                                    FinanceViewModel financeviewmodel,
                                                                                    short institution_code)
        {
            List<SmartFinance.Brands> brands_found = new List<SmartFinance.Brands>();
            // If we are looking for Crypto, then the Brand Account is 'C'
            brands_found = new List<SmartFinance.Brands>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
             join Brand in financeviewmodel.PLO.brandsList
             on new { Cubeface.CUBEFACE_CODE }
             equals new { Brand.CUBEFACE_CODE }
             join Login in financeviewmodel.PLO.finance_loginsList
             on new { Brand.CUBEFACE_CODE, Brand.INSTITUTION_CODE, Brand.BRAND_CODE }
             equals new { Login.CUBEFACE_CODE, Login.INSTITUTION_CODE, Login.BRAND_CODE }
             where Brand.INSTITUTION_CODE == institution_code &&
                    Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag &&
                    Login.ACTIVE_FLAG == SmartParametersV2016.activeFlag
             select Brand);
            return brands_found.Distinct().ToList();
        }

        internal static List<SmartFinance.Brands> Finance_Find_Brands(MainViewModel ourviewmodel,
                                                                                    FinanceViewModel financeviewmodel,
                                                                                    short institution_code,
                                                                                    short brand_code = 0)
        {
            List<SmartFinance.Brands> brands_found = new List<SmartFinance.Brands>();
            // If we are looking for Crypto, then the Brand Account is 'C'
            if (brand_code != 0)
            {
                brands_found = new List<SmartFinance.Brands>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Brand in financeviewmodel.PLO.brandsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Brand.CUBEFACE_CODE }
                 where Brand.INSTITUTION_CODE == institution_code &&
                         Brand.BRAND_CODE == brand_code &&
                         Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                 select Brand);
            }
            else
            {
                brands_found = new List<SmartFinance.Brands>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                 join Brand in financeviewmodel.PLO.brandsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Brand.CUBEFACE_CODE }
                 where Brand.INSTITUTION_CODE == institution_code &&
                         Brand.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                 select Brand);

            }            
            return brands_found.Distinct().ToList();
        }

        internal static List<SmartFinance.Categories> Finance_Find_CategoriesList(MainViewModel ourviewmodel,
                                                                                    FinanceViewModel financeviewmodel,
                                                                                    char category_code)
        {
            List<SmartFinance.Categories> categories_found = new List<SmartFinance.Categories>();
            if (category_code == SmartParametersV2016.defaultChar)
            {
                categories_found = new List<SmartFinance.Categories>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Category in financeviewmodel.PLO.finance_categoriesList
                      on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                      equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                     select Category);
            }
            else
            {
                categories_found = new List<SmartFinance.Categories>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                     join Category in financeviewmodel.PLO.finance_categoriesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Category.USERNAME, Category.CUBEFACE_CODE }
                     where Category.CATEGORY_CODE == category_code
                     select Category);
            }
            return categories_found;
        }
        internal static string Finance_LookupUrl_Prefix(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                short institution_code,
                                                short brand_code)
        {
            List<SmartFinance.Brands> brands_found =
                new List<SmartFinance.Brands>
                (from Brands in financeviewmodel.PLO.brandsList
                 where Brands.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
                         Brands.INSTITUTION_CODE == institution_code &&
                         Brands.BRAND_CODE == brand_code &&
                         Brands.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                 select Brands);
            if (brands_found.Count > 0)
            {
                return brands_found.First().URL_PREFIX;
#if PRODUCTION
                return brands_found.First().URL_BASE_PRODUCTION;
#endif
            }
            return "";
        }

        // Open Banking Shite
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
//        internal static bool Finance_Lookup_Client_Info(MainViewModel ourviewmodel,
//                                                    FinanceViewModel financeviewmodel,
//                                                    short institution_code,
//                                                    short brand_code)
//        {
//            List<SmartFinance.Brands> brands_found =
//                new List<SmartFinance.Brands>
//                (from Brands in financeviewmodel.PLO.brandsList
//                 where Brands.CUBEFACE_CODE == financeviewmodel.cubeface_code &&
//                         Brands.INSTITUTION_CODE == institution_code &&
//                         Brands.BRAND_CODE == brand_code &&
//                         Brands.ACTIVE_FLAG == SmartParametersV2016.activeFlag
//                 select Brands);
//            if (brands_found.Count > 0)
//            {
//                financeviewmodel.Client_Id = brands_found.First().CLIENT_ID;
//                financeviewmodel.Client_Secret = brands_found.First().CLIENT_SECRET;
//#if PRODUCTION
//                financeviewmodel.Client_Id = brands_found.First().CLIENT_ID;
//                financeviewmodel.Client_Secret = brands_found.First().CLIENT_SECRET;
//#endif
//                return true;
//            }
//            financeviewmodel.Client_Id = "";
//            financeviewmodel.Client_Secret = "";
//            return false;
//        }
//#endif
#endif
    }
}