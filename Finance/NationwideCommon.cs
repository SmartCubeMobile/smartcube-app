using System.Text.RegularExpressions;


#if WINFORMS
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using Microsoft.Web.WebView2.Wpf;
using System.Windows;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;

using System.Threading.Tasks;
//using ABI.System;
using System.Collections.Generic;
using System;
using System.Linq;
#endif

#if ANDROIDX
using Android.Util;
using Android.Webkit;
using Android.Views;
using AndroidX.AppCompat.App;
using System.Diagnostics.CodeAnalysis;
using static SmartCubeMobile.CustomWebViewClient;
#endif

#if TEST
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Controls;
#endif

namespace SmartCubeMobile
{
    public class NationwideCommon
    {
        internal static async void HandleTimeout(

                                                WebView FinanceWebView,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel, 
                                                TaskCompletionSource<bool> tcs,
                                                bool loggedIn)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Timeout: No OTP received in time");
            await LogoutAsync(FinanceWebView,

                                ourviewmodel, financeviewmodel, tcs, loggedIn);
            return;
        }

        internal static async Task<bool> LogoutAsync(
                                                    WebView FinanceWebView,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    TaskCompletionSource<bool> tcs,
                                                    bool loggedIn)
        {
            if (loggedIn)
            {
                // No async so we can process the strings returned
                const string scriptLogout = @"(function() {const anchors = Array.from(document.querySelectorAll('a'));const logoutLink = anchors.find(el =>el.textContent.trim().toLowerCase() === 'log out');if (logoutLink) {logoutLink.click();return true;} else {return false;}})();";
                try
                {
                    string resultLogout = await FinanceWebView.EvaluateJavaScriptAsync(scriptLogout);
                    // Logout
                    loggedIn = false;
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout : " + resultLogout);
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = "Logout failed: " + ex.Message;
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                }
            }
            // Completion gets set after this
            if (FinanceWebView != null)
            {
                // Don't dispose of it - yet
                // We may need it again
            }
            tcs.TrySetResult(true); // Unblocks Task.WhenAll
            return true;
        }
        internal static async Task<bool> NationwideFindPasscodes(
                                                                WebView FinanceWebView,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                string passcode,
                                                                Action<string> logAction,
                                                                Action<string> setError)
        {
            try
            {
                const string script = @"(() => {const span = Array.from(document.querySelectorAll('span.control__label__title')).find(s => s.innerText.includes('digits from your passnumber'));return span ? span.innerText : '';})();";
                string result = await FinanceWebView.EvaluateJavaScriptAsync(script);

                result = NationwideCommon.TrimQuotes(result);
                result = Regex.Unescape(result);

                if (string.IsNullOrWhiteSpace(result))
                {
                    financeviewmodel.errorMessage = "Cannot find passnumber instruction.";


                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,financeviewmodel.errorMessage);
                    return false;
                }

                if (passcode.Length < 6)
                {
                    financeviewmodel.errorMessage = "Passcode must contain at least six digits.";
                    return false;
                }

                MatchCollection matches = Regex.Matches(result, @"([1-6])(st|nd|rd|th)");

                if (matches.Count != 3)
                {
                    financeviewmodel.errorMessage = $"Expected 3 requested digits but found {matches.Count}.";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,financeviewmodel.errorMessage);
                    return false;
                }

                string[] dropdownNames =
                {
                    "FirstPassnumberValue",
                    "SecondPassnumberValue",
                    "ThirdPassnumberValue"
                };

