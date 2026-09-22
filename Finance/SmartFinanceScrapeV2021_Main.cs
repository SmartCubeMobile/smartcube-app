using System;
//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is

#if PRODUCTION || DEVELOPMENT
#endif

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
//using System.Net.Http;        // No more of this absolute bollocks shit
using System.Linq;
using System.Globalization;
//using System.Runtime.CompilerServices;
//using System.Net.NetworkInformation;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;

#if WINFORMS
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using MoreLinq;
using SmartDashboardV2018;
#endif

#if WPF
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using MoreLinq;
using System.Timers;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Automation.Text;
#endif

#if XAMARIN
using MoreLinq;
using System.IO;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
#endif

namespace SmartCubeMobile
{
    public class SmartFinanceScrapeV2021
    {
        
        internal static async Task<bool> Finance_ButtonSubmitClick_Actual(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            string urgent_message = string.Empty;

            if (MainPage.financeviewmodel.finance_token == CancellationToken.None)
            {
                MainPage.financeviewmodel.finance_cts = new CancellationTokenSource();
                MainPage.financeviewmodel.finance_token = MainPage.financeviewmodel.finance_cts.Token;
                //MainPage.TextBlockUpdateFinance(MainPage.ourviewmodel, "Finance token: " + financeviewmodel.finance_token.WaitHandle.SafeWaitHandle.GetHashCode().ToString());
            }

            // Not sure if this is the right test, but leave it for the time being
            // It wasn't - THIS is the right test!
            //if (financeviewmodel.FinanceInstitutions.Count > 0)
            if (financeviewmodel.FinanceInstitutions_List.Count > 0)
            {
                SmartFinanceV2021.TurnOffFinanceStatus(
#if WINFORMS
                                        process_components,
#endif
                                        financeviewmodel);
                // Get everything in an entire list, and THEN see if they need adding to Global!!
                List<SmartFinance.BankTransactions> LocalBankTransactions = new List<SmartFinance.BankTransactions>();
#if !DEVELOPMENT
#if WPF
                if (ourviewmodel.TextBlockBorder == Visibility.Hidden)
                {
                    ourviewmodel.TextBlockBorder = Visibility.Visible;
                    ourviewmodel.ScrollViewerVisible = Visibility.Visible;
                }
#endif
#if WINFORMS || XAMARIN
                if (!ourviewmodel.TextBlockBorder)
                {
                    ourviewmodel.TextBlockBorder = true;
                    ourviewmodel.ScrollViewerVisible = true;
                }
#endif
#endif
                // Now.  Do all Accounts within all Brands 
                foreach (FinanceViewModel.InstitutionItem institution_item in financeviewmodel.FinanceInstitutions_List)
                {
#if WINFORMS
                    if (institution_item.Colour == ourviewmodel.green_colour)
#endif
#if WPF
                    // Ray!!! Check all this Color comparison shit actually works  
                    SolidColorBrush newBrush = (SolidColorBrush)ourviewmodel.green_colour;
                    if (institution_item.Colour.ToString() == newBrush.Color.ToString())
#endif
#if XAMARIN
                    // Because Xamarin chimps don't have a picker with colour that works - but I fucking DO!!
                    if (institution_item.Colour == ourviewmodel.green_colour)
#endif
                    {
                        short institution_code = 0,
                        brand_code = 0;

                        SmartRoutinesV2018.Decode_Tag_Institution(institution_item.Value.ToString(), ref institution_code, ref brand_code);
                        if (institution_code > 0 &&
                            brand_code > 0)
                        {
                            string bank_name = institution_item.Content;
#if WINFORMS
                            bank_name = bank_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif

                            financeviewmodel.institution_code = institution_code;
                            financeviewmodel.brand_code = brand_code;
                            List<SmartFinance.Logins> logins_found = SmartSpikeV2017.Lookup_Finance_Logins(ourviewmodel, financeviewmodel);
                            if (logins_found.Count == 0)
                            {
                                // This is a fatal error because you have an Institution * <= checked
                                // which SHOULD have created the Resource record for it ... but that's not 
                                // there anymore ... so where has it gone??
                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Cannot find resource for: " + bank_name);
                                // but carry on as there might be others to scrape
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(logins_found.First().CUSTOMER_NO) ||
                                    string.IsNullOrEmpty(logins_found.First().DOB) ||
                                    string.IsNullOrEmpty(logins_found.First().MEMORABLE) ||
                                    string.IsNullOrEmpty(logins_found.First().PASSCODE))
                                {
                                    // This is a fatal error because you have an Institution * <= checked
                                    // which has a created the Resource record for it ... but the connection  
                                    // info isn't there anymore ... so where has it gone??
                                    await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, institution_code, brand_code, "Cannot find connection data for: " + bank_name);
                                    // but carry on as there might be others to scrape
                                }
                                else
                                {
                                    List<FinanceViewModel.AccountItem> accounts_list = new List<FinanceViewModel.AccountItem>();
                                    financeviewmodel.owner = logins_found.First().OWNER;
                                    financeviewmodel.challenge = string.Empty;

                                    // Its in HERE that we see if we have any HTTP connection info
                                    // YOU COULD DO the RBS Sandbox stuff?
                                    if (bank_name == "Royal Bank of Scotland")
                                    {
#if WINFORMS

                                        if (!await DashboardFinanceV2019.OpenBanking_Test(
#else
                                        if (!await OpenBanking_Test(
#endif
#if WINFORMS
                                                                            process_components,
                                                                                process_components.OpenBankingLog,
#endif
                                                                            ourviewmodel,
                                                                                financeviewmodel,
                                                                                bank_name))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // This SHOULD get all the Inserts and Updates in one fell swoop
                if (!await DoAll_SmartFinance(ourviewmodel,
                            financeviewmodel,
                            true,
                            SmartParametersV2016.sqldateFormat,
                            em => financeviewmodel.error_message = em))
                {
                    return false;
                }
#if !DEVELOPMENT
#if WPF
                if (ourviewmodel.TextBlockBorder == Visibility.Visible)
                {
                    ourviewmodel.TextBlockBorder = Visibility.Hidden;
                    ourviewmodel.ScrollViewerVisible = Visibility.Visible;
                }
#endif
#if WINFORMS || XAMARIN
                if (ourviewmodel.TextBlockBorder)
                {
                    ourviewmodel.TextBlockBorder = false;
                    ourviewmodel.ScrollViewerVisible = false;
                }
#endif
#endif
                // FinanceViewModel 'rebuild' probably been turned on in DoAllSmartFinance above
                if (financeviewmodel.rebuild)
                {
                    SmartSpikeV2017.Find_Bank_Transactions(
#if WINFORMS
                                                    process_components.comboBoxGUIFinanceProviders,
                                                    process_components.comboBoxGUIFinanceAccounts,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    SmartParametersV2016.default_date,
                                                    SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset),
                                                    true);  // Respect the Start and End Dates above

                }
                SmartFinanceV2021.TurnOnFinanceStatus(
#if WINFORMS
                                                        process_components,
#endif
                                                        financeviewmodel);
                return true;
            }
            return false;
        }

        internal static async Task<bool> DoAll_SmartFinance(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                bool full_or_view,
                                                string sqliteformat,
                                                Action<string> set_err_message)
        {
            // Now do all the Finance.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express datanase in
            // fucking stupid US "English" format
            //
            StringBuilder bollocks = new StringBuilder();

            DoAll_Finance_Categories(financeviewmodel,
                                    sqliteformat,
                                    ref bollocks);
            // These can be updated and inserted
            DoAll_Finance_Accounts(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            ref bollocks,
                            set_err_message);   // <= Because data is encrypted inside
            DoAll_Finance_Logins(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            ref bollocks,
                            set_err_message);   // <= Because data is encrypted inside
            DoAll_Finance_Switches(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            ref bollocks,
                            set_err_message);   // <= Because data is encrypted inside
            DoAll_Finance_Transactions(ourviewmodel,
                            financeviewmodel,
                            sqliteformat,
                            ref bollocks,
                            set_err_message);   // <= Because data is encrypted inside
            if (full_or_view)
            {
                // We are taking a chance here that this all works, because the
                // CONSUMER_ENERGY record has either been Created OR updated with its
                // Info ... SO it had better work ...
                //char resource_code = SmartParametersV2016.default_resource_code;
                //switch (resource_code)
                //{
                //    case SmartParametersV2016.Banks:
                //        // These can be only be inserted
                //        DoAll_Banks_Usage(financeviewmodel, sqliteformat, ref bollocks);
                //        break;
                //    case SmartParametersV2016.Investments:
                //        // These can be only be inserted
                //        DoAll_Investment_Usage(financeviewmodel, sqliteformat, ref bollocks);
                //        break;
                //    case SmartParametersV2016.Savings:
                //        // These can be only be inserted
                //        DoAll_Savings_Usage(financeviewmodel, sqliteformat, ref bollocks);
                //        break;
                //    default:
                //        break;
                //}
            }

            if (bollocks.Length > 0 &&
                (ourviewmodel.UserNameColour == ourviewmodel.green_colour))
            {
                // This might return true or false
                if (!await SmartBobV2017.Insert_COMMON_Async(ourviewmodel, ourviewmodel.quit_token, "SmartSwitch", bollocks))
                {
                    return false;
                }
                else
                {
                    // Update the internal DB
                    Finance_Update_Internal_DB(ourviewmodel,
                                        financeviewmodel);
                    financeviewmodel.PLO.consumers_finance_view_list = SmartSpikeV2017.Great_Finance_Consumers_View(ourviewmodel,
                                                                                                                        financeviewmodel);
                }
            }
            // THank FUCK for that!!
            // If we haven't anything to send ... then that's good because any inserts or updates won't fail!
            return true;
        }

        internal static void Finance_Update_Internal_DB(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            // Categories
            if (financeviewmodel.PLO.finance_categories_changes_list.Count > 0)
            {
                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changes_list.ToList())
                {
                    if (categories_row.Updated == false)
                    {
                        financeviewmodel.PLO.finance_categories_list.Add(categories_row);
                    }
                }

                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changes_list.ToList())
                {
                    if (categories_row.Updated == true)
                    {
                        foreach (SmartFinance.Categories old_row in financeviewmodel.PLO.finance_categories_list)
                        {
                            if (old_row.CUBEFACE_CODE == categories_row.CUBEFACE_CODE &&
                                old_row.CATEGORY_CODE == categories_row.CATEGORY_CODE)// &&
                                                                                      //old_row.CATEGORY_TYPE == categories_row.CATEGORY_TYPE)
                            {
                                old_row.LAST_CHECKED = categories_row.LAST_CHECKED;
                            }
                        }
                    }
                }
                // Because we have had an categories change set this
                financeviewmodel.rebuild = true;
                // Clear down all our crimes!
                financeviewmodel.PLO.finance_categories_changes_list.Clear();
            }

            // Accounts
            if (financeviewmodel.PLO.finance_accounts_changes_list.Count > 0)
            {
                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changes_list.ToList())
                {
                    if (accounts_row.Updated == false)
                    {
                        financeviewmodel.PLO.finance_accounts_list.Add(accounts_row);
                    }
                }

                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changes_list.ToList())
                {
                    if (accounts_row.Updated == true)
                    {
                        foreach (SmartFinance.Accounts old_row in financeviewmodel.PLO.finance_accounts_list)
                        {
                            if (old_row.CUBEFACE_CODE == accounts_row.CUBEFACE_CODE &&
                                old_row.CATEGORY_CODE == accounts_row.CATEGORY_CODE &&
                                //old_row.CATEGORY_TYPE == accounts_row.CATEGORY_TYPE &&
                                old_row.INSTITUTION_CODE == accounts_row.INSTITUTION_CODE &&
                                old_row.BRAND_CODE == accounts_row.BRAND_CODE &&
                                old_row.ACCOUNT_CREATED == accounts_row.ACCOUNT_CREATED &&
                                old_row.ACCOUNT_ID == accounts_row.ACCOUNT_ID)
                            {
                                old_row.RANDOMKEY1 = accounts_row.RANDOMKEY1;
                                old_row.SORTCODE = accounts_row.SORTCODE;
                                old_row.ACCOUNT_NO = accounts_row.ACCOUNT_NO;
                                old_row.CURRENCY = accounts_row.CURRENCY;
                                old_row.ACCOUNT_TYPE = accounts_row.ACCOUNT_TYPE;
                                old_row.ACCOUNT_SUBTYPE = accounts_row.ACCOUNT_SUBTYPE;
                                old_row.DESCRIPTION = accounts_row.DESCRIPTION;
                                old_row.NICKNAME = accounts_row.NICKNAME;
                                old_row.ACCOUNT_BALANCE = accounts_row.ACCOUNT_BALANCE;
                                old_row.TITLE = accounts_row.TITLE;
                                old_row.OTHER = accounts_row.OTHER;
                                old_row.STATUS = accounts_row.STATUS;
                                old_row.RANDOMKEY2 = accounts_row.RANDOMKEY2;
                            }
                        }
                    }
                }
                // Because we have had an account change set this
                financeviewmodel.rebuild = true;
                // Clear down all our crimes!
                financeviewmodel.PLO.finance_accounts_changes_list.Clear();
            }

            // Logins
            if (financeviewmodel.PLO.finance_logins_changes_list.Count > 0)
            {
                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changes_list.ToList())
                {
                    if (logins_row.Updated == false)
                    {
                        financeviewmodel.PLO.finance_logins_list.Add(logins_row);
                    }
                }

                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changes_list.ToList())
                {
                    if (logins_row.Updated == true)
                    {
                        foreach (SmartFinance.Logins old_row in financeviewmodel.PLO.finance_logins_list)
                        {
                            if (old_row.CUBEFACE_CODE == logins_row.CUBEFACE_CODE &&
                                old_row.CATEGORY_CODE == logins_row.CATEGORY_CODE &&
                                //old_row.CATEGORY_TYPE == logins_row.CATEGORY_TYPE &&
                                old_row.INSTITUTION_CODE == logins_row.INSTITUTION_CODE &&
                                old_row.BRAND_CODE == logins_row.BRAND_CODE &&
                                old_row.LOGIN_CREATED == logins_row.LOGIN_CREATED)
                            {
                                old_row.CUSTOMER_NO = logins_row.CUSTOMER_NO;
                                old_row.DOB = logins_row.DOB;
                                old_row.MEMORABLE = logins_row.MEMORABLE;
                                old_row.PASSCODE = logins_row.PASSCODE;
                                old_row.OWNER = logins_row.OWNER;
                                old_row.RANDOMKEY = logins_row.RANDOMKEY;
                            }
                        }
                    }
                }
                financeviewmodel.PLO.finance_logins_changes_list.Clear();
            }

