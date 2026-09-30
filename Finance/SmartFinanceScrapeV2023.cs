//using System.Net.Http;        // No more of this absolute bollocks shit
using System.Reflection;
//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is
using System.Text;

#if WINFORMS
using System.Windows.Forms;
using SmartDashboard;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using System.Windows.Media;
using Microsoft.Web.WebView2.Wpf;
#endif

#if WINUI
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
#endif

#if ANDROIDX
using static Android.Telephony.CarrierConfigManager;
using AndroidX.ViewPager.Widget;
using Android.Content;
using AndroidX.AppCompat.App;
using System.Diagnostics.CodeAnalysis;
using Android.Webkit;
#endif

#if SMARTMAUI

#endif

namespace SmartCubeMobile
{
    public class SmartFinanceScrapeV2023
    {
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        internal static async Task<bool> FinanceSubmitScrape(
#if WINFORMS
                                                    WebView2 FinanceWebView,
                                                    RichTextBox textBoxConsole,
                                                    MainProcess components,
#endif
#if SMARTMAUI
                                                    WebView FinanceWebView,
#endif
#if WPF || WINUI 
                                                    WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                                    WebView FinanceWebView,
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.Logins> completeList = new List<SmartFinance.Logins>();

            // Not sure if this is the right test, but leave it for the time being
            // It wasn't - THIS is the right test!
            if (financeviewmodel.FinanceInstitutionsList.Count > 0)
            {
                // Get everything in an entire list, and THEN see if they need adding to Global!!
#if ANDROIDX
                // Switch to the WebView screen
                // Pretty sure it doesn't scrape unless we do this
                financeviewmodel.financePager2.Post(() => financeviewmodel.financePager2.SetCurrentItem(2, false));
#endif
#if WPF  || WINUI
                if (!FrontEndGUI.GetBorderVisible(ourviewmodel))
                {
                    FrontEndGUI.SetBorderScrollVisible(ourviewmodel);
                }
#endif
                // Now.  Do all Accounts within all Brands
                foreach (FinanceViewModel.InstitutionItem institution_item in financeviewmodel.FinanceInstitutionsList)
                {
#if WINFORMS
                    if (institution_item.Colour == ourviewmodel.greenColour)
#endif
#if WPF
                    // Ray!!! Check all this Color comparison shit actually works  
                    SolidColorBrush newBrush = (SolidColorBrush)ourviewmodel.greenColour;
                    if (institution_item.Colour.ToString() == newBrush.Color.ToString())
#endif
#if WINUI
                    // Ray!!! Check all this Color comparison shit actually works  
                    //SolidColorBrush newBrush = (SolidColorBrush)ourviewmodel.greenColour;
                    if (institution_item.Colour == ourviewmodel.greenColour)
#endif
#if ANDROIDX
                    // Because the Chimps don't have a picker with colour that works - but I fucking DO!!
                    if (institution_item.Colour == ourviewmodel.greenColour)
#endif
                    {
                        short institution_code = 0;
                        short brand_code = 0;
                        SmartFinanceV2025.Decode_Tag_Institution(institution_item.Value.ToString(),
                                                                ref institution_code);
                        // Don't know the Brand Code yet
                        if (institution_code <= 0)
                        {
                            continue;
                        }

                        string institution_name = institution_item.Content;
#if WINFORMS
                        institution_name = institution_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                        // Find the last Logins based on Date
                        List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, financeviewmodel.cubeface_code)
                         join Login in financeviewmodel.PLO.finance_loginsList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Login.CUBEFACE_CODE }
                         // Dont we need Login Method here somewhere???
                         where Login.CUBEFACE_CODE == SmartParametersV2016.Finance &&
                                Login.INSTITUTION_CODE == institution_code &&
                                Login.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                         select Login);

                        // Join Logins with Connections across
                        // USERNAME, CUBEFACE_CODE, INSTITUTION_CODE, BRAND_CODE and LOGIN_METHOD
                        logins_found = new List<SmartFinance.Logins>
                        (from Login in logins_found
                         join Connection in financeviewmodel.PLO.finance_connectionsList
                         on new { Login.USERNAME, Login.CUBEFACE_CODE, Login.INSTITUTION_CODE, Login.BRAND_CODE, Login.LOGIN_METHOD }
                         equals new { Connection.USERNAME, Connection.CUBEFACE_CODE, Connection.INSTITUTION_CODE, Connection.BRAND_CODE, Connection.LOGIN_METHOD }
                         where Connection.ACTIVE_FLAG == SmartParametersV2016.activeFlag
                         select Login).ToList();
                        if (logins_found.Count == 0)
                        {
                            // This is a fatal error because you have an Institution * <= checked
                            // which SHOULD have created the Resource record for it ... but that's not 
                            // there anymore ... so where has it gone??
                            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " Cannot find login for: " + institution_name);
                            
                            // but carry on as there might be others to scrape
                            continue;
                        }

                        List<SmartFinance.Parent> parents = new List<SmartFinance.Parent>();

                        // Should really here get the LATEST logins based on CREATED_DATE
                        foreach (SmartFinance.Logins loginInfo in logins_found)
                        {
#if WINFORMS
                            MainProcess.Output_Message(textBoxConsole, SmartParametersV2016.Finance.ToString() + " Login found: " + institution_name, false, false);
#endif
#if WPF  || WINUI
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, SmartParametersV2016.Finance.ToString() + " Login found: " + institution_name);

                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " Login found: " + institution_name))
                            {
                                return false;
                            }
#endif
#if ANDROIDX
                            await SmartRoutinesV2018.TextBlockUpdate(
                                                             meterActivity,
                                                            ourviewmodel, SmartParametersV2016.Finance.ToString() + " Login found: " + institution_name);
                            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " Login found: " + institution_name);
#endif
                            List<FinanceViewModel.AccountItem> accountsList = new List<FinanceViewModel.AccountItem>();

                            // Now look up the Brand Urls etc, given an inst_code and a brand_code
                            List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                                                                                financeviewmodel,
                                                                                                                institution_code,
                                                                                                                loginInfo.BRAND_CODE);
                            if (brands_found.Count <= 0)
                            {
                                continue;
                            }

                            foreach (SmartFinance.Brands brandInfo in brands_found)
                            {
                                // As of today 7th May 2025, I find it difficult to
                                // believe that anything and everything I scrape
                                // won't require two-factor activation of some kind
                                // i.e. I will need to pick SOMETHING up via my phone
                                if (brandInfo.TWOFACTOR_FLAG.ToString() == SmartParametersV2016.yesFlag)
                                {
                                    brandInfo.TWOFACTOR_FLAG = Convert.ToChar(SmartParametersV2016.yesFlag);
                                }
                                if (brandInfo.TWOFACTOR_FLAG.ToString() == SmartParametersV2016.yesFlag)
                                {
#if WPF  || WINUI
                                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, SmartParametersV2016.Finance.ToString() + " Two-factor in operation");
#endif
#if ANDROIDX
                                    await SmartRoutinesV2018.TextBlockUpdate(
                                                             meterActivity,
                                                            ourviewmodel, SmartParametersV2016.Finance.ToString() + " Two-factor in operation");
#endif
#if WINFORMS
                                    MainProcess.Output_Message(textBoxConsole, SmartParametersV2016.Finance.ToString() + " Two-factor in operation", false, false);
#endif
                                }

                                SmartFinance.Parent parent = new SmartFinance.Parent()
                                {
                                    A = loginInfo,
                                    B = brandInfo,
                                    ApiKeys = new List<SmartFinance.DanApiKey>()
                                };
                                parents.Add(parent);
                            }
                        }
                        // This is bollocks as we (hopefully) never have to use
                        // Selenium. Check it out for Playwright
                        // => Good to go ...chatGPT says create a Selenium WebDriver for EACH website
                        // Well its wrong - just need one for all of them <=
                        
                        
                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.greenColour);
//                        if (!await SmartBanksV2023.FinanceInstitutionalPreLogin(
//#if WINFORMS
//                                                                    FinanceWebView,
//                                                                    components,
//                                                                    textBoxConsole,
//#endif
//#if SMARTMAUI
//                                                                    FinanceWebView,
//#endif
//#if WPF || WINUI 
//                                                                    FinanceWebView,
//#endif
//#if ANDROIDX
//                                                                    meterActivity,
//#endif
//                                                                    ourviewmodel,
//                                                                    financeviewmodel,
//                                                                    parents,
//                                                                    institution_code))
//                        {
//#if WPF  || WINUI
//                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, SmartParametersV2016.Finance.ToString() + " Scrape failed: " + financeviewmodel.errorMessage);
//#endif
//#if ANDROIDX
//                            await SmartRoutinesV2018.TextBlockUpdate(
//                                                             meterActivity,
//                                                            ourviewmodel, SmartParametersV2016.Finance.ToString() + " Scrape failed: " + financeviewmodel.errorMessage);
//#endif
//#if WINFORMS
//                            MainProcess.Output_Message(textBoxConsole, SmartParametersV2016.Finance.ToString() + " Scrape failed: " + financeviewmodel.errorMessage, false, false);
//#endif

//                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " Scrape failed: " + financeviewmodel.errorMessage))
//                            {
//                                return false;
//                            }

//                            // SHouldn't we check for the Cancellation failure here?
//                            // In case we did a 'Cancel' ??? I think so
//                            // Anyhow lets get the Transactions et. al. updating first
//                        }
                        // Well ... we get here on a 'success' of kicking off
                        // one or more finance scrapes ... which in themselves
                        // call a Notification listener and ... then exit!
                        // So if we DON'T update all the tables we need to INSIDE
                        // the Notification event, then its all lost!
                        // Not sure why we need to DisplayFinanceMeterAsync here, tho
                        // What does it do??
                        // Moved it to PostProcess
                    }
                }
                if (completeList.Count == 0)
                {

#if WPF  || WINUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, SmartParametersV2016.Finance.ToString() + " No logins found: " + financeviewmodel.errorMessage);
#endif
#if ANDROIDX
                    await SmartRoutinesV2018.TextBlockUpdate(
                                                             meterActivity,
                                                            ourviewmodel, SmartParametersV2016.Finance.ToString() + " No logins found: " + financeviewmodel.errorMessage);
#endif
#if WINFORMS
                    MainProcess.Output_Message(textBoxConsole, SmartParametersV2016.Finance.ToString() + "No logins found: " + financeviewmodel.errorMessage, false, false);
#endif
                }
#if ANDROIDX
                
                //financeviewmodel.financePager2.Post(() => financeviewmodel.financePager2.SetCurrentItem(0, false));

                //if (financeviewmodel.strategy.GetCurrentItem() != 0)
                //{
                //    financeviewmodel.strategy.SetCurrentItem(0);
                //}