                for (int i = 0; i < 3; i++)
                {
                    int digitIndex =
                        int.Parse(matches[i].Groups[1].Value) - 1;

                    string digit = passcode[digitIndex].ToString();

                    string selectScript = $@"(() => {{const select = document.querySelector('select[name=""{dropdownNames[i]}""]');if (!select)return 'Missing';select.value = '{digit}';select.dispatchEvent(new Event('change', {{ bubbles:true }}));return 'OK';}})();";
                    string selectionResult = await FinanceWebView.EvaluateJavaScriptAsync(selectScript);
                    selectionResult = NationwideCommon.TrimQuotes(selectionResult);

                    if (selectionResult != "OK")
                    {
                        financeviewmodel.errorMessage = $"Couldn't find '{dropdownNames[i]}'.";
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,financeviewmodel.errorMessage);
                        return false;
                    }
                }
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,"Step 3: Passcodes set");
                return true;
            }
            catch (Exception ex)
            {
                setError(ex.Message);
                // await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ex.Message);
                return false;
            }
        }
        internal static async Task<bool> ProcessNextAccount(WebView FinanceWebView,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        short institutionCode,
                                                        short brandCode,
                                                        int CurrentAccountIndex,
                                                        List<AccountLink> AccountLinks,
                                                        string UDPRN,
                                                        Action<int> Step,
                                                        Action<bool> StepInProgress)
        {
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "In ProcessNext: " + CurrentAccountIndex.ToString());
            if (CurrentAccountIndex >= AccountLinks.Count)
            {
                return false;
            }
            AccountLink account = AccountLinks[CurrentAccountIndex];

            string accountInfo1 = "", balance = "", symbol = "", accountName = "", accountInfo2 = "";
            string sortcode = "", account_no = "";
            char categoryCode = SmartParametersV2016.defaultChar;

            //NewAccountBalance(financeviewmodel, account.innerHTML, ref accountInfo1, ref balance, ref symbol);
            //if (string.IsNullOrEmpty(accountInfo1))
            //{
            //    financeviewmodel.errorMessage = "Process Account: Couldn't parse account info: " + account.innerHTML;
            //    return false;
            //}

            accountInfo2 = SplitAccountName(financeviewmodel, accountInfo1, ref accountName);
            if (string.IsNullOrEmpty(accountName))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account name empty");
            }
            if (string.IsNullOrEmpty(accountInfo2))
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Account sortcode/account empty");
            }
            short currency_ordinal = 1;// SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, symbol);

            categoryCode = SmartParametersV2016.Banks;
            if (accountName.Contains("Savings", StringComparison.OrdinalIgnoreCase))
                categoryCode = SmartParametersV2016.Savings;

            string[] accstuff = accountInfo2.Split(SmartParametersV2016.spacechar);
            if (accstuff.Length > 1)
            {
                sortcode = accstuff[0].Replace("-", "");
                account_no = accstuff[1];
            }
            else
            {
                account_no = accstuff[0];
            }
            bool decoded = DecodeAccount(ourviewmodel,
                                               financeviewmodel,
                                               accountName,
                                               sortcode,
                                               account_no,
                                               currency_ordinal,
                                               institutionCode,
                                               brandCode,
                                               UDPRN, 
                                               categoryCode);

            if (!decoded)
            {
                financeviewmodel.errorMessage = "Cannot decode account";
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Processing: {sortcode} {account_no}");
            //SortCode = sortcode;
            //AccountNo = account_no;

            // ? Click link to start navigation
            string accountHref = account.Href.Replace("'", "\\'");
            string scriptClick = $@"(() => {{const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{accountHref}'));if (link) {{link.click();return true;}}return false;}})()";
            string resultClick = await FinanceWebView.EvaluateJavaScriptAsync(scriptClick);
            if (resultClick == "false")
            {
                financeviewmodel.errorMessage = "Cannot navigate to account";
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Link not found.");
                return false;
            }
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Processing: {sortcode} {account_no}");
            Step(6);
            StepInProgress(false);
            return true;
        }

        internal static void NewAccountBalance(FinanceViewModel financeviewmodel,
                                                string innerHtml,
                                                ref string accountName,
                                                ref string balance,
                                                ref string symbol)
        {
            try
            {
                innerHtml = innerHtml.Replace("<br>", "|");
                string[] innertext = innerHtml.Split("|");
                if (innertext.Length > 0)
                {
                    accountName = innertext[0];
                    if (innertext.Length > 1)
                    {
                        balance = innertext[1];
                        bool negative = false;
                        if (balance.Length > 0)
                        {
                            if (balance.Substring(0, 1) == "-")
                            {
                                balance = balance.Replace("-", "");
                                negative = true;
                            }
                        }
                        if (balance.Length > 0)
                        {
                            symbol = balance.Substring(0, 1);
                            {
                                balance = balance.Replace(symbol, "");
                            }
                            if (negative)
                            {
                                balance = "-" + balance;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return;
        }
        internal static string SplitAccountName(FinanceViewModel financeviewmodel,
                                                string accountInfo,
                                                ref string accountName)
        {
            string ourid = "";
            string account_no;
            string sortcode = "";

            // Now ... split the text up.
            // assume the FIRST is an account name (or part of one)
            // assume the LAST is always the balance ... good luck!
            try
            {
                string[] items = accountInfo.Split(SmartParametersV2016.spaceSplit);
                int itemCount = items.Length;
                if (itemCount < 2)
                {
                    // Not enough items  what about "/" accounts?
                    return "";
                }
                int itemPosLow = 0;
                int itemPosHigh = itemCount - 1; // 2
                if (items[itemPosLow] == "")
                {
                    // Can't find account name
                    return "";
                }
                accountName = items[itemPosLow]; // FlexAccount
                itemPosLow++;  // 1
                // Now ... everything is betwwen these two
                if (itemPosLow >= itemPosHigh)  // 1 >= 2
                {
                    // Can't find sort code and or acc num
                    return "";
                }
                // Should always have ONE acc number
                account_no = items[itemPosHigh];    // 10722017
                itemPosHigh--;                      // 1
                // Now ... everything is between these two
                while (itemPosLow <= itemPosHigh)  // 1 < 1
                {
                    // Is first character a digit?
                    char abc = Convert.ToChar(items[itemPosLow][0]); // items[1] = 070116?
                    if (Char.IsDigit(abc))
                    {
                        sortcode = items[itemPosLow];
                    }
                    else
                    {
                        accountName += SmartParametersV2016.space + items[itemPosLow];
                    }
                    itemPosLow++;
                }
                ourid = sortcode +
                    SmartParametersV2016.space +
                    account_no;
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            return ourid;
        }
        internal static bool DecodeAccount(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string accountName,
                                            string sortcode,
                                            string account_no,
                                            short accountOrdinal,
                                            short institutionCode,
                                            short brandCode,
                                            string udprn,
                                            char categoryCode)
        {
            // We pick up the Balance from the very last transaction item
            try
            {
                // 0 = sortcode
                // 1 = account_no
                // 2 = udprn
                // 3 = currency
                string vaalue = sortcode + SmartParametersV2016.fieldSeparator +
                                account_no + SmartParametersV2016.fieldSeparator +
                                udprn + SmartParametersV2016.fieldSeparator +
                                accountOrdinal;   // Assume we can find it in Banking
                vaalue = vaalue.Replace("&amp;", "&");                                                                           // Safety check
                string[] itemArray = vaalue.Split(SmartParametersV2016.fieldSeparator);

                financeviewmodel.account_id++;

                FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
                {
                    AccountID = financeviewmodel.account_id,
                    Content = accountName,
                    // Here we have [0] = sort_code
                    //              [1] = account_no
                    //              [2] = udprn
                    //              [3] = account ordinal 
                    Value = string.Join(SmartParametersV2016.bar.ToString(), itemArray)
                };
                financeviewmodel.accountItems.Add(account_item);
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }
        internal static string TrimQuotes(string input)
        {
            if (input == null) return null;
            return input.Trim('"');
        }
        internal static async Task<bool> NationwideTransactions(
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                char categoryCode,
                                                                string[] transactions,
                                                                short institutionCode,
                                                                short brandCode,
                                                                string sortcode,
                                                                string account_no,
                                                                string UDPRN,
                                                                string symbol,
                                                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                                List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                                DateTime accountCreated)
        {
            // This selects the ones we want (!!) with an EXTRA FUCKER which has no 'id' so
            // the Chimps don't let us filter that one out with a selector - we
            // have to FUCK ABOUT and do it ourselves .. I don't know how to
            // do 'multiple selectors' i.e. acLink AND href=/AccountList
            string date = "",
                    description = "",
                    transaction_type = "";
            int isPaidIn = 0;
            int balancePaidIn = 0;
            string type = "";
            double cryptoAmount = 0;
            short cryptoAmountOrdinal = 0;
            double amount = 0;
            short amountOrdinal = 0;
            double balance = 0;
            short balanceOrdinal = 0;
            DateTime transactionDate = SmartParametersV2016.defaultDate;

            string amountSymbol = "";
            string balanceSymbol = "";

            financeviewmodel.sequence_no = SmartFinanceV2025.UnixSequenceNo();

            // These bank transactions are in DESCENDING date order (I think!)            
            // So we find the last, and work backwards down the list to the first
            try
            {
                int howmany = transactions.Length - 1;
                while (howmany >= 0)
                {
                    string[] items = transactions[howmany].Split(SmartParametersV2016.tab);
                    if (items.Length == 6)
                    {
                        int itemIndex = 0;
                        foreach (string item in items)
                        {
                            switch (itemIndex)
                            {
                                case 0: // Date
                                    try
                                    {
                                        date = item.Trim();
                                        transactionDate = SmartRoutinesV2018.DateTimeParse(date);
                                    }
                                    catch (Exception ex)
                                    {
                                        await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW date", ex.Message + ": " + date);
                                        return false;
                                    }
                                    break;
                                case 1: // Transaction type
                                    transaction_type = item.Trim();
                                    break;
                                case 2: // Description
                                    description = item.Trim();
                                    break;
                                case 3: // Paid Out
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        if (AMOUNT == "")
                                        {
                                            isPaidIn = 1;
                                        }
                                        else
                                        {
                                            isPaidIn = 0;
                                            // All amounts as Negative
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;    // Even Paid out values are positive
                                            //Fiat currencies are legal tender controlled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work

                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = 1;
                                    }
                                    break;
                                case 4: // Paid In
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        if (AMOUNT == "")
                                        {
                                            isPaidIn = 0;
                                        }
                                        else
                                        {
                                            isPaidIn = 1;
                                            // All amounts as Positive
                                            // May need to find first NON-DIGIT here, but for the time being ...
                                            amountSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                            double VALUE = Convert.ToDouble(AMOUNT.Replace(amountSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                            amount = VALUE;
                                            //Fiat currencies are legal tender cont rolled by governments.
                                            //Crypto currencies are digital assets that use blockchain technology.
                                            amountOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, amountSymbol); // Er, might work
                                        }
                                    }
                                    else
                                    {
                                        isPaidIn = 0;
                                    }
                                    break;
                                case 5: // Balance
                                    if (item.Length > 0)
                                    {
                                        string AMOUNT = item.Trim();
                                        // All amounts as Positive
                                        // May need to find first NON-DIGIT here, but for the time being ...
                                        balanceSymbol = AMOUNT.Substring(0, 1); // Get LEADING currency symbol

                                        double VALUE = Convert.ToDouble(AMOUNT.Replace(balanceSymbol, "").Replace(SmartParametersV2016.defaultCurrencySeparator.ToString(), ""));
                                        balance = VALUE; // Might be + or -
                                        if (balance < 0)
                                        {
                                            balancePaidIn = 0;
                                        }
                                        else
                                        {
                                            balancePaidIn = 1;
                                        }
                                        balanceOrdinal = SmartSpikeFinanceV2017.GetCurrencyOrdinal(ourviewmodel, balanceSymbol); // Er, might work
                                    }
                                    break;
                                default:
                                    break;
                            }
                            itemIndex++;
                        }
                        short[] transCodes = SmartSpikeFinanceV2017.Finance_Lookup_TransactionTypes(financeviewmodel,
                                                                                        transaction_groupsFound,
                                                                                        transaction_typesFound,
                                                                                        isPaidIn,
                                                                                        transaction_type,
                                                                                        false,
                                                                                        institutionCode,
                                                                                        brandCode,
                                                                                        categoryCode);
                        if (transCodes.Length != 2)
                        {
                            await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                            // Carry on
                        }
                        else
                        {
                            if (transCodes[0] == 0 || transCodes[1] == 0)
                            {
                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NW Trans Lookup", "Trans type  failed: " + transaction_type);
                                // Carry on - we have others to do
                            }
                        }
                        // This is ALL WRONG ... hmm not sure it is
                        // need this
                        DateTime startDate = accountCreated;                    // Latest account
                        DateTime endDate = Convert.ToDateTime(transactionDate);     // Transaction Date

                        int diff = SmartDanV2025.GetMonthsDifference(startDate, endDate);

                        short statementNo = Convert.ToInt16(diff + SmartParametersV2016.NWMonthsAdjustment);

                        // Calculate (Kludge City lives on) the statement date and no
                        DateTime statementDate = SmartDanV2025.GetFirstOfNextMonth(transactionDate);

                        // Have we already got this one? (Ignore sequence_no)
                        List<SmartFinance.TransactionsCategories>
                            bt_found = SmartSpikeFinanceV2017.Finance_Lookup_BankTransaction(financeviewmodel,
                                                    ourviewmodel.UserName,
                                                    financeviewmodel.cubeface_code,
                                                    institutionCode,
                                                    brandCode,
                                                    sortcode,
                                                    account_no,
                                                    UDPRN,
                                                    statementDate,
                                                    statementNo,
                                                    transactionDate,
                                                    isPaidIn,
                                                    transCodes,
                                                    description,
                                                    type,               // Crypto Type
                                                    cryptoAmount,       // Crypto
                                                    cryptoAmountOrdinal,// Crypto
                                                    amount,
                                                    amountOrdinal,
                                                    balance,
                                                    balanceOrdinal,
                                                    balancePaidIn);
                        // Only add it in if we can't find it
                        if (bt_found.Count == 0)
                        {
                            // Before we add it in, check to see if we have its header

                            // Now! We are about to add it in, but does it have a header?
                            // Lets go and see in the Transactions list ...
                            SmartFinance.Transactions transHeader = new SmartFinance.Transactions()
                            {
                                USERNAME = ourviewmodel.UserName,
                                CUBEFACE_CODE = SmartParametersV2016.Finance,
                                INSTITUTION_CODE = institutionCode,
                                BRAND_CODE = brandCode,
                                SORTCODE = sortcode,
                                ACCOUNT_NO = account_no,
                                UDPRN = UDPRN,
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

                            financeviewmodel.sequence_no++;
                            // Still want to add it in even if we can't analyze the transaction ...
                            int random1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            int random2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                            // Use common template
                            SmartFinance.TransactionsCategories bankTransaction =
                                SmartDanV2025.TransactionCategory_Template(ourviewmodel,
                                                                        financeviewmodel,
                                                                        institutionCode,
                                                                        brandCode,
                                                                        sortcode,
                                                                        account_no,
                                                                        UDPRN,
                                                                        statementDate,
                                                                        statementNo,
                                                                        financeviewmodel.sequence_no,
                                                                        random1,
                                                                        transactionDate,
                                                                        isPaidIn,
                                                                        transCodes,
                                                                        description,
                                                                        type,
                                                                        cryptoAmount,
                                                                        cryptoAmountOrdinal,
                                                                        amount,
                                                                        amountOrdinal,  // Should be 1 for GBP!
                                                                        balance,
                                                                        balanceOrdinal, // Should be 1 for GBP
                                                                        balancePaidIn,
                                                                        random2,
                                                                        false);         // Updated
                            financeviewmodel.PLO.transactionscategories_changesList.Add(bankTransaction);
                            financeviewmodel.transactions_count++;
                        }
                    }
                    howmany--;
                }
            }
            catch (System.Exception ex)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "NationwideTransactions" + ": " + ex.Message))
                {
                    return false;
                }
                return false;
            }
            return true;
        }
    }
}