            // Switches
            if (financeviewmodel.PLO.finance_switches_changes_list.Count > 0)
            {
                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changes_list.ToList())
                {
                    if (switches_row.Updated == false)
                    {
                        financeviewmodel.PLO.finance_switches_list.Add(switches_row);
                    }
                }

                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changes_list.ToList())
                {
                    if (switches_row.Updated == true)
                    {
                        foreach (SmartFinance.Switches old_row in financeviewmodel.PLO.finance_switches_list)
                        {
                            if (old_row.CUBEFACE_CODE == switches_row.CUBEFACE_CODE &&
                                old_row.CATEGORY_CODE == switches_row.CATEGORY_CODE &&
                                //old_row.CATEGORY_TYPE == switches_row.CATEGORY_TYPE &&
                                old_row.INSTITUTION_CODE == switches_row.INSTITUTION_CODE &&
                                old_row.BRAND_CODE == switches_row.BRAND_CODE &&
                                old_row.SWITCH_CREATED == switches_row.SWITCH_CREATED)
                            {
                                old_row.SORTCODE = switches_row.SORTCODE;
                                old_row.ACCOUNT_NO = switches_row.ACCOUNT_NO;
                                old_row.UDPRN = switches_row.UDPRN;
                                old_row.AUTOSWITCH = switches_row.AUTOSWITCH;
                                old_row.LAST_DATETIME = switches_row.LAST_DATETIME;
                                old_row.LAST_UPDATE = switches_row.LAST_UPDATE;
                                old_row.EXPIRY_DATE = switches_row.EXPIRY_DATE;
                                old_row.AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION;
                                old_row.AUTOSWITCH_INSTITUTION_CODE = switches_row.AUTOSWITCH_INSTITUTION_CODE;
                                old_row.AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE;
                                old_row.AUTOSWITCH_EMAIL_SENT = switches_row.AUTOSWITCH_EMAIL_SENT;
                                old_row.RANDOMKEY = switches_row.RANDOMKEY;
                            }
                        }
                    }
                }
                financeviewmodel.PLO.finance_switches_changes_list.Clear();
            }

            // Transactions
            if (financeviewmodel.PLO.finance_transactions_changes_list.Count > 0)
            {
                foreach (SmartFinance.BankTransactions transactions_row in financeviewmodel.PLO.finance_transactions_changes_list.ToList())
                {
                    if (transactions_row.Updated == false)
                    {
                        financeviewmodel.PLO.bank_transactions_list.Add(transactions_row);
                    }
                }

                foreach (SmartFinance.BankTransactions transactions_row in financeviewmodel.PLO.finance_transactions_changes_list.ToList())
                {
                    if (transactions_row.Updated == true)
                    {
                        foreach (SmartFinance.BankTransactions old_row in financeviewmodel.PLO.bank_transactions_list)
                        {
                            if (old_row.CUBEFACE_CODE == transactions_row.CUBEFACE_CODE &&
                                old_row.CATEGORY_CODE == transactions_row.CATEGORY_CODE &&
                                old_row.CATEGORY_TYPE == transactions_row.CATEGORY_TYPE &&
                                old_row.INSTITUTION_CODE == transactions_row.INSTITUTION_CODE &&
                                old_row.BRAND_CODE == transactions_row.BRAND_CODE &&
                                old_row.ACCOUNT_ID == transactions_row.ACCOUNT_ID &&
                                old_row.TRANSACTION_ID == transactions_row.TRANSACTION_ID)
                            {
                                // For an Update RANDOMKEY1 doesn't change!
                                old_row.RANDOMKEY1 = transactions_row.RANDOMKEY1;
                                old_row.SORTCODE = transactions_row.SORTCODE;
                                old_row.ACCOUNT_NO = transactions_row.ACCOUNT_NO;
                                old_row.SEQUENCE_NO = transactions_row.SEQUENCE_NO;
                                old_row.CREDITDEBIT_INDICATOR = transactions_row.CREDITDEBIT_INDICATOR;
                                old_row.BOOKING_DATE = transactions_row.BOOKING_DATE;
                                old_row.TRANSACTION_STATUS = transactions_row.TRANSACTION_STATUS;
                                old_row.TRANSACTION_INFORMATION = transactions_row.TRANSACTION_INFORMATION;
                                old_row.TRANSACTION_CODE = transactions_row.TRANSACTION_CODE;
                                old_row.TRANSACTION_SUB_CODE = transactions_row.TRANSACTION_SUB_CODE;
                                old_row.DESCRIPTION = transactions_row.DESCRIPTION;
                                old_row.AMOUNT = transactions_row.AMOUNT;
                                old_row.CURRENCY = transactions_row.CURRENCY;
                                old_row.BALANCE_TYPE = transactions_row.BALANCE_TYPE;
                                old_row.BALANCE_AMOUNT = transactions_row.BALANCE_AMOUNT;
                                old_row.BALANCE_CURRENCY = transactions_row.BALANCE_CURRENCY;
                                old_row.BALANCE_CREDITDEBIT_INDICATOR = transactions_row.BALANCE_CREDITDEBIT_INDICATOR;
                                old_row.RANDOMKEY2 = transactions_row.RANDOMKEY2;
                            }
                        }
                    }
                }
                // Done here

                // Sort Global to get everything in order
                // They SHOULD be sorted by TRANSACTON_CREATED and SEQUENCE_NO
                // Can we safely assue that for TWO or more batches on the same day
                // that TRANSACTION_CREATED will not be the same time i.e. Batch B
                // will be at least one second behind Batch A?

                // So for two batches A: 16-10-2018   19-09-2018   1
                //                    B: 16-10-2018   20-09-2018   1
                // Then A will always come before B

                financeviewmodel.PLO.bank_transactions_list.Sort(); // Hopefully ... the comparer gets them in the right order

                // Fix the start and end dates according to the Gloabl list

                if (financeviewmodel.PLO.bank_transactions_list.Count > 0)
                {
                    // These two alone being changed are enough to make 
                    // Find_Bank_Transactions get called again
#if WINFORMS
                    financeviewmodel.StartDate = financeviewmodel.PLO.bank_transactions_list.First().BOOKING_DATE;
                    financeviewmodel.EndDate = financeviewmodel.PLO.bank_transactions_list.Last().BOOKING_DATE;
#else
                    financeviewmodel.StartDate = financeviewmodel.PLO.bank_transactions_list.First().BOOKING_DATE;
                    financeviewmodel.EndDate = financeviewmodel.PLO.bank_transactions_list.Last().BOOKING_DATE;
#endif
                }
                // Set this one because we have had at least one Transaction change
                financeviewmodel.rebuild = true;
                // Clear down all evidence of our crimes!
                financeviewmodel.PLO.finance_transactions_changes_list.Clear();
            }
            return;
        }

        internal static void DoAll_Finance_Categories(FinanceViewModel financeviewmodel,
                                                        string yymmddShortFormat,
                                                        ref StringBuilder bollocks)
        {
            if (financeviewmodel.PLO.finance_categories_changes_list.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.Categories).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                string rowAsString = string.Empty;
                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changes_list.ToList())
                {
                    if (categories_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.Categories>(myFields, categories_row, yymmddShortFormat);
                        financeviewmodel.PLO.finance_categories_changes_list.Remove(categories_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Categories": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Categories" +
                                            SmartParametersV2016.group_separator +
                                            "I" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }

                rowAsString = string.Empty;
                foreach (SmartFinance.Categories categories_row in financeviewmodel.PLO.finance_categories_changes_list.ToList())
                {
                    if (categories_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.Categories>(myFields, categories_row, yymmddShortFormat);
                        financeviewmodel.PLO.finance_categories_changes_list.Remove(categories_row);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Categories": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Categories" +
                                            SmartParametersV2016.group_separator +
                                            "U" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }
            }
            return;
        }

        internal static void DoAll_Finance_Accounts(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    string yymmddShortFormat,
                                                    ref StringBuilder bollocks,
                                                    Action<string> set_err_message)
        {
            if (financeviewmodel.PLO.finance_accounts_changes_list.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.AccountsEnc).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                string rowAsString = string.Empty;
                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changes_list.ToList())
                {
                    if (accounts_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.AccountsEnc>(myFields, ConvertFinanceAccounts(ourviewmodel, accounts_row, set_err_message), yymmddShortFormat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Accounts" +
                                            SmartParametersV2016.group_separator +
                                            "I" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }

                rowAsString = string.Empty;
                foreach (SmartFinance.Accounts accounts_row in financeviewmodel.PLO.finance_accounts_changes_list.ToList())
                {
                    if (accounts_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.AccountsEnc>(myFields, ConvertFinanceAccounts(ourviewmodel, accounts_row, set_err_message), yymmddShortFormat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Accounts": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Accounts" +
                                            SmartParametersV2016.group_separator +
                                            "U" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }
            }
            return;
        }

        internal static SmartFinance.AccountsEnc ConvertFinanceAccounts(MainViewModel ourviewmodel,
                                                                SmartFinance.Accounts accounts_row,
                                                                Action<string> set_err_message)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = accounts_row.ACCOUNT_ID;


            string key_details = (ourviewmodel.PassWord == string.Empty ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)accounts_row.RANDOMKEY1), string.Empty, true, set_err_message));


            string details_unencrypted = accounts_row.CURRENCY +
                                SmartParametersV2016.unit_separator +
                                accounts_row.ACCOUNT_TYPE +
                                SmartParametersV2016.unit_separator +
                                accounts_row.ACCOUNT_SUBTYPE +
                                SmartParametersV2016.unit_separator +
                                accounts_row.DESCRIPTION +
                                SmartParametersV2016.unit_separator +
                                accounts_row.NICKNAME +
                                SmartParametersV2016.unit_separator +
                                accounts_row.ACCOUNT_BALANCE +
                                SmartParametersV2016.unit_separator +
                                accounts_row.SORTCODE +
                                SmartParametersV2016.unit_separator +
                                accounts_row.ACCOUNT_NO +
                                SmartParametersV2016.unit_separator +
                                accounts_row.TITLE +
                                SmartParametersV2016.unit_separator +
                                accounts_row.OTHER +
                                SmartParametersV2016.unit_separator +
                                accounts_row.STATUS;
            string details = (ourviewmodel.PassWord == string.Empty ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)accounts_row.RANDOMKEY2), string.Empty, true, set_err_message));

            Console.WriteLine(details.Length);
            SmartFinance.AccountsEnc accounts_enc = new SmartFinance.AccountsEnc()
            {
                CUBEFACE_CODE = accounts_row.CUBEFACE_CODE,
                CATEGORY_CODE = accounts_row.CATEGORY_CODE,
                //CATEGORY_TYPE = accounts_row.CATEGORY_TYPE,
                INSTITUTION_CODE = accounts_row.INSTITUTION_CODE,
                BRAND_CODE = accounts_row.BRAND_CODE,
                ACCOUNT_CREATED = accounts_row.ACCOUNT_CREATED,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = accounts_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = accounts_row.RANDOMKEY2,
                Updated = false
            };
            return accounts_enc;
        }

        // You don't understand your own logic ....
        // Finance_Logins_Changes_List contains BOTH Inserts AND Updates in ONE list
        // only differentiated by the value of 'Update'.  If 'Update' is False
        // then its and INSERT, if 'Update' is True then its an UPDATE.
        // At the end of this routine ... Finance_Logins_Changes_List will be EMPTY
        // as all the Inserts and/or Updates will have been fed into Insert_Common.
        //
        // But how do you get those Inserts and/or Updates reflected into
        // the current Finance_Logins_List?
        internal static void DoAll_Finance_Logins(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string sqliteformat,
                                            ref StringBuilder bollocks,
                                            Action<string> set_err_message)
        {
            if (financeviewmodel.PLO.finance_logins_changes_list.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.LoginsEnc).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                string rowAsString = string.Empty;
                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changes_list.ToList())
                {
                    if (logins_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.LoginsEnc>(myFields, ConvertFinanceLogins(ourviewmodel, logins_row, set_err_message), sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Logins": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Logins" +
                                            SmartParametersV2016.group_separator +
                                            "I" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }

                rowAsString = string.Empty;
                foreach (SmartFinance.Logins logins_row in financeviewmodel.PLO.finance_logins_changes_list.ToList())
                {
                    if (logins_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.LoginsEnc>(myFields, ConvertFinanceLogins(ourviewmodel, logins_row, set_err_message), sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Finance.Logins": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Logins" +
                                            SmartParametersV2016.group_separator +
                                            "U" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }
            }
            return;
        }

        internal static SmartFinance.LoginsEnc ConvertFinanceLogins(MainViewModel ourviewmodel,
                                                                SmartFinance.Logins logins_row,
                                                                Action<string> set_err_message)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = logins_row.CUSTOMER_NO +
                                SmartParametersV2016.unit_separator +
                                logins_row.DOB +
                                SmartParametersV2016.unit_separator +
                                logins_row.MEMORABLE +
                                SmartParametersV2016.unit_separator +
                                logins_row.PASSCODE +
                                SmartParametersV2016.unit_separator +
                                logins_row.OWNER;
            string details = (ourviewmodel.PassWord == string.Empty ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)logins_row.RANDOMKEY), string.Empty, true, set_err_message));

            SmartFinance.LoginsEnc logins_enc = new SmartFinance.LoginsEnc()
            {
                CUBEFACE_CODE = logins_row.CUBEFACE_CODE,
                CATEGORY_CODE = logins_row.CATEGORY_CODE,
                INSTITUTION_CODE = logins_row.INSTITUTION_CODE,
                BRAND_CODE = logins_row.BRAND_CODE,
                LOGIN_CREATED = logins_row.LOGIN_CREATED,
                DETAILS = details,
                RANDOMKEY = logins_row.RANDOMKEY,
                Updated = false
            };
            return logins_enc;
        }

        internal static void DoAll_Finance_Switches(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string sqliteformat,
                                        ref StringBuilder bollocks,
                                        Action<string> set_err_message)
        {
            if (financeviewmodel.PLO.finance_switches_changes_list.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.SwitchesEnc).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                string rowAsString = string.Empty;
                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changes_list.ToList())
                {
                    if (switches_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.SwitchesEnc>(myFields, ConvertFinanceSwitches(ourviewmodel, switches_row, set_err_message), sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Switches" +
                                            SmartParametersV2016.group_separator +
                                            "I" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }

                rowAsString = string.Empty;
                foreach (SmartFinance.Switches switches_row in financeviewmodel.PLO.finance_switches_changes_list.ToList())
                {
                    if (switches_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.SwitchesEnc>(myFields, ConvertFinanceSwitches(ourviewmodel, switches_row, set_err_message), sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Switches": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "Switches" +
                                            SmartParametersV2016.group_separator +
                                            "U" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }
            }
            return;
        }

        internal static SmartFinance.SwitchesEnc ConvertFinanceSwitches(MainViewModel ourviewmodel,
                                                                SmartFinance.Switches switches_row,
                                                                Action<string> set_err_message)
        {
            // Encrypt all of these into DETAILS with randomkey
            string details_unencrypted = switches_row.SORTCODE +
                                SmartParametersV2016.unit_separator +
                                switches_row.ACCOUNT_NO;
            string details = (ourviewmodel.PassWord == string.Empty ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)switches_row.RANDOMKEY), string.Empty, true, set_err_message));

            SmartFinance.SwitchesEnc switches_enc = new SmartFinance.SwitchesEnc()
            {
                CUBEFACE_CODE = switches_row.CUBEFACE_CODE,
                CATEGORY_CODE = switches_row.CATEGORY_CODE,
                //CATEGORY_TYPE = switches_row.CATEGORY_TYPE,
                INSTITUTION_CODE = switches_row.INSTITUTION_CODE,
                BRAND_CODE = switches_row.BRAND_CODE,
                SWITCH_CREATED = switches_row.SWITCH_CREATED,
                DETAILS = details,
                UDPRN = switches_row.UDPRN,
                AUTOSWITCH = 'N',                           // Turn this OFF now
                LAST_DATETIME = switches_row.LAST_DATETIME,
                LAST_UPDATE = switches_row.LAST_UPDATE,
                EXPIRY_DATE = switches_row.EXPIRY_DATE,
                AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION,
                AUTOSWITCH_EMAIL_SENT = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset),  // GMT time
                AUTOSWITCH_INSTITUTION_CODE = switches_row.AUTOSWITCH_INSTITUTION_CODE,
                AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE,
                RANDOMKEY = switches_row.RANDOMKEY,
                Updated = false
            };
            return switches_enc;
        }

        // With Transactions there SHOULD never be any changes as we are SUPPOSED
        // to append them sequentially ... However!! This fucking program - being
        // what it is - its EASIER to leave the 'changed' code in now and remove it
        // in 5 years time, than to leave it out now and re-insert it in 5 years ...
        internal static void DoAll_Finance_Transactions(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string yymmddShortFormat,
                                        ref StringBuilder bollocks,
                                        Action<string> set_err_message)
        {
            if (financeviewmodel.PLO.finance_transactions_changes_list.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartFinance.BankTransactionsEnc).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                string rowAsString = string.Empty;
                foreach (SmartFinance.BankTransactions transactions_row in financeviewmodel.PLO.finance_transactions_changes_list.ToList())
                {
                    if (transactions_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.BankTransactionsEnc>(myFields, ConvertFinanceTransactions(ourviewmodel, transactions_row, set_err_message), yymmddShortFormat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Transactions": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "BankTransactions" +
                                            SmartParametersV2016.group_separator +
                                            "I" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }

                rowAsString = string.Empty;
                foreach (SmartFinance.BankTransactions transactions_row in financeviewmodel.PLO.finance_transactions_changes_list.ToList())
                {
                    if (transactions_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.record_separator;
                        }
                        rowAsString += SmartNibbyV2016.Build_String<SmartFinance.BankTransactionsEnc>(myFields, ConvertFinanceTransactions(ourviewmodel, transactions_row, set_err_message), yymmddShortFormat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "BankTransactions": followed by update_common in SmartDBServer 
                    // for this to work
                    bollocks.AppendLine(SmartParametersV2016.smartfinance_schema + "." + "BankTransactions" +
                                            SmartParametersV2016.group_separator +
                                            "U" + SmartParametersV2016.group_separator +
                                            rowAsString);
                }
            }
            return;
        }

        internal static SmartFinance.BankTransactionsEnc ConvertFinanceTransactions(MainViewModel ourviewmodel,
                                                                SmartFinance.BankTransactions transactions_row,
                                                                Action<string> set_err_message)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = transactions_row.ACCOUNT_ID +
                                SmartParametersV2016.unit_separator +
                                transactions_row.TRANSACTION_ID;
            string details_unencrypted = transactions_row.SORTCODE +
                                SmartParametersV2016.unit_separator +
                                transactions_row.ACCOUNT_NO +
                                SmartParametersV2016.unit_separator +
                                transactions_row.BOOKING_DATE +
                                SmartParametersV2016.unit_separator +
                                transactions_row.SEQUENCE_NO +
                                SmartParametersV2016.unit_separator +
                                transactions_row.CREDITDEBIT_INDICATOR +
                                SmartParametersV2016.unit_separator +
                                transactions_row.TRANSACTION_STATUS +
                                SmartParametersV2016.unit_separator +
                                transactions_row.TRANSACTION_INFORMATION +
                                SmartParametersV2016.unit_separator +
                                transactions_row.TRANSACTION_CODE +
                                SmartParametersV2016.unit_separator +
                                transactions_row.TRANSACTION_SUB_CODE +
                                SmartParametersV2016.unit_separator +
                                transactions_row.DESCRIPTION +
                                SmartParametersV2016.unit_separator +
                                transactions_row.AMOUNT +
                                SmartParametersV2016.unit_separator +
                                transactions_row.CURRENCY +
                                SmartParametersV2016.unit_separator +
                                transactions_row.BALANCE_TYPE +
                                SmartParametersV2016.unit_separator +
                                transactions_row.BALANCE_AMOUNT +
                                SmartParametersV2016.unit_separator +
                                transactions_row.BALANCE_CURRENCY +
                                SmartParametersV2016.unit_separator +
                                transactions_row.BALANCE_CREDITDEBIT_INDICATOR;
            // STRICTLY SPEAKING we (I) should use a different random number here .. but
            // I will cross that security bridge when (if) I ever come to it!!
            string key_details = (ourviewmodel.PassWord == string.Empty ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)transactions_row.RANDOMKEY1), string.Empty, true, set_err_message));
            string details = (ourviewmodel.PassWord == string.Empty ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PassWord, (short)transactions_row.RANDOMKEY2), string.Empty, true, set_err_message));

            SmartFinance.BankTransactionsEnc transactions_enc = new SmartFinance.BankTransactionsEnc()
            {
                CUBEFACE_CODE = transactions_row.CUBEFACE_CODE,
                CATEGORY_CODE = transactions_row.CATEGORY_CODE,
                CATEGORY_TYPE = transactions_row.CATEGORY_TYPE,
                INSTITUTION_CODE = transactions_row.INSTITUTION_CODE,
                BRAND_CODE = transactions_row.BRAND_CODE,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = transactions_row.RANDOMKEY1,
                DETAILS = details,
                RANDOMKEY2 = transactions_row.RANDOMKEY2,
                Updated = false
            };
            return transactions_enc;
        }

        internal static async Task<bool> OpenBanking_Test(
#if WINFORMS
                                                    MainProcess mainprocess,
                                                    RichTextBox OpenBankingLog,
#endif
                                                    MainViewModel ourviewmodel,
                                                   FinanceViewModel financeviewmodel,
                                                   string item_text)
        {
            bool status = true;
            if (!string.IsNullOrEmpty(item_text))
            {

#if WINFORMS
                MainProcess.banks_list = new List<SmartDashboardV2018.Bank>();
                MainProcess.accounts_list = new List<SmartFinance.Accounts>();
                MainProcess.transactions_list = new List<SmartFinance.BankTransactions>();
#endif
                string organization_id = "0014H00001lFE7pQAG";   // OB Keasdon Energy

                string Client_Id = string.Empty;
                string Client_Secret = string.Empty;
                string url_base = string.Empty;

                string Redirect_Uri = SmartSpikeV2017.Lookup_Redirect_URI(financeviewmodel,
                                                                            organization_id);

                switch (item_text)
                {
#if WINFORMS
                    case "OpenBanking":
                        OpenBankingV2021.SetUp(

                                    OpenBankingLog,
                                    ourviewmodel,
                                    financeviewmodel,
                                    MainProcess.banks_list,
                                    MainProcess.accounts_list,
                                    MainProcess.transactions_list
                                    );
                        break;
#endif
                    case "Barclays":
                        if (SmartSpikeV2017.Lookup_Client_Info(financeviewmodel,
                                                                ref Client_Id,
                                                                ref Client_Secret))
                        {
#if WINFORMS
                            mainprocess.textBoxClient_Id.Text = Client_Id;
                            mainprocess.textBoxClient_Secret.Text = Client_Secret;
#endif
                            financeviewmodel.consumer_key = Client_Id;
                            financeviewmodel.consumer_secret = Client_Secret;
                            url_base = SmartSpikeV2017.Lookup_URL_Base(financeviewmodel);
                            BarclaysV2021.SetUp(
#if WINFORMS
                                OpenBankingLog,
#endif
                                        ourviewmodel,
                                        financeviewmodel,
                                        organization_id,
                                        Client_Id,
                                        Client_Secret,
                                        url_base,
                                        Redirect_Uri
#if WINFORMS
//                                        ,
//                                        MainProcess.banks_list,
//                                        MainProcess.accounts_list,
//                                        MainProcess.transactions_list
#endif
                                        );
                        }
                        break;
                    case "Allied Irish Bank":
                        if (SmartSpikeV2017.Lookup_Client_Info(financeviewmodel,
                                                                ref Client_Id,
                                                                ref Client_Secret))
                        {
#if WINFORMS
                            mainprocess.textBoxClient_Id.Text = Client_Id;
                            mainprocess.textBoxClient_Secret.Text = Client_Secret;
#endif
                            url_base = SmartSpikeV2017.Lookup_URL_Base(financeviewmodel);
                            AlliedIrishBankV2021.SetUp(
#if WINFORMS
                                        OpenBankingLog,
#endif
                                        ourviewmodel,
                                        financeviewmodel,
                                        organization_id,
                                        Client_Id,
                                        Client_Secret,
                                        url_base,
                                        Redirect_Uri
#if WINFORMS
//                                        ,
//                                        MainProcess.banks_list,
//                                        MainProcess.accounts_list,
//                                        MainProcess.transactions_list
#endif
                                        );
                        }
                        break;

                    case "HSBC":
                        //if (SmartSpikeV2017.Lookup_Client_Info(financeviewmodel,
                        //                                        ref Client_Id,
                        //                                        ref Client_Secret))
                        //{
#if WINFORMS
                        mainprocess.textBoxClient_Id.Text = Client_Id;
                        mainprocess.textBoxClient_Secret.Text = Client_Secret;
#endif
                        url_base = SmartSpikeV2017.Lookup_URL_Base(financeviewmodel);
                        HSBCV2021.SetUp(
#if WINFORMS
                            OpenBankingLog,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    organization_id,
                                    Client_Id,
                                    Client_Secret,
                                    url_base
#if WINFORMS
//,
//                                    MainProcess.banks_list,
//                                    MainProcess.accounts_list,
//                                    MainProcess.transactions_list
#endif
                                    );
                        //}
                        break;
                    case "Royal Bank of Scotland":
                        if (SmartSpikeV2017.Lookup_Client_Info(financeviewmodel,
                                                                ref Client_Id,
                                                                ref Client_Secret))
                        {
#if WINFORMS
                            mainprocess.textBoxClient_Id.Text = Client_Id;
                            mainprocess.textBoxClient_Secret.Text = Client_Secret;
#endif
                            url_base = SmartSpikeV2017.Lookup_URL_Base(financeviewmodel);
                            if (!await RoyalBankofScotlandV2021.SetUp(
#if WINFORMS
                                OpenBankingLog,
#endif
                                        ourviewmodel,
                                        financeviewmodel,
                                        organization_id,
                                        Client_Id,
                                        Client_Secret,
                                        url_base,
                                        Redirect_Uri))
                            {
                                status = false;
                            }
                        }
                        break;
                    case "Santander":
                        if (SmartSpikeV2017.Lookup_Client_Info(financeviewmodel,
                                                                ref Client_Id,
                                                                ref Client_Secret))
                        {
#if WINFORMS
                            mainprocess.textBoxClient_Id.Text = Client_Id;
                            mainprocess.textBoxClient_Secret.Text = Client_Secret;
#endif
                            url_base = SmartSpikeV2017.Lookup_URL_Base(financeviewmodel);
                            SantanderV2021.SetUp(
#if WINFORMS
                                OpenBankingLog,
#endif
                                        ourviewmodel,
                                        financeviewmodel,
                                        organization_id,
                                        Client_Id,
                                        Client_Secret,
                                        url_base
#if WINFORMS
//                                        ,
//                                        MainProcess.banks_list,
//                                        MainProcess.accounts_list,
//                                        MainProcess.transactions_list
#endif
                                        );
                        }
                        break;
                    default:
                        break;
                }
                // Now!  Fit the fuckers into the OB DataGridViews!!!
            }
            return status;
        }

    }
}
//        internal static async Task<bool> Finance_Check_Meter_Async(
//#if WINFORMS
//                                                    MainProcess process_components,
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
//                                                    string default_date_string,
//                                                    DateTime default_date,
//                                                    char target_category_code,                 // Should either be F, U or default
//                                                    short target_institution_code,         // Should be known
//                                                    short target_brand_code,            // Should be known
//                                                    string target_account_no,           // Might not be known
//                                                    DateTime target_created)            // Comes in as today'sdate
//        {
//            string urgent_message = string.Empty;