#endif
                // Ha ha - this turns the little black window off!!

                //if (FrontEndGUI.GetBorderVisible(ourviewmodel))
                //{
                //    FrontEndGUI.SetBorderScrollInvisible(ourviewmodel);
                //}
                return true;
            }
            return false;
        }

        internal static async Task<bool> PostProcessing(
#if WINFORMS
                                                        MainProcess components,
#endif
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {
            if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                            SmartParametersV2016.sqliteformat))
            {
                // Tell the console we have added an address
                financeviewmodel.errorMessage = "Cannot add address - cannot continue";
                return false;
            }
            else
            {
                // This SHOULD get all the Inserts and Updates in one fell swoop
                if (!await DoAllSmartFinance(ourviewmodel,
                            financeviewmodel,
                            true,
                            SmartParametersV2016.sqliteformat,
                            false))
                {
#if WPF  || WINUI
                    financeviewmodel.errorMessage = "Cannot update address(1) - cannot continue";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    // This is a bit fatal, because if we can't do this, there's
                    // every chance we can't do lots of things we need to further on
                    return false;
#endif
                }
                try
                {
                    // Assume we are back to the beginning!!!
#if WINFORMS
                    financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
                    financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WPF  || WINUI
                    financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
                    financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
                    financeviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
                    financeviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
                    // Switch to the Transactions screen
                    // Fingers scrossed this works
                    financeviewmodel.financePager2.Post(() => financeviewmodel.financePager2.SetCurrentItem(0, false));
#endif
                    if (!await SmartFinanceV2025.DisplayFinanceMeterAsync(
#if WINFORMS
                                                                            components,
#endif
#if ANDROIDX
                                                                            meterActivity,
#endif
                                                                            ourviewmodel,
                                                                            financeviewmodel,
                                                                            SmartParametersV2016.activeFlag,
                                                                            SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                                financeviewmodel.financeToken.IsCancellationRequested))
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }

                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = ex.Message;
                    return false;
                }
                SmartFinanceV2025.TurnOnFinanceStatus(
#if WINFORMS
                                        components,
#endif
                                        financeviewmodel);
            }
            return true;
        }