//            string target_supplier_prefix = string.Empty,
//                        target_supplier_xxx = string.Empty;
//            //bool target_use_proxy = false;
//            string target_user_id = string.Empty,
//                        target_password = string.Empty,
//                        //target_udprn = SmartParametersV2016.udprnDefault,
//                        target_bill_token = string.Empty;
//            DateTime[] target_last_datetime = new DateTime[2],
//                        last_datetime = new DateTime[2];
//            DateTime next_connection;


//            // We have to have a Network before we can do anything ...

//#if WINFORMS
//            if (ourviewmodel.Led6 != ourviewmodel.white_colour ||
//                ourviewmodel.Led8 == ourviewmodel.red_colour)
//#endif
//#if WPF
//            if (ourviewmodel.Led6.ToString() != ourviewmodel.white_colour.ToString() ||
//                ourviewmodel.Led8.ToString() == ourviewmodel.red_colour.ToString())
//#endif
//#if XAMARIN
//            if ((ourviewmodel.Led6 != ourviewmodel.white_colour) ||
//                (ourviewmodel.Led8 == ourviewmodel.red_colour))
//#endif
//            {
//                // If either Led6 is not white or Led8 is Red, then we just don't go here ... (very rare)
//                return false;
//            }
//#if WINFORMS
//            if (ourviewmodel.Led5 == ourviewmodel.green_colour)
//#endif
//#if WPF
//            if (ourviewmodel.Led5.ToString() == ourviewmodel.green_colour.ToString())
//#endif
//#if XAMARIN
//            if (ourviewmodel.Led5 == ourviewmodel.green_colour)
//#endif
//            {
//                // If Led5 is Green, then we just don't go here ... because we already are!
//                return false;
//            }