#endif

        internal static async Task<bool> DoAllSmartFinance(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                bool full_or_view,
                                                string sqliteformat,
                                                bool delete = false)
        {

            // Now do all the Finance.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express datanase in
            // fucking stupid US "English" format
            //
            bool status;
#if CRYPTOS
            status = true;
#endif
            status = false;
            financeviewmodel.bollocks = new StringBuilder();

            // These can be updated and inserted
            DoAllFinanceCategories(financeviewmodel,
                                    sqliteformat);
            DoAllFinanceCategoryTypes(financeviewmodel,
                                    sqliteformat);

            DoAllEntities<SmartFinance.Accounts, SmartFinance.AccountsSQLite>(
                ourviewmodel,
                financeviewmodel.PLO.finance_accounts_changesList,
                financeviewmodel.PLO.sqlite_finance_accountsList,
                financeviewmodel.bollocks,
                "SmartFinance.Accounts",
                sqliteformat,
                acc => acc.Updated,
                ConvertFinanceAccounts,
                typeof(SmartFinance.AccountsSQLite).GetFields(SmartParametersV2016.bindingFlags)
            );

            //DoAllFinanceAccounts(ourviewmodel,
            //                financeviewmodel,
            //                sqliteformat);   // <= Because data is encrypted inside
            DoAllFinanceConnections(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            delete);   // <= Because data is encrypted inside
            DoAllFinanceLogins(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            delete);   // <= Because data is encrypted inside
            DoAllFinanceSwitches(ourviewmodel,
                            financeviewmodel,
                            sqliteformat);   // <= Because data is encrypted inside
            DoAllFinanceCryptoAccounts(financeviewmodel,
                            sqliteformat);
            DoAllFinanceCryptoAddresses(financeviewmodel,
                            sqliteformat);
            DoAllFinanceCryptoCurrencyRates(financeviewmodel,
                            sqliteformat);
            DoAllFinanceCryptoLedgers(financeviewmodel,
                            sqliteformat);
            //DoAllFinanceCryptoTransactions(ourviewmodel,
            //                financeviewmodel,
            //                sqliteformat);
            DoAllFinanceCryptoWallets(financeviewmodel,
                            sqliteformat);
            DoAllFinanceCryptoWalletTotals(financeviewmodel,
                            sqliteformat);
            DoAllFinanceTransactions(ourviewmodel,
                            financeviewmodel,
                            sqliteformat);
            DoAllFinanceTransactionsCategories(ourviewmodel,
                            financeviewmodel,
                            sqliteformat);
            if (full_or_view)
            {
                // We are taking a chance here that this all works, because the
                // CONSUMER_ENERGY record has either been Created OR updated with its
                // Info ... SO it had better work ...
                //char resource_code = SmartParametersV2016.defaultResourceCode;
                //switch (resource_code)
                //{
                //    case SmartParametersV2016.Banks:
                //        // These can be only be inserted
                //        DoAllBanks_Usage(financeviewmodel, sqliteformat, rf builder);
                //        break;
                //    case SmartParametersV2016.Investments:
                //        // These can be only be inserted
                //        DoAllInvestment_Usage(financeviewmodel, sqliteformat, rf builder);
                //        break;
                //    case SmartParametersV2016.Savings:
                //        // These can be only be inserted
                //        DoAllSavings_Usage(financeviewmodel, sqliteformat, rf builder);
                //        break;
                //    default:
                //        break;
                //}
            }

            // If we haven't anything to send ... then that's good because any inserts or updates won't fail!
            if (financeviewmodel.bollocks.Length == 0 ||
                ourviewmodel.UserNameColour != ourviewmodel.greenColour)
            {
                return true;
            }
            // This might return true or false
            if (!ourviewmodel.localOnly)
            {
                status = await SmartBobV2017.Insert_COMMON_Async(ourviewmodel, financeviewmodel.financeToken, financeviewmodel.bollocks);                
            }
            if (status || ourviewmodel.localOnly)
            {
                // Update the internal DB
                status = await Update_SQLite_Finance(ourviewmodel,
                                                    financeviewmodel,
                                                    SmartParametersV2016.SmartFinanceSchema,
                                                    ourviewmodel.FDEK,
                                                    delete);
            }           
            return status;
        }

        internal static void DoAllEntities<TModel, TSQLite>(
                                    MainViewModel mainViewModel,
                                    List<TModel> changeList,
                                    List<TSQLite> sqliteList,
                                    StringBuilder builder,
                                    string derivedName,
                                    string sqliteFormat,
                                    Func<TModel, bool> isUpdatedFunc,
                                    Func<MainViewModel, TModel, int, TSQLite> convertFunc,
                                    FieldInfo[] fields)
                                    where TModel : class
                                    where TSQLite : class
        {
            sqliteList.Clear();

            if (changeList.Count == 0)
                return;

            ProcessChanges("I", false);
            ProcessChanges("U", true);
            changeList.Clear();

            void ProcessChanges(string operationType, bool updatedFlag)
            {
                string rowAsString = "";

                foreach (var item in changeList)
                {
                    if (isUpdatedFunc(item) != updatedFlag)
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        rowAsString += SmartParametersV2016.recordSeparator;
                    }
                    TSQLite sqliteEntity = convertFunc(mainViewModel, item, derivedName.Length);
                    sqliteList.Add(sqliteEntity);

                    rowAsString += SmartNibbyV2016.Build_StringNew<TSQLite>(fields, sqliteEntity, sqliteFormat);
                }

                if (!string.IsNullOrEmpty(rowAsString))
                {
                    builder.AppendLine(derivedName +
                        SmartParametersV2016.groupSeparator +
                        operationType +
                        SmartParametersV2016.groupSeparator +
                        rowAsString);
                }
                return;
            }
        }

        internal static async Task<bool> Update_SQLite_Finance(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                string schema_name,
                                                                string FDEK,
                                                                bool delete = false)
        {
            // Here we successfully updated the remote SERVER db
            // Now we have to UPDATE the SQLite record already in Profiles
            // or INSERT in the new SQLite record into Profiles
            // We never DELETE records btw
            // But we have to do that with the ENCRYPTED record
            // because WPF.db3 holds ENCRYPTED records

            // Don't really need primary because we only ever store 'ours'
            if (schema_name == "")
            {
                return false;
            }
            
            // Accounts
            if (financeviewmodel.PLO.sqlite_finance_accountsList.Count > 0)
            {
                foreach (SmartFinance.AccountsSQLite accounts_row in financeviewmodel.PLO.sqlite_finance_accountsList)
                {
                    if (accounts_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Accounts",
                                                        accounts_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Accounts",
                                                            accounts_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_accountsList.Clear();
                financeviewmodel.PLO.finance_accountsList = new List<SmartFinance.Accounts>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Accounts",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }
            
            // Categories
            if (financeviewmodel.PLO.sqlite_finance_categoriesList.Count > 0)
            {
                // Should never need 'ToList()' on this if the button logic is working correctly
                foreach (SmartFinance.CategoriesSQLite categories_row in financeviewmodel.PLO.sqlite_finance_categoriesList)
                {
                    if (categories_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Categories",
                                                        categories_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Categories",
                                                            categories_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_categoriesList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.finance_categoriesList = new List<SmartFinance.Categories>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Categories",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            // CategoryTypes <= This NEEDS RE-DOING AS WE UPDATE A FIELD
            // WHICH IS A KEY!!!!  Need an extra field e.g. Description
            if (financeviewmodel.PLO.sqlite_finance_categorytypesList.Count > 0)
            {
                foreach (SmartFinance.CategoryTypesSQLite categorytypes_row in financeviewmodel.PLO.sqlite_finance_categorytypesList)
                {
                    if (categorytypes_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CategoryTypes",
                                                        categorytypes_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CategoryTypes",
                                                            categorytypes_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_categorytypesList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.finance_categorytypesList = new List<SmartFinance.CategoryTypes>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CategoryTypes",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            if (financeviewmodel.PLO.sqlite_crypto_accountsList.Count > 0)
            {
                foreach (SmartFinance.CryptoAccountsSQLite cryptoaccounts_row in financeviewmodel.PLO.sqlite_crypto_accountsList)
                {
                    if (cryptoaccounts_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoAccounts",
                                                        cryptoaccounts_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoAccounts",
                                                            cryptoaccounts_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_accountsList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_accountsList = new List<SmartFinance.CryptoAccounts>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoAccounts",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            if (financeviewmodel.PLO.sqlite_crypto_addressesList.Count > 0)
            {
                foreach (SmartFinance.CryptoAddressesSQLite cryptoaddresses_row in financeviewmodel.PLO.sqlite_crypto_addressesList)
                {
                    if (cryptoaddresses_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoAddresses",
                                                        cryptoaddresses_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoAddresses",
                                                            cryptoaddresses_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_addressesList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_addressesList = new List<SmartFinance.CryptoAddresses>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoAddresses",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            if (financeviewmodel.PLO.sqlite_crypto_currenciesList.Count > 0)
            {
                foreach (SmartFinance.CryptoCurrencyRatesSQLite cryptocurrencies_row in financeviewmodel.PLO.sqlite_crypto_currenciesList)
                {
                    if (cryptocurrencies_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoCurrencyRates",
                                                        cryptocurrencies_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoCurrencyRates",
                                                            cryptocurrencies_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_currenciesList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_currenciesList = new List<SmartFinance.CryptoCurrencyRates>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoCurrencyRates",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            if (financeviewmodel.PLO.sqlite_crypto_ledgersList.Count > 0)
            {
                foreach (SmartFinance.CryptoLedgersSQLite cryptoledgers_row in financeviewmodel.PLO.sqlite_crypto_ledgersList)
                {
                    if (cryptoledgers_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoLedgers",
                                                        cryptoledgers_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoLedgers",
                                                            cryptoledgers_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_ledgersList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_ledgersList = new List<SmartFinance.CryptoLedgers>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoLedgers",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }


            //// CryptoTransactions! We never either Delete nor Update any!
            //// They are ALL DERIVED from each Exchange - they are not
            //// *ours* to either Delete or Update!
            //if (financeviewmodel.PLO.sqlite_crypto_transactionsList.Count > 0)
            //{
            //    foreach (SmartFinance.CryptoTransactionsViewSQLite cryptotransactions_row in financeviewmodel.PLO.sqlite_crypto_transactionsList)
            //    {
            //        if (cryptotransactions_row.Updated)
            //        {
            //            if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
            //                                            schema_name,
            //                                            "CryptoTransactions",
            //                                            cryptotransactions_row))
            //            {
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
            //                                                schema_name,
            //                                                "CryptoTransactions",
            //                                                cryptotransactions_row))
            //            {
            //                return false;
            //            }
            //        }
            //    }

            //    // Clear down all our crimes!
            //    financeviewmodel.PLO.sqlite_crypto_transactionsList.Clear();
            //    // Reload everything from the SQLite tables
            //    financeviewmodel.PLO.crypto_transactionsList = new List<SmartFinance.CryptoTransactionsView>();
            //    if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
            //                                    financeviewmodel,
            //                                    true,//primary,
            //                                    schema_name,
            //                                    "CryptoTransactions",
            //                                    ourviewmodel.UserName,
            //                                    FDEK))
            //    {
            //        return false;
            //    }
            //}

            if (financeviewmodel.PLO.sqlite_crypto_walletsList.Count > 0)
            {
                foreach (SmartFinance.CryptoWalletsSQLite cryptowallets_row in financeviewmodel.PLO.sqlite_crypto_walletsList)
                {
                    if (cryptowallets_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoWallets",
                                                        cryptowallets_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoWallets",
                                                            cryptowallets_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_walletsList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_walletsList = new List<SmartFinance.CryptoWallets>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoWallets",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            if (financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Count > 0)
            {
                foreach (SmartFinance.CryptoWalletTotalsSQLite cryptowallettotals_row in financeviewmodel.PLO.sqlite_crypto_wallettotalsList)
                {
                    if (cryptowallettotals_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "CryptoWalletTotals",
                                                        cryptowallettotals_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "CryptoWalletTotals",
                                                            cryptowallettotals_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.crypto_wallettotalsList = new List<SmartFinance.CryptoWalletTotals>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "CryptoWalletTotals",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }


            // Transactions! We never either Delete nor Update any!
            // Except perhaps in SmartDashboard
            // They are ALL DERIVED from each bank account - they are not
            // *ours* to either Delete or Update!
            if (financeviewmodel.PLO.sqlite_transactionsList.Count > 0)
            {
                foreach (SmartFinance.TransactionsSQLite transactions_row in financeviewmodel.PLO.sqlite_transactionsList)
                {
                    if (transactions_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Transactions",
                                                        transactions_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Transactions",
                                                            transactions_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_transactionsList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.transactionsList = new List<SmartFinance.Transactions>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Transactions",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            // TransactionsCategories! We never either Delete nor Update any!
            // They are ALL DERIVED from each bank account - they are not
            // *ours* to either Delete or Update!
            if (financeviewmodel.PLO.sqlite_transactionscategoriesList.Count > 0)
            {
                foreach (SmartFinance.TransactionsCategoriesSQLite transactioncategories_row in financeviewmodel.PLO.sqlite_transactionscategoriesList)
                {
                    if (transactioncategories_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "TransactionCategories",
                                                        transactioncategories_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "TransactionCategories",
                                                            transactioncategories_row))
                        {
                            return false;
                        }
                    }
                }

                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_transactionscategoriesList.Clear();
                // Reload everything from the SQLite tables
                financeviewmodel.PLO.transactionscategoriesList = new List<SmartFinance.TransactionsCategories>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "TransactionsCategories",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            // Connections
            if (financeviewmodel.PLO.sqlite_finance_connectionsList.Count > 0)
            {
                string sql_operation = "";
                if (delete)
                {
                    // Only tested to do one at a time!!
                    foreach (SmartFinance.ConnectionsSQLite sqlite_connection_row in financeviewmodel.PLO.sqlite_finance_connectionsList)
                    {
                        sql_operation += sql_operation +
                            "DELETE FROM [SmartFinance.Connections]" +
                            " WHERE " +
                            "USERNAME = '" + sqlite_connection_row.USERNAME + "'" +
                            " AND " +
                            "CUBEFACE_CODE = '" + sqlite_connection_row.CUBEFACE_CODE + "'" +
                            " AND " +
                            "INSTITUTION_CODE = " + sqlite_connection_row.INSTITUTION_CODE +
                            " AND " +
                            "BRAND_CODE = " + sqlite_connection_row.BRAND_CODE +
                            " AND " +
                            "LOGIN_METHOD = " + sqlite_connection_row.LOGIN_METHOD +
                            ";";
                    }
                    if (sql_operation.Length > 0)
                    {
                        if (!await SmartPhyllV2020.ExecuteSQLite(ourviewmodel, sql_operation, em => ourviewmodel.errorMessage = em))
                        {
                            return false;
                        }
                    }
                }
                else
                {

                    foreach (SmartFinance.ConnectionsSQLite connections_row in financeviewmodel.PLO.sqlite_finance_connectionsList)
                    {
                        if (connections_row.Updated)
                        {
                            if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Connections",
                                                            connections_row))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                                schema_name,
                                                                "Connections",
                                                                connections_row))
                            {
                                return false;
                            }
                        }
                    }
                }
                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_connectionsList.Clear();
                financeviewmodel.PLO.finance_connectionsList = new List<SmartFinance.Connections>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Connections",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }

            
            // Logins
            if (financeviewmodel.PLO.sqlite_finance_loginsList.Count > 0)
            {
                string sql_operation = "";
                if (delete)
                {
                    // Only tested to do one at a time!!
                    foreach (SmartFinance.LoginsSQLite sqlite_login_row in financeviewmodel.PLO.sqlite_finance_loginsList)
                    {
                        sql_operation += sql_operation +
                            "DELETE FROM [SmartFinance.Logins]" +
                            " WHERE " +
                            "USERNAME = '" + sqlite_login_row.USERNAME + "'" +
                            " AND " +
                            "CUBEFACE_CODE = '" + sqlite_login_row.CUBEFACE_CODE + "'" +
                            " AND " +
                            "INSTITUTION_CODE = " + sqlite_login_row.INSTITUTION_CODE +
                            " AND " +
                            "BRAND_CODE = " + sqlite_login_row.BRAND_CODE +
                            " AND " +
                            "LOGIN_METHOD = " + sqlite_login_row.LOGIN_METHOD +
                            ";";
                    }
                    if (sql_operation.Length > 0)
                    {
                        if (!await SmartPhyllV2020.ExecuteSQLite(ourviewmodel, sql_operation, em => ourviewmodel.errorMessage = em))
                        {
                            return false;
                        }
                    }
                }
                else
                {

                    foreach (SmartFinance.LoginsSQLite logins_row in financeviewmodel.PLO.sqlite_finance_loginsList)
                    {
                        if (logins_row.Updated)
                        {
                            if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Logins",
                                                            logins_row))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                                schema_name,
                                                                "Logins",
                                                                logins_row))
                            {
                                return false;
                            }
                        }
                    }
                }                
                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_loginsList.Clear();
                financeviewmodel.PLO.finance_loginsList = new List<SmartFinance.Logins>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Logins",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }

            }

            // Switches
            if (financeviewmodel.PLO.sqlite_finance_switchesList.Count > 0)
            {
                foreach (SmartFinance.SwitchesSQLite switches_row in financeviewmodel.PLO.sqlite_finance_switchesList)
                {
                    if (switches_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Switches",
                                                        switches_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Switches",
                                                            switches_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                financeviewmodel.PLO.sqlite_finance_switchesList.Clear();
                financeviewmodel.PLO.finance_switchesList = new List<SmartFinance.Switches>();
                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                financeviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Switches",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                FDEK,
                                                false))
                {
                    return false;
                }
            }
            return true;
        }

        internal static void DoAllFinanceAccounts(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    string sqliteformat)
        {
            string deriveds = "SmartFinance.Accounts";
            financeviewmodel.PLO.sqlite_finance_accountsList.Clear(); //Just in case!
            if (financeviewmodel.PLO.finance_accounts_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.AccountsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changesList)
                {
                    if (accounts_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.AccountsSQLite abc = ConvertFinanceAccounts(ourviewmodel, accounts_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_finance_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.AccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changesList)
                {
                    if (accounts_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.AccountsSQLite abc = ConvertFinanceAccounts(ourviewmodel, accounts_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_finance_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.AccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.finance_accounts_changesList.Clear();
            }
            return;
        }


        //internal static SmartFinance.AccountsSQLite ConvertFinanceAccountsX(MainViewModel vm, SmartFinance.Accounts acc,
        //                                                                    int derived)
        //{
        //    var result = new SmartFinance.AccountsSQLite
        //    {
        //        // Map properties manually or with AutoMapper/etc.
        //        //Id = acc.Id,
        //        //Name = acc.Name,
        //        //Balance = acc.Balance,
        //        // ... other fields
        //    };

        //    return result;
        //}

        internal static SmartFinance.AccountsSQLite ConvertFinanceAccounts(MainViewModel ourviewmodel,
                                                                            SmartFinance.Accounts accounts_row,
                                                                            int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = accounts_row.SORTCODE +
                                            SmartParametersV2016.fieldSeparator +
                                            accounts_row.ACCOUNT_NO +
                                            SmartParametersV2016.fieldSeparator +
                                            accounts_row.UDPRN;

            // Time for a new RandomKey1
            accounts_row.RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string key_details = (ourviewmodel.FDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, accounts_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = accounts_row.CURRENCY_ORDINAL.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        accounts_row.ACCOUNT_TITLE +
                                        SmartParametersV2016.fieldSeparator +
                                        accounts_row.ACCOUNT_BALANCE.ToString() +
                                        SmartParametersV2016.fieldSeparator +
                                        accounts_row.STATUS.ToString();
            // Time for a new RandomKey2
            accounts_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.FDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, accounts_row.RANDOMKEY2, derived), "", true));

            SmartFinance.AccountsSQLite accounts_enc = new SmartFinance.AccountsSQLite()
            {
                USERNAME = accounts_row.USERNAME,                       // Key
                CUBEFACE_CODE = accounts_row.CUBEFACE_CODE.ToString(),  // Key
                INSTITUTION_CODE = accounts_row.INSTITUTION_CODE,       // Key
                BRAND_CODE = accounts_row.BRAND_CODE,                   // Key
                KEY_DETAILS = key_details,                              // Key
                ACCOUNT_CREATED = accounts_row.ACCOUNT_CREATED,
                RANDOMKEY1 = accounts_row.RANDOMKEY1,                   // Unchanged
                CATEGORY_CODE = accounts_row.CATEGORY_CODE.ToString(),
                DETAILS = details,
                RANDOMKEY2 = accounts_row.RANDOMKEY2,                   // New!
                Updated = accounts_row.Updated
            };
            return accounts_enc;
        }

        
        internal static void DoAllFinanceCryptoAccounts(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoAccounts";
            financeviewmodel.PLO.sqlite_crypto_accountsList.Clear();
            if (financeviewmodel.PLO.crypto_accounts_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoAccountsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoAccounts generic_row in financeviewmodel.PLO.crypto_accounts_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoAccountsSQLite abc = ConvertFinanceCryptoAccounts(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoAccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Transactions": followed by update_common in SmartDBServer 
                    // for this to work <= Not TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // I'm pretty sure no Crypto Accounts will *ever* be
                // updated, so this section is in here for completeness
                // CAN'T BE SURE IF I ADD IN ASSOCIATIONS
                rowAsString = "";
                foreach (SmartFinance.CryptoAccounts generic_row in financeviewmodel.PLO.crypto_accounts_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoAccountsSQLite abc = ConvertFinanceCryptoAccounts(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_accountsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoAccountsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoAccounts": followed by update_common in SmartDBServer 
                    // for this to work <= NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_accounts_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoAccountsSQLite ConvertFinanceCryptoAccounts(SmartFinance.CryptoAccounts generic_row,
                                                                                        int derived)
        {
            // No encryption needed for Crypto - its all bollocks to start with!
            SmartFinance.CryptoAccountsSQLite generic_enc = new SmartFinance.CryptoAccountsSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = generic_row.INSTITUTION_CODE,
                BRAND_CODE = generic_row.BRAND_CODE,
                SORTCODE = generic_row.SORTCODE,
                ACCOUNT_NO = generic_row.ACCOUNT_NO,
                UDPRN = generic_row.UDPRN,
                //CURRENCY_ORDINAL = Convert.ToInt16(generic_row.CURRENCY_ORDINAL),
                CREATED_AT = Convert.ToDateTime(generic_row.CREATED_AT.ToString(SmartParametersV2016.sqldateFormat)),
                DETAILS = //generic_row.ACTIVE +
                            SmartParametersV2016.dc2 +      // Special for compressed fields
                            generic_row.TYPE +
                            SmartParametersV2016.dc2 +
                            generic_row.AVAILABLE_BALANCE_VALUE +
                            SmartParametersV2016.dc2 +
                            generic_row.AVAILABLE_BALANCE_CURRENCY +
                            SmartParametersV2016.dc2 +
                            generic_row.DEFAULT +
                            SmartParametersV2016.dc2 +
                            generic_row.DELETED_AT.ToString(SmartParametersV2016.sqldateFormat) +
                            SmartParametersV2016.dc2 +
                            generic_row.HOLD_VALUE +
                            SmartParametersV2016.dc2 +
                            generic_row.HOLD_CURRENCY +
                            SmartParametersV2016.dc2 +
                            generic_row.NAME +
                            SmartParametersV2016.dc2 +
                            //generic_row.PLATFORM +
                            SmartParametersV2016.dc2 +
                            generic_row.READY +
                            SmartParametersV2016.dc2 +
                            generic_row.RETAIL_PORTFOLIO_ID +
                            SmartParametersV2016.dc2 +
                            generic_row.UPDATED_AT.ToString(SmartParametersV2016.sqldateFormat),
                Updated = generic_row.Updated
            };
            return generic_enc;
        }

        internal static void DoAllFinanceCryptoAddresses(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoAddresses";
            financeviewmodel.PLO.sqlite_crypto_addressesList.Clear();
            if (financeviewmodel.PLO.crypto_addresses_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoAddressesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoAddresses generic_row in financeviewmodel.PLO.crypto_addresses_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoAddressesSQLite abc = ConvertFinanceCryptoAddresses(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_addressesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoAddressesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoAddresses": followed by update_common in SmartDBServer 
                    // for this to work <= Not TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // I'm pretty sure no Crypto Addresses will *ever* be
                // updated, so this section is in here for completeness
                // THAT IS NOT TRUE IF we ARE GOING TO ADD ASSOCIATIONS TO THEM!!
                rowAsString = "";
                foreach (SmartFinance.CryptoAddresses generic_row in financeviewmodel.PLO.crypto_addresses_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoAddressesSQLite abc = ConvertFinanceCryptoAddresses(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_addressesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoAddressesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoAddresses": followed by update_common in SmartDBServer 
                    // for this to work <= NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_addresses_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoAddressesSQLite ConvertFinanceCryptoAddresses(SmartFinance.CryptoAddresses generic_row,
                                                                                        int derived)
        {
            // No encryption needed for Crypto - its all bollocks to start with!
            SmartFinance.CryptoAddressesSQLite generic_enc = new SmartFinance.CryptoAddressesSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = generic_row.INSTITUTION_CODE,
                BRAND_CODE = generic_row.BRAND_CODE,
                SORTCODE = generic_row.SORTCODE,
                ACCOUNT_NO = generic_row.ACCOUNT_NO,
                UDPRN = generic_row.UDPRN,
                //CURRENCY_ORDINAL = generic_row.CURRENCY_ORDINAL,
                DETAILS = generic_row.ADDRESS +
                        SmartParametersV2016.dc2 +
                        generic_row.ADDRESS_INFO_ADDRESS +
                        SmartParametersV2016.dc2 +
                        generic_row.ADDRESS_LABEL +
                        SmartParametersV2016.dc2 +
                        generic_row.CALLBACK_URL +
                        SmartParametersV2016.dc2 +
                        generic_row.CREATED_AT.ToString(SmartParametersV2016.sqldateFormat) +
                        SmartParametersV2016.dc2 +
                        generic_row.DEFAULT_RECEIVE +
                        SmartParametersV2016.dc2 +
                        generic_row.DEPOSIT_URI +
                        SmartParametersV2016.dc2 +
                        generic_row.DESTINATION_TAG +
                        SmartParametersV2016.dc2 +
                        generic_row.ID +
                        SmartParametersV2016.dc2 +
                        generic_row.INLINE_WARNING_TEXT +
                        SmartParametersV2016.dc2 +
                        generic_row.INLINE_WARNING_TOOLTIP +
                        SmartParametersV2016.dc2 +
                        generic_row.NAME +
                        SmartParametersV2016.dc2 +
                        generic_row.NETWORK +
                        SmartParametersV2016.dc2 +
                        generic_row.QR_CODE_IMAGE_URL +
                        SmartParametersV2016.dc2 +
                        generic_row.RECEIVE_SUBTITLE +
                        SmartParametersV2016.dc2 +
                        generic_row.RESOURCE +
                        SmartParametersV2016.dc2 +
                        generic_row.RESOURCE_PATH +
                        SmartParametersV2016.dc2 +
                        generic_row.SHARE_ADDRESS_COPY_LINE1 +
                        SmartParametersV2016.dc2 +
                        generic_row.SHARE_ADDRESS_COPY_LINE2 +
                        SmartParametersV2016.dc2 +
                        generic_row.UPDATED_AT.ToString(SmartParametersV2016.sqldateFormat) +
                        SmartParametersV2016.dc2 +
                        generic_row.URI_SCHEME,

                Updated = generic_row.Updated
            };
            return generic_enc;
        }

        internal static void DoAllFinanceCryptoCurrencyRates(FinanceViewModel financeviewmodel,
                                                            string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoCurrencyRates";
            financeviewmodel.PLO.sqlite_crypto_currenciesList.Clear();
            if (financeviewmodel.PLO.crypto_currencies_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoCurrencyRatesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoCurrencyRates generic_row in financeviewmodel.PLO.crypto_currencies_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoCurrencyRatesSQLite abc = ConvertFinanceCryptoCurrencyRates(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_currenciesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoCurrencyRatesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoCurrencyRates": followed by update_common in SmartDBServer 
                    // for this to work <= Not TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // I'm pretty sure Crypto Currencies will *never* be
                // updated, so this section is in here for completeness
                // 
                rowAsString = "";
                foreach (SmartFinance.CryptoCurrencyRates generic_row in financeviewmodel.PLO.crypto_currencies_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoCurrencyRatesSQLite abc = ConvertFinanceCryptoCurrencyRates(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_currenciesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoCurrencyRatesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoCurrencyRates": followed by update_common in SmartDBServer 
                    // for this to work <= NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_currencies_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoCurrencyRatesSQLite ConvertFinanceCryptoCurrencyRates(SmartFinance.CryptoCurrencyRates generic_row,
                                                                                                int derived)
        {
            // No encryption needed for Crypto - its all bollocks to start with!
            SmartFinance.CryptoCurrencyRatesSQLite generic_enc = new SmartFinance.CryptoCurrencyRatesSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                CRYPTO_ORDINAL = generic_row.CRYPTO_ORDINAL,
                CRYPTO_RATE = generic_row.CRYPTO_RATE,
                CRYPTO_RATE_DATE = generic_row.CRYPTO_RATE_DATE,
                Updated = generic_row.Updated
            };
            return generic_enc;
        }

        internal static void DoAllFinanceCryptoLedgers(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoLedgers";
            financeviewmodel.PLO.sqlite_crypto_ledgersList.Clear();
            if (financeviewmodel.PLO.crypto_ledgers_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoLedgersSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoLedgers generic_row in financeviewmodel.PLO.crypto_ledgers_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoLedgersSQLite abc = ConvertFinanceCryptoLedgers(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_ledgersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoLedgersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoLedgers": followed by update_common in SmartDBServer 
                    // for this to work  NOT TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // This section is in here for completeness
                rowAsString = "";
                foreach (SmartFinance.CryptoLedgers generic_row in financeviewmodel.PLO.crypto_ledgers_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoLedgersSQLite abc = ConvertFinanceCryptoLedgers(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_ledgersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoLedgersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoLedgers": followed by update_common in SmartDBServer 
                    // for this to work NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_ledgers_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoLedgersSQLite ConvertFinanceCryptoLedgers(SmartFinance.CryptoLedgers generic_row,
                                                                                    int derived)
        {

            SmartFinance.CryptoLedgersSQLite generic_enc = new SmartFinance.CryptoLedgersSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = generic_row.INSTITUTION_CODE,
                BRAND_CODE = generic_row.BRAND_CODE,
                LEDGER_NAME = generic_row.LEDGER_NAME,
                WALLET = generic_row.WALLET,
                ACCOUNT = generic_row.ACCOUNT,
                TRANSACTION_DATE = generic_row.TRANSACTION_DATE,
                AMOUNT = generic_row.AMOUNT,
                SOURCE = generic_row.SOURCE,
                DESTINATION = generic_row.DESTINATION,
                CURRENCY_DISPLAY = generic_row.CURRENCY_DISPLAY,
                FEE = generic_row.FEE,
                TRANSACTION_TYPE = generic_row.TRANSACTION_TYPE,
                TO_ADDRESS = generic_row.TO_ADDRESS,
                NETWORK_NAME = generic_row.NETWORK_NAME,
                HASH = generic_row.HASH,
                Updated = generic_row.Updated
            };
            return generic_enc;
        }

        // With Transactions there SHOULD never be any changes as we are SUPPOSED
        // to append them sequentially ... However!! This fucking program - being
        // what it is - its EASIER to leave the 'changed' code in now and remove it
        // in 5 years time, than to leave it out now and re-insert it in 5 years ...

        // Well... that is extremely prescient of you Ray, but I'm still going to
        // comment it out for the time being 
        //internal static void DoAllFinanceCryptoTransactions(MainViewModel ourviewmodel,
        //                                FinanceViewModel financeviewmodel,
        //                                string sqliteformat)
        //{
        //    string deriveds = "SmartFinance.CryptoTransactions";
        //    financeviewmodel.PLO.sqlite_crypto_transactionsList.Clear();
        //    if (financeviewmodel.PLO.crypto_transactions_changesList.Count > 0)
        //    {
        //        FieldInfo[] myFields = typeof(SmartFinance.CryptoTransactionsViewSQLite).GetFields(SmartParametersV2016.bindingFlags);

        //        string rowAsString = "";
        //        foreach (SmartFinance.CryptoTransactionsView transactions_row in financeviewmodel.PLO.crypto_transactions_changesList)
        //        {
        //            if (transactions_row.Updated == false)
        //            {
        //                if (!string.IsNullOrEmpty(rowAsString))
        //                {
        //                    // Separate each record line with this
        //                    rowAsString += SmartParametersV2016.recordSeparator;
        //                }
        //                SmartFinance.CryptoTransactionsViewSQLite abc = ConvertFinanceCryptoTransactions(ourviewmodel, transactions_row, deriveds.Length);
        //                financeviewmodel.PLO.sqlite_crypto_transactionsList.Add(abc);
        //                rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoTransactionsViewSQLite>(myFields, abc, sqliteformat);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(rowAsString))
        //        {
        //            // You have to have a case "CryptoTransactions": followed by update_common in SmartDBServer 
        //            // for this to work  NOT TRUE ANY MORE
        //            financeviewmodel.bollocks.AppendLine(deriveds +
        //                                    SmartParametersV2016.groupSeparator +
        //                                    "I" + SmartParametersV2016.groupSeparator +
        //                                    rowAsString);
        //        }

        //        // I'm pretty sure no CryptoTransactions will *ever* be
        //        // updated, so this section is in here for completeness
        //        rowAsString = "";
        //        foreach (SmartFinance.CryptoTransactionsView transactions_row in financeviewmodel.PLO.crypto_transactions_changesList)
        //        {
        //            if (transactions_row.Updated == true)
        //            {
        //                if (!string.IsNullOrEmpty(rowAsString))
        //                {
        //                    // Separate each record line with this
        //                    rowAsString += SmartParametersV2016.recordSeparator;
        //                }
        //                SmartFinance.CryptoTransactionsViewSQLite abc = ConvertFinanceCryptoTransactions(ourviewmodel, transactions_row, deriveds.Length);
        //                financeviewmodel.PLO.sqlite_crypto_transactionsList.Add(abc);
        //                rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoTransactionsViewSQLite>(myFields, abc, sqliteformat);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(rowAsString))
        //        {
        //            // You have to have a case "CryptoTransactions": followed by update_common in SmartDBServer 
        //            // for this to work NOT TRUE ANYMORE
        //            financeviewmodel.bollocks.AppendLine(deriveds +
        //                                    SmartParametersV2016.groupSeparator +
        //                                    "U" + SmartParametersV2016.groupSeparator +
        //                                    rowAsString);
        //        }
        //        financeviewmodel.PLO.crypto_transactions_changesList.Clear();
        //    }
        //    return;
        //}

        //internal static SmartFinance.CryptoTransactionsViewSQLite ConvertFinanceCryptoTransactions(MainViewModel ourviewmodel,
        //                                                        SmartFinance.CryptoTransactionsView transactions_row,
        //                                                        int derived)
        //{
        //    // Encrypt all of these into DETAILS with randomkey
        //    string key_details_unencrypted = transactions_row.EXCHANGE +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.UUID +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.UDPRN +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CREATED_AT.ToString(SmartParametersV2016.sqldateFormat);

        //    string details_unencrypted = transactions_row.TYPE +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CRYPTO_CURRENCY +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CRYPTO_CURRENCY_ORDINAL +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CRYPTO_AMOUNT +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CRYPTO_RATE +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CURRENCY_DISPLAY +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.CREDITDEBIT_INDICATOR +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.NATIVE_AMOUNT +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.NATIVE_CURRENCY +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.FEE_AMOUNT +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.FEE_CURRENCY +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.TOTAL_COST +
        //                        SmartParametersV2016.fieldSeparator +
        //                        transactions_row.TO_ADDRESS;
        //    // STRICTLY SPEAKING we (I) should use a different random number here .. but
        //    // I will cross that security bridge when (if) I ever come to it!!
        //    // Time for a new RandomKey2
        //    transactions_row.RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
        //    string key_details = (ourviewmodel.FDEK == "" ?
        //        key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, transactions_row.RANDOMKEY1, derived), "", true));

        //    // Time for a new RandomKey2
        //    transactions_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
        //    string details = (ourviewmodel.FDEK == "" ?
        //        details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, transactions_row.RANDOMKEY2, derived), "", true));

        //    SmartFinance.CryptoTransactionsViewSQLite transactions_enc = new SmartFinance.CryptoTransactionsViewSQLite()
        //    {
        //        USERNAME = transactions_row.USERNAME,
        //        CUBEFACE_CODE = transactions_row.CUBEFACE_CODE.ToString(),
        //        INSTITUTION_CODE = transactions_row.INSTITUTION_CODE,
        //        BRAND_CODE = transactions_row.BRAND_CODE,
        //        KEY_DETAILS = key_details,
        //        SEQUENCE_NO = transactions_row.SEQUENCE_NO,
        //        RANDOMKEY1 = transactions_row.RANDOMKEY1,
        //        DETAILS = details,
        //        RANDOMKEY2 = transactions_row.RANDOMKEY2,
        //        Updated = transactions_row.Updated
        //    };
        //    return transactions_enc;
        //}

        internal static void DoAllFinanceCryptoWallets(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoWallets";
            financeviewmodel.PLO.sqlite_crypto_walletsList.Clear();
            if (financeviewmodel.PLO.crypto_wallets_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoWalletsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoWallets generic_row in financeviewmodel.PLO.crypto_wallets_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoWalletsSQLite abc = ConvertFinanceCryptoWallets(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_walletsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoWalletsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoWallets": followed by update_common in SmartDBServer 
                    // for this to work  NOT TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // This section is in here for completeness
                rowAsString = "";
                foreach (SmartFinance.CryptoWallets generic_row in financeviewmodel.PLO.crypto_wallets_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoWalletsSQLite abc = ConvertFinanceCryptoWallets(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_walletsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoWalletsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoWallets": followed by update_common in SmartDBServer 
                    // for this to work NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_wallets_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoWalletsSQLite ConvertFinanceCryptoWallets(SmartFinance.CryptoWallets generic_row,
                                                                int derived)
        {
            SmartFinance.CryptoWalletsSQLite generic_enc = new SmartFinance.CryptoWalletsSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                CRYPTO_DESTINATION = generic_row.CRYPTO_DESTINATION,
                CRYPTO_NAME = generic_row.CRYPTO_NAME,
                BALANCE = generic_row.BALANCE,
                Updated = generic_row.Updated
            };
            return generic_enc;
        }

        internal static void DoAllFinanceCryptoWalletTotals(FinanceViewModel financeviewmodel,
                                                            string sqliteformat)
        {
            string deriveds = "SmartFinance.CryptoWalletTotals";
            financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Clear();
            if (financeviewmodel.PLO.crypto_wallettotals_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CryptoWalletTotalsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CryptoWalletTotals generic_row in financeviewmodel.PLO.crypto_wallettotals_changesList)
                {
                    if (generic_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoWalletTotalsSQLite abc = ConvertFinanceCryptoWalletTotals(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoWalletTotalsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoWalletTotals": followed by update_common in SmartDBServer 
                    // for this to work  NOT TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // This section is in here for completeness
                rowAsString = "";
                foreach (SmartFinance.CryptoWalletTotals generic_row in financeviewmodel.PLO.crypto_wallettotals_changesList)
                {
                    if (generic_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CryptoWalletTotalsSQLite abc = ConvertFinanceCryptoWalletTotals(generic_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CryptoWalletTotalsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CryptoWalletTotals": followed by update_common in SmartDBServer 
                    // for this to work NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.crypto_wallettotals_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CryptoWalletTotalsSQLite ConvertFinanceCryptoWalletTotals(SmartFinance.CryptoWalletTotals generic_row,
                                                                                                int derived)
        {
            SmartFinance.CryptoWalletTotalsSQLite generic_enc = new SmartFinance.CryptoWalletTotalsSQLite()
            {
                USERNAME = generic_row.USERNAME,
                CUBEFACE_CODE = generic_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = generic_row.INSTITUTION_CODE,
                BRAND_CODE = generic_row.BRAND_CODE,
                ADDRESS = generic_row.ADDRESS,
                CRYPTO_WALLET_NAME = generic_row.CRYPTO_WALLET_NAME,
                CRYPTO_CURRENCY_ORDINAL = generic_row.CRYPTO_CURRENCY_ORDINAL,
                CRYPTO_CURRENCY = generic_row.CRYPTO_CURRENCY,
                CRYPTO_AMOUNT = generic_row.CRYPTO_AMOUNT,
                RATE = generic_row.RATE,
                VALUE = generic_row.VALUE,
                Updated = generic_row.Updated
            };
            return generic_enc;
        }
        internal static void DoAllFinanceTransactions(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.Transactions";
            financeviewmodel.PLO.sqlite_transactionsList.Clear();
            if (financeviewmodel.PLO.transactions_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.TransactionsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.Transactions transactions_row in financeviewmodel.PLO.transactions_changesList)
                {
                    if (transactions_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.TransactionsSQLite abc = ConvertFinanceTransactions(ourviewmodel, transactions_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_transactionsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.TransactionsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Transactions": followed by update_common in SmartDBServer 
                    // for this to work <= Not TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // I'm pretty sure no Transactions will *ever* be
                // updated, so this section is in here for completeness
                rowAsString = "";
                foreach (SmartFinance.Transactions transactions_row in financeviewmodel.PLO.transactions_changesList)
                {
                    if (transactions_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.TransactionsSQLite abc = ConvertFinanceTransactions(ourviewmodel, transactions_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_transactionsList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.TransactionsSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Transactions": followed by update_common in SmartDBServer 
                    // for this to work <= NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.transactions_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.TransactionsSQLite ConvertFinanceTransactions(MainViewModel ourviewmodel,
                                                                SmartFinance.Transactions transactions_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = transactions_row.SORTCODE +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.ACCOUNT_NO +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.UDPRN +   // Surely *NOT* a key?? Yep sure is
                                                           //SmartParametersV2016.fieldSeparator +
                                                           //transactions_row.CURRENCY_ORDINAL +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.STATEMENT_DATE +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.STATEMENT_NO;

            // STRICTLY SPEAKING we (I) should use a different random number here .. but
            // I will cross that security bridge when (if) I ever come to it!!
            string key_details = (ourviewmodel.FDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, transactions_row.RANDOMKEY1, derived), "", true));
            SmartFinance.TransactionsSQLite transactions_enc = new SmartFinance.TransactionsSQLite()
            {
                USERNAME = transactions_row.USERNAME,
                CUBEFACE_CODE = transactions_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = transactions_row.INSTITUTION_CODE,
                BRAND_CODE = transactions_row.BRAND_CODE,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = transactions_row.RANDOMKEY1,
                Updated = transactions_row.Updated
            };
            return transactions_enc;
        }

        // With Transactions there SHOULD never be any changes as we are SUPPOSED
        // to append them sequentially ... However!! This fucking program - being
        // what it is - its EASIER to leave the 'changed' code in now and remove it
        // in 5 years time, than to leave it out now and re-insert it in 5 years ...

        // Well... that is extremely prescient of you Ray, but I'm still going to
        // comment it out for the time being 
        internal static void DoAllFinanceTransactionsCategories(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.TransactionsCategories";
            financeviewmodel.PLO.sqlite_transactionscategoriesList.Clear();
            if (financeviewmodel.PLO.transactionscategories_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.TransactionsCategoriesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.TransactionsCategories transactions_row in financeviewmodel.PLO.transactionscategories_changesList)
                {
                    if (transactions_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.TransactionsCategoriesSQLite abc = ConvertFinanceTransactionsCategories(ourviewmodel, transactions_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_transactionscategoriesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.TransactionsCategoriesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "TransactionsCategories": followed by update_common in SmartDBServer 
                    // for this to work  NOT TRUE ANY MORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // I'm pretty sure no Bank Transactions will *ever* be
                // updated, so this section is in here for completeness
                rowAsString = "";
                foreach (SmartFinance.TransactionsCategories transactions_row in financeviewmodel.PLO.transactionscategories_changesList)
                {
                    if (transactions_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.TransactionsCategoriesSQLite abc = ConvertFinanceTransactionsCategories(ourviewmodel, transactions_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_transactionscategoriesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.TransactionsCategoriesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "TransactionsCategories": followed by update_common in SmartDBServer 
                    // for this to work NOT TRUE ANYMORE
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.transactionscategories_changesList.Clear();
            }
            return;
        }
        internal static SmartFinance.TransactionsCategoriesSQLite ConvertFinanceTransactionsCategories(MainViewModel ourviewmodel,
                                                                SmartFinance.TransactionsCategories transactions_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = transactions_row.SORTCODE +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.ACCOUNT_NO +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.UDPRN +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.STATEMENT_DATE +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.STATEMENT_NO;

            string details_unencrypted = transactions_row.TRANSACTION_DATE.ToString(SmartParametersV2016.sqliteformat) +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.CREDITDEBIT_INDICATOR.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.PTC_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.TRANSGROUP_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.TRANSACTION_CODE.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.DESCRIPTION +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.TYPE +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.CRYPTO_AMOUNT.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.CRYPTO_CURRENCY_ORDINAL +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.AMOUNT.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.AMOUNT_CURRENCY_ORDINAL +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.BALANCE_AMOUNT.ToString() +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.BALANCE_CURRENCY_ORDINAL +
                                SmartParametersV2016.fieldSeparator +
                                transactions_row.BALANCE_CREDITDEBIT_INDICATOR.ToString();

            // STRICTLY SPEAKING we (I) should use a different random number here .. but
            // I will cross that security bridge when (if) I ever come to it!!
            string key_details = (ourviewmodel.FDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, transactions_row.RANDOMKEY1, derived), "", true));

            // Time for a new RandomKey2
            transactions_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.FDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, transactions_row.RANDOMKEY2, derived), "", true));

            SmartFinance.TransactionsCategoriesSQLite transactions_enc = new SmartFinance.TransactionsCategoriesSQLite()
            {
                USERNAME = transactions_row.USERNAME,
                CUBEFACE_CODE = transactions_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = transactions_row.INSTITUTION_CODE,
                BRAND_CODE = transactions_row.BRAND_CODE,
                KEY_DETAILS = key_details,
                SEQUENCE_NO = transactions_row.SEQUENCE_NO,
                RANDOMKEY1 = transactions_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = transactions_row.RANDOMKEY2,
                Updated = transactions_row.Updated
            };
            return transactions_enc;
        }


        internal static void DoAllFinanceCategories(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.Categories";
            financeviewmodel.PLO.sqlite_finance_categoriesList.Clear(); // Just in case
            if (financeviewmodel.PLO.finance_categories_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.Categories).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changesList)
                {
                    if (categories_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CategoriesSQLite abc = ConvertFinanceCategories(categories_row);
                        financeviewmodel.PLO.sqlite_finance_categoriesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.Categories>(myFields, categories_row, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Categories": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changesList)
                {
                    if (categories_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CategoriesSQLite abc = ConvertFinanceCategories(categories_row);
                        financeviewmodel.PLO.sqlite_finance_categoriesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.Categories>(myFields, categories_row, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Categories": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.finance_categories_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CategoriesSQLite ConvertFinanceCategories(SmartFinance.Categories categories_row)
        {
            SmartFinance.CategoriesSQLite categories_enc = new SmartFinance.CategoriesSQLite()
            {
                USERNAME = categories_row.USERNAME,
                CUBEFACE_CODE = categories_row.CUBEFACE_CODE.ToString(),
                CATEGORY_CODE = categories_row.CATEGORY_CODE.ToString(),
                CHECKED = categories_row.CHECKED.ToString(),
                Updated = categories_row.Updated
            };
            return categories_enc;
        }

        internal static void DoAllFinanceCategoryTypes(FinanceViewModel financeviewmodel,
                                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.CategoryTypes";
            financeviewmodel.PLO.sqlite_finance_categorytypesList.Clear();
            if (financeviewmodel.PLO.finance_categorytypes_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.CategoryTypes).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.CategoryTypes categorytypes_row in financeviewmodel.PLO.finance_categorytypes_changesList)
                {
                    if (categorytypes_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CategoryTypesSQLite abc = ConvertFinanceCategoryTypes(categorytypes_row);
                        financeviewmodel.PLO.sqlite_finance_categorytypesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CategoryTypes>(myFields, categorytypes_row, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CategoryTypes": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartFinance.CategoryTypes categorytypes_row in financeviewmodel.PLO.finance_categorytypes_changesList)
                {
                    if (categorytypes_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.CategoryTypesSQLite abc = ConvertFinanceCategoryTypes(categorytypes_row);
                        financeviewmodel.PLO.sqlite_finance_categorytypesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.CategoryTypes>(myFields, categorytypes_row, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "CategoryTypes": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.finance_categorytypes_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.CategoryTypesSQLite ConvertFinanceCategoryTypes(SmartFinance.CategoryTypes categorytypes_row)
        {
            SmartFinance.CategoryTypesSQLite categorytypes_enc = new SmartFinance.CategoryTypesSQLite()
            {
                USERNAME = categorytypes_row.USERNAME,
                CUBEFACE_CODE = categorytypes_row.CUBEFACE_CODE.ToString(),
                CATEGORY_CODE = categorytypes_row.CATEGORY_CODE.ToString(),
                CURRENCY_ORDINAL = Convert.ToInt16(categorytypes_row.CURRENCY_ORDINAL),
                Updated = categorytypes_row.Updated
            };
            return categorytypes_enc;
        }

        internal static void DoAllFinanceConnections(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string sqliteformat,
                                            bool deleteFlag = false) // When true, treat Update = true as 'Delete'
        {
            string deriveds = "SmartFinance.Connections";

            if (deleteFlag)
            {
                // Now, smarty pants ....delete this 'one' record
                if (financeviewmodel.PLO.finance_connections_changesList.Count > 0)
                {
                    // Delete all Groups
                    FieldInfo[] myFields = typeof(SmartFinance.ConnectionsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartFinance.Connections connection_row in financeviewmodel.PLO.finance_connections_changesList) // Checked
                    {
                        if (connection_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.ConnectionsSQLite abc = ConvertFinanceConnections(ourviewmodel, connection_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_connectionsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.ConnectionsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Connections" +
                                                SmartParametersV2016.groupSeparator +
                                                "D" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                }
                financeviewmodel.PLO.finance_connections_changesList.Clear();
            }
            else
            {
                
                financeviewmodel.PLO.sqlite_finance_connectionsList.Clear();
                if (financeviewmodel.PLO.finance_connections_changesList.Count > 0)
                {
                    FieldInfo[] myFields = typeof(SmartFinance.ConnectionsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartFinance.Connections connection_row in financeviewmodel.PLO.finance_connections_changesList) // Checked
                    {
                        if (connection_row.Updated == false)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.ConnectionsSQLite abc = ConvertFinanceConnections(ourviewmodel, connection_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_connectionsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.ConnectionsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Connections" +
                                                SmartParametersV2016.groupSeparator +
                                                "I" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }

                    rowAsString = "";
                    foreach (SmartFinance.Connections connection_row in financeviewmodel.PLO.finance_connections_changesList)
                    {
                        // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                        if (connection_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.ConnectionsSQLite abc = ConvertFinanceConnections(ourviewmodel, connection_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_connectionsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.ConnectionsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Connections" +
                                                SmartParametersV2016.groupSeparator +
                                                "U" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                    financeviewmodel.PLO.finance_connections_changesList.Clear();
                }
            }
            return;
        }

        
        internal static SmartFinance.ConnectionsSQLite ConvertFinanceConnections(MainViewModel ourviewmodel,
                                                                SmartFinance.Connections connections_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = connections_row.Parameter1 +
                                SmartParametersV2016.fieldSeparator +
                                connections_row.Parameter2 +
                                SmartParametersV2016.fieldSeparator +
                                connections_row.Parameter3;

            // Time for a new RandomKey
            string details = (ourviewmodel.FDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, connections_row.RANDOMKEY, derived), "", true));

            SmartFinance.ConnectionsSQLite connections_enc = new SmartFinance.ConnectionsSQLite()
            {
                USERNAME = connections_row.USERNAME,
                CUBEFACE_CODE = connections_row.CUBEFACE_CODE.ToString(),   // KEY
                INSTITUTION_CODE = connections_row.INSTITUTION_CODE,        // KEY
                BRAND_CODE = connections_row.BRAND_CODE,        // KEY
                LOGIN_METHOD = connections_row.LOGIN_METHOD,              // KEY in Connections
                ACTIVE_FLAG = connections_row.ACTIVE_FLAG.ToString(),
                DETAILS = details,
                RANDOMKEY = connections_row.RANDOMKEY,
                Updated = connections_row.Updated,
                Delete = connections_row.Delete
            };
            return connections_enc;
        }
        // You don't understand your own logic ....
        // Finance_Logins_ChangesList contains BOTH Inserts AND Updates in ONE list
        // only differentiated by the value of 'Update'.  If 'Update' is False
        // then its an INSERT, if 'Update' is True then its an UPDATE.
        // At the end of this routine ... Finance_Logins_ChangesList will be EMPTY
        // as all the Inserts and/or Updates will have been fed into Insert_Common.
        //
        // But how do you get those Inserts and/or Updates reflected into
        // the current Finance_LoginsList?  Watch this space! Genius at work .....

        internal static void DoAllFinanceLogins(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string sqliteformat,
                                            bool deleteFlag = false) // When true, treat Update = true as 'Delete'
        {
            string deriveds = "SmartFinance.Logins";

            if (deleteFlag)
            {
                // Now, smarty pants ....delete this 'one' record
                if (financeviewmodel.PLO.finance_logins_changesList.Count > 0)
                {
                    // Delete all Groups
                    FieldInfo[] myFields = typeof(SmartFinance.LoginsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartFinance.Logins login_row in financeviewmodel.PLO.finance_logins_changesList) // Checked
                    {
                        if (login_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.LoginsSQLite abc = ConvertFinanceLogins(ourviewmodel, login_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_loginsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.LoginsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Logins" +
                                                SmartParametersV2016.groupSeparator +
                                                "D" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                }
                financeviewmodel.PLO.finance_logins_changesList.Clear();
            }
            else
            {

                financeviewmodel.PLO.sqlite_finance_loginsList.Clear();
                if (financeviewmodel.PLO.finance_logins_changesList.Count > 0)
                {
                    FieldInfo[] myFields = typeof(SmartFinance.LoginsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartFinance.Logins login_row in financeviewmodel.PLO.finance_logins_changesList) // Checked
                    {
                        if (login_row.Updated == false)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.LoginsSQLite abc = ConvertFinanceLogins(ourviewmodel, login_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_loginsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.LoginsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Logins" +
                                                SmartParametersV2016.groupSeparator +
                                                "I" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }

                    rowAsString = "";
                    foreach (SmartFinance.Logins login_row in financeviewmodel.PLO.finance_logins_changesList)
                    {
                        // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                        if (login_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartFinance.LoginsSQLite abc = ConvertFinanceLogins(ourviewmodel, login_row, deriveds.Length);
                            financeviewmodel.PLO.sqlite_finance_loginsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.LoginsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        financeviewmodel.bollocks.AppendLine("SmartFinance.Logins" +
                                                SmartParametersV2016.groupSeparator +
                                                "U" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                    financeviewmodel.PLO.finance_logins_changesList.Clear();
                }
            }
            return;
        }

        //internal static void DoAllFinance_LoginsX(MainViewModel ourviewmodel,
        //                                    FinanceViewModel financeviewmodel,
        //                                    string sqliteformat)
        //{
        //    string deriveds = "SmartFinance.Logins";
        //    financeviewmodel.PLO.sqlite_finance_loginsList.Clear();
        //    if (financeviewmodel.PLO.finance_logins_changesList.Count > 0)
        //    {

        //        FieldInfo[] myFields = typeof(SmartFinance.LoginsSQLite).GetFields(SmartParametersV2016.bindingFlags);

        //        string rowAsString = "";

        //        foreach (SmartFinance.Logins logins in financeviewmodel.PLO.finance_logins_changesList)
        //        {
        //            if (logins.Delete)
        //            {
        //                // We are only expecting ONE delete at a time
        //                SmartFinance.LoginsSQLite logins_enc = ConvertFinanceLogins(ourviewmodel, logins, deriveds.Length);

        //                string sql = "DELETE FROM " + "Logins" +          // Same as above - might need a schema if not added auto ..think it will ALWAYS need a schema!
        //                            " WHERE USERNAME = '" + logins_enc.USERNAME + "'" +
        //                            " AND CUBEFACE_CODE = '" + logins_enc.CUBEFACE_CODE + "'" +
        //                            " AND INSTITUTION_CODE = " + logins_enc.INSTITUTION_CODE +
        //                            " AND BRAND_CODE = " + logins_enc.BRAND_CODE +
        //                            " AND LOGIN_CREATED = '" + logins_enc.LOGIN_CREATED.ToString(sqliteformat) + "'";
        //                financeviewmodel.bollocks.AppendLine(deriveds +
        //                                    SmartParametersV2016.groupSeparator +
        //                                    "D" + SmartParametersV2016.groupSeparator +
        //                                    sql);
        //            }
        //            else
        //            {
        //                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changesList)
        //                {
        //                    if (logins_row.Updated == false)
        //                    {
        //                        if (!string.IsNullOrEmpty(rowAsString))
        //                        {
        //                            // Separate each record line with this
        //                            rowAsString += SmartParametersV2016.recordSeparator;
        //                        }
        //                        SmartFinance.LoginsSQLite abc = ConvertFinanceLogins(ourviewmodel, logins_row, deriveds.Length);
        //                        financeviewmodel.PLO.sqlite_finance_loginsList.Add(abc);
        //                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.LoginsSQLite>(myFields, abc, sqliteformat);
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(rowAsString))
        //                {
        //                    // You have to have a case "Finance.Logins": followed by update_common in SmartDBServer 
        //                    // for this to work
        //                    financeviewmodel.bollocks.AppendLine(deriveds +
        //                                            SmartParametersV2016.groupSeparator +
        //                                            "I" + SmartParametersV2016.groupSeparator +
        //                                            rowAsString);
        //                }

        //                rowAsString = "";
        //                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changesList)
        //                {
        //                    if (logins_row.Updated == true)
        //                    {
        //                        if (!string.IsNullOrEmpty(rowAsString))
        //                        {
        //                            // Separate each record line with this
        //                            rowAsString += SmartParametersV2016.recordSeparator;
        //                        }
        //                        SmartFinance.LoginsSQLite abc = ConvertFinanceLogins(ourviewmodel, logins_row, deriveds.Length);
        //                        financeviewmodel.PLO.sqlite_finance_loginsList.Add(abc);
        //                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.LoginsSQLite>(myFields, abc, sqliteformat);
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(rowAsString))
        //                {
        //                    // You have to have a case "Finance.Logins": followed by update_common in SmartDBServer 
        //                    // for this to work
        //                    financeviewmodel.bollocks.AppendLine(deriveds +
        //                                            SmartParametersV2016.groupSeparator +
        //                                            "U" + SmartParametersV2016.groupSeparator +
        //                                            rowAsString);
        //                }
        //            }
        //        }
        //        financeviewmodel.PLO.finance_logins_changesList.Clear();
        //    }
        //    return;
        //}

        internal static SmartFinance.LoginsSQLite ConvertFinanceLogins(MainViewModel ourviewmodel,
                                                                SmartFinance.Logins logins_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = logins_row.OWNER +
                                SmartParametersV2016.fieldSeparator +
                                logins_row.CONTACT_EMAIL +
                                SmartParametersV2016.fieldSeparator +
                                logins_row.CONTACT_PHONENO;

            // Time for a new RandomKey
            string details = (ourviewmodel.FDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, logins_row.RANDOMKEY, derived), "", true));

            SmartFinance.LoginsSQLite logins_enc = new SmartFinance.LoginsSQLite()
            {
                USERNAME = logins_row.USERNAME,
                CUBEFACE_CODE = logins_row.CUBEFACE_CODE.ToString(),// KEY
                INSTITUTION_CODE = logins_row.INSTITUTION_CODE,     // KEY
                BRAND_CODE = logins_row.BRAND_CODE,                 // KEY
                LOGIN_METHOD = logins_row.LOGIN_METHOD,             // *NOT* a Key in Logins(anymore!) Yes it is!
                ACTIVE_FLAG = logins_row.ACTIVE_FLAG.ToString(),
                BANKSCHECKED = Convert.ToInt32(logins_row.BANKSCHECKED),
                SAVINGSCHECKED = Convert.ToInt32(logins_row.SAVINGSCHECKED),
                INVESTMENTSCHECKED = Convert.ToInt32(logins_row.INVESTMENTSCHECKED),
                CRYPTOSCHECKED = Convert.ToInt32(logins_row.CRYPTOSCHECKED),
                DETAILS = details,
                RANDOMKEY = logins_row.RANDOMKEY,
                Updated = logins_row.Updated,
                Delete = logins_row.Delete
            };
            return logins_enc;
        }

        internal static void DoAllFinanceSwitches(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string sqliteformat)
        {
            string deriveds = "SmartFinance.Switches";
            financeviewmodel.PLO.sqlite_finance_switchesList.Clear();
            if (financeviewmodel.PLO.finance_switches_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.SwitchesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changesList)
                {
                    if (switches_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.SwitchesSQLite abc = ConvertFinanceSwitches(ourviewmodel, switches_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_finance_switchesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.SwitchesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changesList)
                {
                    if (switches_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartFinance.SwitchesSQLite abc = ConvertFinanceSwitches(ourviewmodel, switches_row, deriveds.Length);
                        financeviewmodel.PLO.sqlite_finance_switchesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartFinance.SwitchesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    financeviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                financeviewmodel.PLO.finance_switches_changesList.Clear();
            }
            return;
        }

        internal static SmartFinance.SwitchesSQLite ConvertFinanceSwitches(MainViewModel ourviewmodel,
                                                                SmartFinance.Switches switches_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = switches_row.SORTCODE +
                                SmartParametersV2016.fieldSeparator +
                                switches_row.ACCOUNT_NO +
                                SmartParametersV2016.fieldSeparator +
                                switches_row.UDPRN;
            string key_details = (ourviewmodel.FDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, switches_row.RANDOMKEY, derived), "", true));

            // Time for a new RandomKey2
            //switches_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            //string details_unencrypted = switches_row.UDPRN;
            //string details = (ourviewmodel.FDEK == "" ?
            //    details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.FDEK, switches_row.RANDOMKEY2, derived), "", true));

            SmartFinance.SwitchesSQLite switches_enc = new SmartFinance.SwitchesSQLite()
            {
                USERNAME = switches_row.USERNAME,
                CUBEFACE_CODE = switches_row.CUBEFACE_CODE.ToString(),
                INSTITUTION_CODE = switches_row.INSTITUTION_CODE,
                BRAND_CODE = switches_row.BRAND_CODE,
                SWITCH_CREATED = switches_row.SWITCH_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY = switches_row.RANDOMKEY,
                //DETAILS = details,
                AUTOSWITCH = "N",                           // Turn this OFF now
                LAST_DATETIME = switches_row.LAST_DATETIME,
                LAST_UPDATE = switches_row.LAST_UPDATE,
                EXPIRY_DATE = switches_row.EXPIRY_DATE,
                AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION,
                AUTOSWITCH_EMAIL_SENT = DateTime.Now + ourviewmodel.utcOffset,  // Local Time
                AUTOSWITCH_INSTITUTION_CODE = switches_row.AUTOSWITCH_INSTITUTION_CODE,
                AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE,
                //RANDOMKEY2 = switches_row.RANDOMKEY2,
                Updated = switches_row.Updated
            };
            return switches_enc;
        }
    }
}

//        internal static async Task<bool> Finance_Check_Meter_Async(
//#if WINFORMS
//                                                    MainProcess components,
//                                                    bool decode,
//                                                    //CheckBox checkBoxStopOnError,
//                                                    //CheckBox checkBoxQuiet,
//                                                    //CheckBox checkBoxInsert_SmartSwitch,
//                                                    //CheckBox checkBoxDownload_Bills,
//#endif

//                                                    SignInViewModel signinviewmodel,
//                                                    MainViewModel ourviewmodel,
//                                                    FinanceViewModel financeviewmodel,
//                                                    DateTime time_now,
//                                                    char finance_category_code, // Which one though?
//                                                    bool finance_submit_button,
//                                                    string defaultDate_string,
//                                                    DateTime defaultDate,
//                                                    char target_category_code,                 // Should either be F, U or default
//                                                    short target_institution_code,         // Should be known
//                                                    short target_brand_code,            // Should be known
//                                                    string target_account_no,           // Might not be known
//                                                    DateTime target_created)            // Comes in as today'sdate
//        {
//            string urgent_message = "";

//            string target_supplier_prfix = "",
//                        target_supplier_xxx = "";
//            //bool target_useProxy = false;
//            string target_user_id = "",
//                        target_password = "",
//                        //target_udprn = SmartParametersV2016.udprnDefault,
//                        target_bill_token = "";
//            DateTime[] target_last_datetime = new DateTime[2],
//                        last_datetime = new DateTime[2];
//            DateTime next_connection;


//            // We have to have a Network before we can do anything ...
//            if (!FrontEndGUI.CompareLedColour(6, ourviewmodel.whiteColour) ||
//                 FrontEndGUI.CompareLedColour( 8, ourviewmodel.redColour))
//            {
//                // If either Led6 is not white or Led8 is Red, then we just don't go here ... (very rare)
//                return false;
//            }
//            if (FrontEndGUI.CompareLedColour(5, ourviewmodel.greenColour))
//            {
//                // If Led5 is Green, then we just don't go here ... because we already are!
//                return false;
//            }

//            // The Submit button is only ever 'considered' when the network is available
//            if (!SmartNibbyV2016.Network_Availability())
//            {
//                FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
//                // We can get to here because both Led6 and Led8 are either Green or Orange
//                // But we have no network so its pointless checking for last connection
//                // dates and times.
//                return false;
//            }


//            foreach (SmartProfile.Cubefaces cubefaces in ourviewmodel.cubefaces_found)
//            {
//                if (target_category_code == SmartParametersV2016.default_army_code ||
//                    (target_army_code != SmayrtParametersV2016.default_army_code &&
//                     target_cubeface_code == cubefaces.CUBEFACE_CODE)) // <= These will all be 'Active'
//                {
//                    bool read_it = false;
//                    // This could be Default 
//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            if (!Finance_Check_Primary(finance_submit_button,
//                                                "",
//                                                "",
//                                                defaultDate,
//                                                rf last_datetime,
//                                                rf target_last_datetime,
//                                                rf target_user_id,
//                                                rf target_password,
//                                                rf read_it))
//                            {
//                                // We have Led6 on Red OR Led8 on Red OR we have no network
//                                return false;
//                            }
//                            break;                        
//                        default:
//                            break;
//                    }

//                    // If we have to skip the auto-scrape load then make
//                    // sure ... ==>
//                    if (!Finance_Check_Meter_Skip_Async(ourviewmodel,
//                                                    financeviewmodel,
//                                                    cubefaces.CUBEFACE_CODE,
//                                                    time_now))
//                    {
//                        // Hamas could be null or the consumers list could be 0
//                        continue;
//                    }

//                    // Read_it may be true or false here - true if the Submit button
//                    // has been presed, false otherwise.  But we might have a scheduled
//                    // connection so even if read-it is false, we press on ...

//                    // What if we are Submitting with stuff already there?
//                    //char ce_category_code = SmartParametersV2016.default_army_code;
//                    //short ce_institution_code = 0,
//                    //        ce_brand_code = 0;
//                    string ce_user_id = "";

//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
//                            {
//                                // Is Connection time on or before 'now'?
//                                if (Finance_Check_Connection(cubeface_row.NEXT_CONNECTION,
//                                                        defaultDate,
//                                                        time_now,
//                                                        finance_submit_button))
//                                {
//                                    // Next connection is behind 'now' so DO THIS ONE and update its time
//                                    // so if it succeeds we don't do it again
//                                    //if (!finance_submit_button)
//                                    //{
//                                    //    target_supplier_code = consumers_view_row.INSTITUTION_CODE;
//                                    //    target_brand_code = account_row_row.BRAND_CODE;
//                                    //    target_account_no = account_row_row.ACCOUNT_NO;
//                                    //    //target_created = account_row_row.CREATED;
//                                    //}
//                                    //Check_ATM_Category(account_row_row,
//                                    //                            rf target_last_datetime,
//                                    //                            rf target_udprn,
//                                    //                            rf target_udprn,
//                                    //                            rf target_udprn,
//                                    //                            rf ce_resource_code,
//                                    //                            rf ce_supplier_code,
//                                    //                            rf ce_brand_code,
//                                    //                            rf ce_user_id);
//                                    read_it = true;
//                                }
//                                if (read_it)
//                                {
//                                    break;  // Only one initiator at any one time (but two might be scraped)
//                                }
//                            }
//                            break;                        
//                        default:
//                            break;
//                    }

//                    if (read_it)
//                    {
//                        // NOW! we flag up the Meter
//                        FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);
//                    }

//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            // This is all just in case the Last Update for Banks is
//                            // different than the Last Update for Investments and SAvings for the same Supplier
//                            if (read_it)
//                            {
//                                // Form collection
//                                //foreach (SmartFinance.ConsumersView account_row_row in SmartSpikeFinanceV2017.Consumers_Finance_UserList(ourviewmodel, financeviewmodel, ce_user_id))
//                                //{
//                                // Set the 'other one' in
//                                //Check_ATM_Switch(account_row_row,
//                                //                rf target_last_datetime,
//                                //                rf target_udprn,
//                                //                rf target_udprn,
//                                //                rf target_udprn);
//                                //    break;  // Only one
//                                //}
//                            }
//                            break;                       
//                        default:
//                            break;
//                    }

//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            // Now.  Do all Accounts within all Institutions 
//                            if (read_it)
//                            {
//                                next_connection = time_now.AddDays(1);
//                                // Turn off the Quit and Submit buttons until we have finished scraping
//                                // as well as the Electricity, Gas and Dual Fuel radio buttons
//                                // ... but not the Cancel button
//                                TurnOffFinanceStatus(
//#if WINFORMS
//                                                        components,
//#endif
//                                                        financeviewmodel);                                
//                                // If Orange then set to Green
//                                // i.e. Red always stays showing if it happened
//                                if (FrontEndGUI.CompareLedColour(6, ourviewmodel.whiteColour))
//                                {
//                                  FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);
//                                }
//
//                                foreach (FinanceViewModel.InstitutionItem institution_item in financeviewmodel.FinanceInstitutionsList)
//                                {
//                                    string institution_name = institution_item.Content.ToString();
//                                    string category_type = "";
//                                    short institution_code = 0,
//                                            brand_code = 0;
//                                    SmartRoutinesV2018.Decode_Tag_Institution(institution_item.Value.ToString(), rf institution_code, rf brand_code);
//                                    if (institution_code > 0 &&
//                                        brand_code > 0)
//                                    {
//                                        financeviewmodel.category_type = category_type;
//                                        financeviewmodel.institution_code = institution_code;
//                                        financeviewmodel.brand_code = brand_code;

//                                        List<SmartFinance.Logins> logins_found =
//                                                              SmartSpikeFinanceV2017.Lookup_Finance_Logins(ourviewmodel,
//                                                                                financeviewmodel);
//                                        if (logins_found.Count == 0)
//                                        {
//                                            await SmartBobV2017.ListenerAsync(ourviewmodel, ourviewmodel.userToken, brand_code, institution_code, "Nothing found for: " + institution_name);
//                                        }
//                                        else
//                                        {
//                                            if (!string.IsNullOrEmpty(logins_found.First().CUSTOMER_NO) &&
//                                                !string.IsNullOrEmpty(logins_found.First().DOB) &&
//                                                !string.IsNullOrEmpty(logins_found.First().PASSCODE))
//                                            {
//                                                financeviewmodel.owner = "";  // NW and Santander
//                                                financeviewmodel.challenge = "";    // Santander only
//                                                List<FinanceViewModel.AccountItem> accountsList = new List<FinanceViewModel.AccountItem>();
//                                                if (await SmartBanksV2019.FinanceInstitution(
//                                                                                            ourviewmodel,
//                                                                                            financeviewmodel,
//                                                                                            accountsList,
//                                                                                            logins_found.First()))
//                                                {
//                                                    // We will have found the Accounts which will be in FinanceAccounts
//                                                    if (accountsList.Count > 0)
//                                                    {
//                                                        financeviewmodel.PLO.bank_transactionsList = new List<SmartFinance.BankTransactions>();
//                                                        if (await SmartBanksV2019.FinanceAccounts(
//                                                                                                ourviewmodel,
//                                                                                                financeviewmodel,
//                                                                                                "",   // <== all need fixing
//                                                                                                "",
//                                                                                                ""))
//                                                        {
//                                                            //financeviewmodel.listGlobalBankTransactions.AddRange(listBankTransactions);
//                                                        }

//                                                        //fnanceviewmodel.comboBoxBrandsItems.Items.Clear(); not sure why this is here
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                // No ... if either of the LEDs are Red, then there has been a problem so DO update the
//                                // next connection label to ensure we don't cycle round scraping away after a failure ...
//                                financeviewmodel.NextConnectionMessage = next_connection.ToString(financeviewmodel.financeDisplayCulture);
//                                // These can all be turned on now
//                                SmartFinanceV2025.TurnOnFinanceStatus(
//#if WINFORMS
//                                                        components,
//#endif
//                                                        financeviewmodel);                                
//                            }
//                            break;                        
//                        default:
//                            break;
//                    }
//                }
//            }

//            // Turn Off the Network Led
//            if (FrontEndGUI.CompareLedColour(5, ourviewmodel.greenColour))
//            {
//              FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.orangeColour);
//            }
//            return true;
//        }

//private static bool Finance_Check_Meter_Skip_Async(MainViewModel ourviewmodel,
//                                                FinanceViewModel financeviewmodel,           // May need to re-load these someday ..
//                                                char cubeface_code,
//                                                DateTime time_now)
//{
//    switch (cubeface_code)
//    {

//        case SmartParametersV2016.Finance:
//            if (ourviewmodel.Hamas == nll ||
//               financeviewmodel.PLO.consumers_finance_viewList.Count == 0)
//            {
//                // NO => Set the fourth Led to Red ... and carry on having flagged the failure!
//                FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
//                return false; // Avoids 'race' condition
//            }
//            break;                
//        default:
//            break;

//    }

//    switch (cubeface_code)
//    {
//        case SmartParametersV2016.Finance:
//            //if (financeviewmodel.next_connection == SmartParametersV2016.defaultDate)
//            //{
//            //    return false;
//            //}
//            break;                
//        default:
//            break;
//    }
//    return true;
//}

//internal static bool Finance_Check_Primary(bool submit_button,
//                                        string current_user_id,
//                                        string current_password,
//                                        DateTime defaultDate,
//                                        rf DateTime[] last_datetime,
//                                        rf DateTime[] target_last_datetime,
//                                        rf string target_user_id,
//                                        rf string target_password,
//                                        rf bool read_it)         // Comes in as false

//{
//    // Neither Led6 nor Led8 is Red, and Led5 is Green so we can carry on ...
//    last_datetime[0] = defaultDate;
//    last_datetime[1] = defaultDate;

//    // The Submit button is only ever 'considered' when the network is available
//    if (submit_button)  // Can't be on if Led6 or Led8 is Red?
//    {
//        // Target Supplier Id set in 'Get_Bits'
//        target_user_id = current_user_id;
//        target_password = current_password;
//        target_last_datetime[0] = defaultDate;
//        target_last_datetime[1] = defaultDate;
//        read_it = true;
//    }
//    // Even if read_it is false here we might have an scheduled connection pending
//    return true;
//}

//internal static bool Finance_Check_Connection(DateTime next_connection,
//                                            DateTime defaultDate,
//                                            DateTime time_now,
//                                            bool finance_submit_button)
//{
//    return (//(account_row_row.STATUS != 'N') &&
//            (next_connection != defaultDate) &&
//            (SmartRoutinesV2018.DateTimeCompare(next_connection, time_now) < 0) ||
//            finance_submit_button);
//}

//private static void Check_ATM_Switch(SmartProfile.Cubefaces cubeface_row,
//                                        rf DateTime[] target_last_datetime,
//                                        rf string b_target_udprn,
//                                        rf string i_target_udprn,
//                                        rf string s_target_udprn)
//{
//    // Obviously I could combine these two ...
//    string udprn = account_row_row.UDPRN;
//    char local_category_code = account_row_row.CATEGORY_CODE;
//    switch (local_category_code)
//    {
//        case SmartParametersV2016.Banks:
//            target_last_datetime[0] = account_row_row.LAST_DATETIME;
//            b_target_udprn = udprn;
//            break;
//        case SmartParametersV2016.Investments:
//            target_last_datetime[1] = account_row_row.LAST_DATETIME;
//            i_target_udprn = udprn;
//            break;
//        case SmartParametersV2016.Savings:
//            target_last_datetime[1] = account_row_row.LAST_DATETIME;
//            s_target_udprn = udprn;
//            break;
//        default:
//            break;
//    }
//    return;
//}


//private static void Check_ATM_ResourceX(SmartFinance.ConsumersView account_row_row,
//                                        rf DateTime[] target_last_datetime,
//                                        rf string b_target_udprn,
//                                        rf string i_target_udprn,
//                                        rf string s_target_udprn,
//                                        rf char ce_resource_code,
//                                        rf short ce_institution_code,
//                                        rf short ce_brand_code,
//                                        rf string ce_customer_no)
//{
//    // Again could combine these two
//    char local_category_code = account_row_row.CATEGORY_CODE;

//    string udprn = account_row_row.UDPRN;
//    switch (local_category_code)
//    {
//        case SmartParametersV2016.Banks:
//            target_last_datetime[0] = account_row_row.LAST_DATETIME;
//            b_target_udprn = udprn;

//            // Try and find the corresponding SmartParametersV2016.Investments
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Investments;
//            ce_institution_code = account_row_row.INSTITUTION_CODE;
//            ce_brand_code = account_row_row.BRAND_CODE;
//            ce_customer_no = account_row_row.CUSTOMER_NO;
//            break;
//        case SmartParametersV2016.Investments:
//            target_last_datetime[1] = account_row_row.LAST_DATETIME;
//            i_target_udprn = udprn;

//            // Try and find the corresponding 'B'
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Banks;
//            ce_institution_code = account_row_row.INSTITUTION_CODE;
//            ce_brand_code = account_row_row.BRAND_CODE;
//            ce_customer_no = account_row_row.CUSTOMER_NO;
//            break;
//        case SmartParametersV2016.Savings:
//            target_last_datetime[1] = account_row_row.LAST_DATETIME;
//            s_target_udprn = udprn;

//            // Try and find the corresponding 'B'
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Banks;
//            ce_institution_code = account_row_row.INSTITUTION_CODE;
//            ce_brand_code = account_row_row.BRAND_CODE;
//            ce_customer_no = account_row_row.CUSTOMER_NO;
//            break;
//        default:
//            break;
//    }
//    return;
//}