//            // The Submit button is only ever 'considered' when the network is available
//            if (!SmartNibbyV2016.Network_Availability())
//            {
//                ourviewmodel.Led5 = ourviewmodel.orange_colour;
//                // We can get to here because both Led6 and Led8 are either Green or Orange
//                // But we have no network so its pointless checking for last connection
//                // dates and times.
//                return false;
//            }


//            foreach (SmartUsers.Cubefaces cubefaces in ourviewmodel.cubefaces_found)
//            {
//                if (target_category_code == SmartParametersV2016.default_category_code ||
//                    (target_category_code != SmartParametersV2016.default_category_code &&
//                     target_category_code == cubefaces.CUBEFACE_CODE)) // <= These will all be 'Active'
//                {
//                    bool read_it = false;
//                    // This could be Default 
//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            if (!Finance_Check_Primary(finance_submit_button,
//                                                string.Empty,
//                                                string.Empty,
//                                                default_date,
//                                                ref last_datetime,
//                                                ref target_last_datetime,
//                                                ref target_user_id,
//                                                ref target_password,
//                                                ref read_it))
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
//                    //char ce_category_code = SmartParametersV2016.default_category_code;
//                    //short ce_institution_code = 0,
//                    //        ce_brand_code = 0;
//                    string ce_user_id = string.Empty;

//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            foreach (SmartUsers.Cubefaces cubeface_row in ourviewmodel.Hamas.cubefaces_list)
//                            {
//                                // Is Connection time on or before 'now'?
//                                if (Finance_Check_Connection(cubeface_row.NEXT_CONNECTION,
//                                                        default_date,
//                                                        time_now,
//                                                        finance_submit_button))
//                                {
//                                    // Next connection is behind 'now' so DO THIS ONE and update its time
//                                    // so if it succeeds we don't do it again
//                                    //if (!finance_submit_button)
//                                    //{
//                                    //    target_supplier_code = consumers_view_row.INSTITUTION_CODE;
//                                    //    target_brand_code = consumers_view_row.BRAND_CODE;
//                                    //    target_account_no = consumers_view_row.ACCOUNT_NO;
//                                    //    //target_created = consumers_view_row.CREATED;
//                                    //}
//                                    //Check_ATM_Category(consumers_view_row,
//                                    //                            ref target_last_datetime,
//                                    //                            ref target_udprn,
//                                    //                            ref target_udprn,
//                                    //                            ref target_udprn,
//                                    //                            ref ce_resource_code,
//                                    //                            ref ce_supplier_code,
//                                    //                            ref ce_brand_code,
//                                    //                            ref ce_user_id);
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
//                        ourviewmodel.Led5 = ourviewmodel.green_colour;
//                    }

//                    switch (cubefaces.CUBEFACE_CODE)
//                    {
//                        case SmartParametersV2016.Finance:
//                            // This is all just in case the Last Update for Banks is
//                            // different than the Last Update for Investments and SAvings for the same Supplier
//                            if (read_it)
//                            {
//                                // Form collection
//                                //foreach (SmartFinance.ConsumersView consumers_view_row in SmartSpikeV2017.Consumers_Finance_User_List(ourviewmodel, financeviewmodel, ce_user_id))
//                                //{
//                                // Set the 'other one' in
//                                //Check_ATM_Switch(consumers_view_row,
//                                //                ref target_last_datetime,
//                                //                ref target_udprn,
//                                //                ref target_udprn,
//                                //                ref target_udprn);
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
//                                                        process_components,
//#endif
//                                                        financeviewmodel);                                
//                                // If Orange then set to Green
//                                // i.e. Red always stays showing if it happened
//#if WINFORMS
//                                if (ourviewmodel.Led6 == ourviewmodel.white_colour)
//                                {
//                                    ourviewmodel.Led6 = ourviewmodel.orange_colour;
//                                }
//#endif
//#if WPF
//                                if (ourviewmodel.Led6.ToString() == ourviewmodel.white_colour.ToString())
//                                {
//                                    ourviewmodel.Led6 = ourviewmodel.orange_colour;
//                                }
//#endif
//#if XAMARIN
//                                if (ourviewmodel.Led6 == ourviewmodel.white_colour)
//                                {
//                                    ourviewmodel.Led6 = ourviewmodel.orange_colour;
//                                }
//#endif
//                                foreach (FinanceViewModel.InstitutionItem institution_item in financeviewmodel.FinanceInstitutions_List)
//                                {
//                                    string institution_name = institution_item.Content.ToString();
//                                    string category_type = string.Empty;
//                                    short institution_code = 0,
//                                            brand_code = 0;
//                                    SmartRoutinesV2018.Decode_Tag_Institution(institution_item.Value.ToString(), ref institution_code, ref brand_code);
//                                    if (institution_code > 0 &&
//                                        brand_code > 0)
//                                    {
//                                        financeviewmodel.category_type = category_type;
//                                        financeviewmodel.institution_code = institution_code;
//                                        financeviewmodel.brand_code = brand_code;

//                                        List<SmartFinance.Logins> logins_found =
//                                                              SmartSpikeV2017.Lookup_Finance_Logins(ourviewmodel,
//                                                                                financeviewmodel);
//                                        if (logins_found.Count == 0)
//                                        {
//                                            await SmartBobV2017.ListenerAsync(ourviewmodel, ourviewmodel.quit_token, brand_code, institution_code, "Nothing found for: " + institution_name);
//                                        }
//                                        else
//                                        {
//                                            if (!string.IsNullOrEmpty(logins_found.First().CUSTOMER_NO) &&
//                                                !string.IsNullOrEmpty(logins_found.First().DOB) &&
//                                                !string.IsNullOrEmpty(logins_found.First().MEMORABLE) &&
//                                                !string.IsNullOrEmpty(logins_found.First().PASSCODE))
//                                            {
//                                                financeviewmodel.owner = string.Empty;  // NW and Santander
//                                                financeviewmodel.challenge = string.Empty;    // Santander only
//                                                List<FinanceViewModel.AccountItem> accounts_list = new List<FinanceViewModel.AccountItem>();
//                                                if (await SmartBanksV2019.FinanceInstitution(
//                                                                                            ourviewmodel,
//                                                                                            financeviewmodel,
//                                                                                            accounts_list,
//                                                                                            logins_found.First()))
//                                                {
//                                                    // We will have found the Accounts which will be in FinanceAccounts
//                                                    if (accounts_list.Count > 0)
//                                                    {
//                                                        financeviewmodel.PLO.bank_transactions_list = new List<SmartFinance.BankTransactions>();
//                                                        if (await SmartBanksV2019.FinanceAccounts(
//                                                                                                ourviewmodel,
//                                                                                                financeviewmodel,
//                                                                                                string.Empty,   // <== all need fixing
//                                                                                                string.Empty,
//                                                                                                string.Empty))
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
//                                financeviewmodel.NextConnectionMessage = next_connection.ToString(financeviewmodel.finance_displayCulture);
//                                // These can all be turned on now
//                                SmartFinanceV2021.TurnOnFinanceStatus(
//#if WINFORMS
//                                                        process_components,
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
//#if WINFORMS
//            if (ourviewmodel.Led5 == ourviewmodel.green_colour)
//            {
//                ourviewmodel.Led5 = ourviewmodel.orange_colour;
//            }
//#endif
//#if WPF
//            if (ourviewmodel.Led5.ToString() == ourviewmodel.green_colour.ToString())
//            {
//                ourviewmodel.Led5 = ourviewmodel.orange_colour;
//            }
//#endif
//#if XAMARIN
//            if (ourviewmodel.Led5 == ourviewmodel.green_colour)
//            {
//                ourviewmodel.Led5 = ourviewmodel.orange_colour;
//            }
//#endif
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
//               financeviewmodel.PLO.consumers_finance_view_list.Count == 0)
//            {
//                // NO => Set the fourth Led to Red ... and carry on having flagged the failure!
//                //ourviewmodel.Led4 = ourviewmodel.red_colour;
//                return false; // Avoids 'race' condition
//            }
//            break;                
//        default:
//            break;

//    }

//    switch (cubeface_code)
//    {
//        case SmartParametersV2016.Finance:
//            //if (financeviewmodel.next_connection == SmartParametersV2016.default_date)
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
//                                        DateTime default_date,
//                                        ref DateTime[] last_datetime,
//                                        ref DateTime[] target_last_datetime,
//                                        ref string target_user_id,
//                                        ref string target_password,
//                                        ref bool read_it)         // Comes in as false

//{
//    // Neither Led6 nor Led8 is Red, and Led5 is Green so we can carry on ...
//    last_datetime[0] = default_date;
//    last_datetime[1] = default_date;

//    // The Submit button is only ever 'considered' when the network is available
//    if (submit_button)  // Can't be on if Led6 or Led8 is Red?
//    {
//        // Target Supplier Id set in 'Get_Bits'
//        target_user_id = current_user_id;
//        target_password = current_password;
//        target_last_datetime[0] = default_date;
//        target_last_datetime[1] = default_date;
//        read_it = true;
//    }
//    // Even if read_it is false here we might have an scheduled connection pending
//    return true;
//}

//internal static bool Finance_Check_Connection(DateTime next_connection,
//                                            DateTime default_date,
//                                            DateTime time_now,
//                                            bool finance_submit_button)
//{
//    return (//(consumers_view_row.STATUS != 'N') &&
//            (next_connection != default_date) &&
//            (SmartRoutinesV2018.DateTimeCompare(next_connection, time_now) < 0) ||
//            finance_submit_button);
//}

//private static void Check_ATM_Switch(SmartUsers.Cubefaces cubeface_row,
//                                        ref DateTime[] target_last_datetime,
//                                        ref string b_target_udprn,
//                                        ref string i_target_udprn,
//                                        ref string s_target_udprn)
//{
//    // Obviously I could combine these two ...
//    string udprn = consumers_view_row.UDPRN;
//    char local_category_code = consumers_view_row.CATEGORY_CODE;
//    switch (local_category_code)
//    {
//        case SmartParametersV2016.Banks:
//            target_last_datetime[0] = consumers_view_row.LAST_DATETIME;
//            b_target_udprn = udprn;
//            break;
//        case SmartParametersV2016.Investments:
//            target_last_datetime[1] = consumers_view_row.LAST_DATETIME;
//            i_target_udprn = udprn;
//            break;
//        case SmartParametersV2016.Savings:
//            target_last_datetime[1] = consumers_view_row.LAST_DATETIME;
//            s_target_udprn = udprn;
//            break;
//        default:
//            break;
//    }
//    return;
//}


//private static void Check_ATM_ResourceX(SmartFinance.ConsumersView consumers_view_row,
//                                        ref DateTime[] target_last_datetime,
//                                        ref string b_target_udprn,
//                                        ref string i_target_udprn,
//                                        ref string s_target_udprn,
//                                        ref char ce_resource_code,
//                                        ref short ce_institution_code,
//                                        ref short ce_brand_code,
//                                        ref string ce_customer_no)
//{
//    // Again could combine these two
//    char local_category_code = consumers_view_row.CATEGORY_CODE;

//    string udprn = consumers_view_row.UDPRN;
//    switch (local_category_code)
//    {
//        case SmartParametersV2016.Banks:
//            target_last_datetime[0] = consumers_view_row.LAST_DATETIME;
//            b_target_udprn = udprn;

//            // Try and find the corresponding SmartParametersV2016.Investments
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Investments;
//            ce_institution_code = consumers_view_row.INSTITUTION_CODE;
//            ce_brand_code = consumers_view_row.BRAND_CODE;
//            ce_customer_no = consumers_view_row.CUSTOMER_NO;
//            break;
//        case SmartParametersV2016.Investments:
//            target_last_datetime[1] = consumers_view_row.LAST_DATETIME;
//            i_target_udprn = udprn;

//            // Try and find the corresponding 'B'
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Banks;
//            ce_institution_code = consumers_view_row.INSTITUTION_CODE;
//            ce_brand_code = consumers_view_row.BRAND_CODE;
//            ce_customer_no = consumers_view_row.CUSTOMER_NO;
//            break;
//        case SmartParametersV2016.Savings:
//            target_last_datetime[1] = consumers_view_row.LAST_DATETIME;
//            s_target_udprn = udprn;

//            // Try and find the corresponding 'B'
//            // (Both the UDPRN and the Account No may be different)
//            ce_resource_code = SmartParametersV2016.Banks;
//            ce_institution_code = consumers_view_row.INSTITUTION_CODE;
//            ce_brand_code = consumers_view_row.BRAND_CODE;
//            ce_customer_no = consumers_view_row.CUSTOMER_NO;
//            break;
//        default:
//            break;
//    }
//    return;
//}