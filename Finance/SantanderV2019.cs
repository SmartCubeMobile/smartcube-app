using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


#if WINFORMS
#endif
#if WPF
using System.Windows.Controls;
#endif

namespace SmartCubeMobile
{
    // This entire Microshit system is such utter bollocks you couldn't make it up ...
    // I got SO FAR!!  Down to the post to login in, but NW is returning a 302 after I post all the data
    // and - if you check on the actual website, then it really does think I have logged in!
    // But the 302 with the Customerized webpage shit has a bug?  or the NW website has a bug? 
    // and it keeps returning 10 re-directs to /Login/Login instead of AccountsList
    // So I am trying to control the 302 re-directs down to 1 (!!) but can I fucking do it?  Can I as fuck;
    // This UWP shit picks up a hard-coded (yes!!!- unbelievable I know) value of 10 from the Wininet (whatever the fuck
    // that is) and then - wait for it - won't let me specify a value of ... anything!  I can't set it to 0, 1, or 2
    // it HAS TO BE fucking 10!!!  So I can't then wait for 1 re-direct and then try and go and get the data
    // So its eithe 10 re-directs or none (if I turn auto-redirect off) WHAT AN ABSOLUTE PILE OF FUCKING BOLLOCKS
    // You need to  look at this https://github.com/dotnet/corefx/issues/17986


    public class SantanderV2019
    {
        internal static async Task<List<SmartFinance.BankTransactions>> Santander_Find_Transactions(
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        bool log,
                                                                        string referer,
                                                                        string url_base,
                                                                        //string url_href,
                                                                        string sortcode,
                                                                        string account_no)
        {
            string href = string.Empty;
            financeviewmodel.type = string.Empty;
            financeviewmodel.response = string.Empty;

            financeviewmodel.html_document = new HtmlAgilityPack.HtmlDocument();

            List<SmartFinance.BankTransactions> Bollocks = new List<SmartFinance.BankTransactions>();

            string data = string.Empty;
            url_base = "https://retail.santander.co.uk/EBAN_Accounts_ENS/";


            href = url_base + "channel.ssobto?dse_operationName=MyAccounts";
            Uri next_url = new Uri(href);
            referer = string.Empty;

            if (await SmartBanksV2019.DoGet_Finance(
                                                ourviewmodel,
                                                financeviewmodel,
                                                financeviewmodel.finance_token,
                                                next_url,
                                                url_base,
                                                log,
                                                "GET",

                                                referer))

            {
                await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Succeeded: Return to MyAccounts");
            }
            else
            {
                await SmartRoutinesV2018.TextBlockUpdate(
                                                    ourviewmodel,
                                                    "Failed: Return to MyAccounts");
                return Bollocks;    // Which IS empty
            }

            referer = "https://retail.santander.co.uk/EBAN_Accounts_ENS/channel.ssobto?dse_operationName=MyAccounts";

            string accounts = string.Empty;

            financeviewmodel.response = string.Empty;
            string owner = string.Empty;            // Don'tthink this is ever used?? Its just a placeholder??
            Santander_Build_Account_List(financeviewmodel.html_document, ref accounts, ref owner);
            if (!string.IsNullOrEmpty(accounts))
            {
                string[] accounts_list = accounts.Split(SmartParametersV2016.record_separator);
                if (accounts_list.Count() > 0)
                {
                    string account_name = string.Empty,
                    account_number = string.Empty,
                            sorty_code = string.Empty,
                            balance = string.Empty;
                    foreach (string account in accounts_list)
                    {
                        string[] comp = account.Split(SmartParametersV2016.unit_separator);
                        if (comp.Count() > 1)
                        {
                            string account_token = comp[0];
                            if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref account_number))
                            {
                                if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref sorty_code))
                                {
                                    sorty_code = sorty_code.Replace("-", string.Empty);
                                    account_name = account_token;
                                    if (account_number == account_no &&
                                        sorty_code == sortcode)
                                    {
                                        financeviewmodel.response = comp[1].Replace("&amp;", "&");  // Do an 'amp' conversion
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(financeviewmodel.response))
            {

                await SmartRoutinesV2018.TextBlockUpdate(
                                                ourviewmodel,
                                                "Failed: Finding account transactions link");

                return Bollocks;    // Which IS empty
            }
            else
            {
                // DON'T FORGET!! data should be separated by SmartParametersV2016.record_separator
                StringContent not_empty2 = SmartBanksV2019.Common_Build_Form_Strings_New(data);
                Uri transactions_url = new Uri(url_base + financeviewmodel.response);


                await SmartRoutinesV2018.TextBlockUpdate(
                                                    ourviewmodel,
                                                    "Requesting transactions list");


                if (await SmartBanksV2019.DoPost_NewX(
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.finance_token,
                                                    transactions_url,
                                                    url_base,
                                                    log,
                                                    1,
                                                    "POST",
                                                    not_empty2,
                                                    referer))

                {

                    await SmartRoutinesV2018.TextBlockUpdate(
                                                        ourviewmodel,
                                                        "Received transactions list");

                    // ONE DATE for all of this Batch of transactions!!
                    DateTime transaction_created = SmartEncryptionV2016.DateTimeNow(ourviewmodel.gmt_offset);  // GMT time

                    await Decode_Transactions(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.html_document,
                                                    transaction_created,
                                                    sortcode,
                                                    account_no,
                                                    Bollocks);


                    await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Found: " + Bollocks.Count + " transactions");

                    not_empty2 = new StringContent(string.Empty); // SmartBanksV2019.Common_Build_Form_Strings(next_challenge);

                    bool look_for_next = true;
                    while (look_for_next)
                    {
                        if (!Find_Pagination(financeviewmodel.html_document, ref href))
                        {
                            look_for_next = false;
                            break;
                        }
                        else
                        {
                            //transactions_url = new Uri(href);
                            transactions_url = new Uri(url_base + href);

                            if (await SmartBanksV2019.DoPost_NewX(
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        financeviewmodel.finance_token,
                                                        transactions_url,
                                                        url_base,
                                                        log,
                                                        1,
                                                        "POST",
                                                        not_empty2,
                                                        referer))

                            {
                                await Decode_Transactions(ourviewmodel,
                                                            financeviewmodel,
                                                            financeviewmodel.html_document,
                                                            transaction_created,
                                                            sortcode,
                                                            account_no,
                                                            Bollocks);

                                await SmartRoutinesV2018.TextBlockUpdate(
                                                                ourviewmodel,
                                                                "Found: " + Bollocks.Count + " transactions");


                            }
                            else
                            {

                                await SmartRoutinesV2018.TextBlockUpdate(
                                                                ourviewmodel,
                                                                "Failed: Asked for subsequent transactions");
                            }
                        }
                    }
                }
                else
                {

                    await SmartRoutinesV2018.TextBlockUpdate(
                                                    ourviewmodel,
                                                    "Failed: Asked for first transactions");
                }
            }
            return Bollocks;    // Which may be empty ...
        }

        internal static bool Find_Pagination(HtmlAgilityPack.HtmlDocument document,
                                                    ref string href)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                    HtmlCol2,
                    HtmlCol3,
                    HtmlCol4,
                    HtmlCol5,
                    HtmlCol6;

            string classname = string.Empty;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", string.Empty);
                if (classname == "pagination")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//ul");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//li");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//ul");
                            foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                            {
                                HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//li");
                                foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                                {
                                    classname = element5.GetAttributeValue("class", string.Empty);
                                    if (classname == "next")
                                    {
                                        HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//a");
                                        foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                                        {
                                            href = element6.GetAttributeValue("href", string.Empty);
                                            href = href.Replace("&amp;", "&");
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static async Task<bool> Decode_Transactions(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    HtmlAgilityPack.HtmlDocument document,
                                                    DateTime transaction_created,
                                                    string sortcode,
                                                    string account_no,
                                                    List<SmartFinance.BankTransactions> Bollocks)
        //List<SmartFinance.TransactionTypes> transaction_types_list,
        //List<SmartFinance.TransactionSubTypes> transaction_sub_types_list)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                    HtmlCol2,
                    HtmlCol3,
                    HtmlCol4;

            string inner_text = string.Empty,
                        acct_type_tag = string.Empty,
                        account_address = string.Empty,
                        classname = string.Empty,
                        href = string.Empty,
                        token = string.Empty;

            //string response_type = string.Empty,
            //        response_itself = string.Empty;

            string currency = string.Empty,
                    amount = string.Empty,
                    date = string.Empty,
                    description = string.Empty,
                    balance = string.Empty,
                    sequence = string.Empty,
                    transaction_type = string.Empty;
            bool is_paid_in = false;
            short transaction_code = 0,
                    transaction_sub_code = 0;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//table");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                classname = element1.GetAttributeValue("class", string.Empty);
                if (classname == "cardlytics_history_table data")
                {
                    HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(element1, ".//tbody");
                    foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                    {
                        HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(element2, ".//tr");
                        foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
                        {
                            int cols = 0;
                            is_paid_in = false;
                            HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//td");
                            foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                            {
                                classname = element4.GetAttributeValue("class", string.Empty);
                                string text = element4.InnerText.Trim();
                                switch (cols)
                                {
                                    case 0:     // Date
                                        if (classname == "date")    // Check
                                        {
                                            date = text;
                                        }
                                        break;
                                    case 1:     // Description
                                        description = text.Replace("&amp;", "&");
                                        description = description.Replace("#39", "'");
                                        transaction_type = description;
                                        //foreach (SmartFinance.TransactionTypes type_row in transaction_types_found)
                                        //{
                                        //    if (type_row.DESCRIPTION.IndexOf("|") == -1)
                                        //    {
                                        //        // Check first chars of description to try and allocate it
                                        //        if (description.IndexOf(type_row.DESCRIPTION, 0) >= 0)
                                        //        {
                                        //            transaction_type = type_row.TRANSACTION_GROUP;
                                        //            break;
                                        //        }
                                        //    }
                                        //}
                                        break;
                                    case 2:     // Money In
                                        if (classname == "currency")    // Check
                                        {
                                            if (string.IsNullOrEmpty(text))
                                            {
                                                is_paid_in = false;
                                            }
                                            else
                                            {
                                                amount = text.Replace("£", string.Empty);
                                                amount = amount.Replace(".", string.Empty);
                                                amount = amount.Replace(",", string.Empty);
                                            }
                                            currency = "GBP";
                                        }
                                        break;
                                    case 3:     // Money Out
                                        if (string.IsNullOrEmpty(text))
                                        {
                                            is_paid_in = true;
                                        }
                                        else
                                        {
                                            amount = text.Replace("£", string.Empty);
                                            amount = amount.Replace(".", string.Empty);
                                            amount = amount.Replace(",", string.Empty);
                                        }
                                        break;
                                    case 4:     // Balance
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            balance = text.Replace("£", string.Empty);
                                            balance = balance.Replace(".", string.Empty);
                                            balance = balance.Replace(",", string.Empty);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                cols = cols + 1;
                            }

                            if (!SmartSpikeFinanceV2017.Lookup_Transaction_Code(financeviewmodel,
                                                                        is_paid_in,
                                                                        ref transaction_type,
                                                                        ref transaction_code,
                                                                        true))
                            {

                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, "Trans type  failed: " + transaction_type);

                            }

                            if (!SmartSpikeFinanceV2017.Lookup_Transaction_SubCode(financeviewmodel,
                                                                            is_paid_in,
                                                                            ref transaction_type,
                                                                            ref transaction_code,
                                                                            ref transaction_sub_code,
                                                                            true))
                            {

                                await SmartBobV2017.ListenerAsync(ourviewmodel, financeviewmodel.finance_token, financeviewmodel.institution_code, financeviewmodel.brand_code, "Trans type  failed: " + transaction_type);

                            }
                            int random1 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);
                            int random2 = SmartRoutinesV2018.Get_Next_Random(ourviewmodel.random_r);

                            SmartFinance.BankTransactions zzz = new SmartFinance.BankTransactions()
                            {
                                USERNAME = ourviewmodel.UserName,
                                INSTITUTION_CODE = financeviewmodel.institution_code,
                                BRAND_CODE = financeviewmodel.brand_code,
                                SORTCODE = sortcode,
                                ACCOUNT_NO = account_no,
                                RANDOMKEY1 = random1,
                                BOOKING_DATE = Convert.ToDateTime(date),
                                SEQUENCE_NO = new DateTimeOffset(DateTime.Now + ourviewmodel.gmt_offset).ToUnixTimeMilliseconds(), // Unique AND increasing
                                CREDITDEBIT_INDICATOR = is_paid_in,
                                TRANSACTION_CODE = transaction_code,
                                TRANSACTION_SUB_CODE = transaction_sub_code,
                                DESCRIPTION = description,
                                AMOUNT = Convert.ToInt32(amount),
                                CURRENCY = currency,
                                BALANCE_AMOUNT = Convert.ToInt32(balance),
                                RANDOMKEY2 = random2
                            };
                            // Because they are in descending date order
                            Bollocks.Insert(0, zzz);

                        }
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> Santander_Find_Accounts(
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                //string url_prefix,
                                                                string url_base,
                                                                bool log,
                                                                string referer,
                                                                //if WINFORMS || WPF
                                                                //                                                                <List<CustomListView>> set_Santander_Accounts,
                                                                //#endif

                                                                //#if XAMARIN
                                                                //                                                                <List<CustomListView>> set_Santander_Accounts,
                                                                //#endif
                                                                string customer,
                                                                string passcode,
                                                                string ERN)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1,
                    HtmlCol2;
            string inner_text = string.Empty,
                        acct_type_tag = string.Empty,
                        account_address = string.Empty,
                        classname = string.Empty,
                        href = string.Empty,
                        name = string.Empty;
            bool found_it = false;

            string foData = string.Empty,
                            origen = string.Empty,
                            binddevicePrint = string.Empty,
                            IdGhostAccount = string.Empty,
                            urlLog = string.Empty,
                            ssCookie = string.Empty,
                            jsessionID = string.Empty;

            string dse_applicationName = string.Empty,
                                            dse_sessionId = string.Empty,
                                            dse_operationName = string.Empty,
                                            dse_threadId = string.Empty,
                                            dse_pageId = string.Empty,
                                            dse_processorState = string.Empty,
                                            dse_processorId = string.Empty,
                                            dse_cmd = string.Empty,
                                            dse_errorPage = string.Empty,
                                            urlLog_new = string.Empty,
                                            // ssCookie_new = string.Empty,
                                            dse_nextEventName = string.Empty;


            financeviewmodel.type = string.Empty;
            financeviewmodel.response = string.Empty;

            bool carryon = false;

            Uri url = new Uri(url_base);

            financeviewmodel.html_document = new HtmlAgilityPack.HtmlDocument();

            if (await SmartBanksV2019.DoGet_Finance(
                                                ourviewmodel,
                                                financeviewmodel,
                                                financeviewmodel.finance_token,
                                                url,
                                                url_base,
                                                log,
                                                "GET",

                                                referer))

            {
                switch (financeviewmodel.type)
                {
                    case "uri":

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "not expected " + financeviewmodel.type);
                        break;
                    case "json":

                        await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "not expected " + financeviewmodel.type);
                        break;
                    case "text":
                        carryon = true;
                        break;
                }

                if (carryon)
                {
                    HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(financeviewmodel.html_document.DocumentNode, "//a");
                    foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                    {
                        classname = element1.GetAttributeValue("class", string.Empty);
                        if (classname == "log-on")  // This may well change!!
                        {
                            name = element1.GetAttributeValue("target", string.Empty);
                            if (name == "_self")    // <= This may well change!!
                            {
                                href = element1.GetAttributeValue("href", string.Empty);
                                href = href.Replace("&amp;", "&");
                                //Log(href);
                                found_it = true;

                                if (found_it && !string.IsNullOrEmpty(href))
                                {
                                    Uri next_url = new Uri(href);

                                    if (await SmartBanksV2019.DoGet_Finance(
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            financeviewmodel.finance_token,
                                                            next_url,
                                                            url_base,
                                                            log,
                                                            "GET",

                                                            referer))

                                    {
                                        // Look for these:
                                        // input type = "hidden" id = "foData" name = "foData" />

                                        await SmartRoutinesV2018.TextBlockUpdate(
                                                                            ourviewmodel,
                                                                            "Received LOGON link" + financeviewmodel.type);
                                        Santander_Get_The_Hidden_Inputs(financeviewmodel.html_document,
                                                        ref foData,
                                                        ref origen,
                                                        ref binddevicePrint,
                                                        ref IdGhostAccount,
                                                        ref urlLog,
                                                        ref ssCookie,
                                                        ref jsessionID,
                                                        ref dse_applicationName,
                                                        ref dse_sessionId,
                                                        ref dse_operationName,
                                                        ref dse_threadId,
                                                        ref dse_pageId,
                                                        ref dse_processorState,
                                                        ref dse_processorId,
                                                        ref dse_cmd,
                                                        ref dse_errorPage,
                                                        ref dse_nextEventName);

                                        HtmlCol2 = YetAnotherFuckingHoop.SelectNodesAsList(financeviewmodel.html_document.DocumentNode, "//input");
                                        foreach (HtmlAgilityPack.HtmlNode element2 in HtmlCol2)
                                        {
                                            string tiepe = element2.GetAttributeValue("type", string.Empty);
                                            if (tiepe == "text")
                                            {
                                                classname = element2.GetAttributeValue("class", string.Empty);
                                                if (classname == "mandatory firstfocus")
                                                {
                                                    name = element2.GetAttributeValue("name", string.Empty);
                                                    if (name == "infoLDAP_E.customerID")
                                                    {

                                                        // Now try and POST to get the digits
                                                        Uri customer_id_url = new Uri("https://retail.santander.co.uk/LOGSUK_NS_ENS/ChannelDriver.ssobto?dse_operationName=LOGON");
                                                        // and to do this post you need the customer_number
                                                        string customer_id = name + "|" + customer + SmartParametersV2016.record_separator +
                                                                                "foData" + "|" + foData + SmartParametersV2016.record_separator +
                                                                                "origen" + "|" + origen + SmartParametersV2016.record_separator +
                                                                                "bind.devicePrint" + "|" + binddevicePrint + SmartParametersV2016.record_separator +
                                                                                "IdGhostAccount" + "|" + IdGhostAccount + SmartParametersV2016.record_separator +
                                                                                "urlLog" + "|" + urlLog + SmartParametersV2016.record_separator +
                                                                                "ssCookie" + "|" + ssCookie + SmartParametersV2016.record_separator +
                                                                                "jsessionID" + "|" + jsessionID;

                                                        var not_empty1 = SmartBanksV2019.Common_Build_Form_Strings_New(customer_id);
                                                        financeviewmodel.response = string.Empty;

                                                        string question = string.Empty,
                                                                answer = string.Empty;

                                                        if (await SmartBanksV2019.DoPost_NewX(
                                                                        ourviewmodel,
                                                                        financeviewmodel,
                                                                        financeviewmodel.finance_token,
                                                                        customer_id_url,
                                                                        url_base,
                                                                        log,
                                                                        1,
                                                                        "POST",
                                                                        not_empty1))

                                                        {
                                                            question = Santander_Look_For_Question(financeviewmodel.html_document);


                                                            if (question == "Father's middle name")
                                                            {
                                                                answer = "DENIS";
                                                            }
                                                            if (question == "Maternal grandfather's first name")
                                                            {
                                                                answer = "RAY";
                                                            }
                                                            if (question == "Mother's middle name")
                                                            {
                                                                answer = "ANN";
                                                            }

                                                            if (!string.IsNullOrEmpty(answer))
                                                            {
                                                                Santander_Get_The_Hidden_Inputs(financeviewmodel.html_document,
                                                                                ref foData,
                                                                                ref origen,
                                                                                ref binddevicePrint,
                                                                                ref IdGhostAccount,
                                                                                ref urlLog,
                                                                                ref ssCookie,
                                                                                ref jsessionID,
                                                                                ref dse_applicationName,
                                                                                ref dse_sessionId,
                                                                                ref dse_operationName,
                                                                                ref dse_threadId,
                                                                                ref dse_pageId,
                                                                                ref dse_processorState,
                                                                                ref dse_processorId,
                                                                                ref dse_cmd,
                                                                                ref dse_errorPage,
                                                                                ref dse_nextEventName);

                                                                // Now try and POST to get the password and passcode
                                                                Uri question_url = new Uri("https://retail.santander.co.uk/LOGSUK_NS_ENS/ChannelDriver.ssobto?dse_operationName=LOGON");
                                                                // and to do this post you need the customer_number
                                                                string challenge = "cbQuestionChallenge.responseUser" + "|" + answer + SmartParametersV2016.record_separator +
                                                                                    "control.bind" + "|" + "N" + SmartParametersV2016.record_separator +
                                                                                    "buttons.1" + "|" + "Continue" + SmartParametersV2016.record_separator +
                                                                                    "dse_applicationName" + "|" + dse_applicationName + SmartParametersV2016.record_separator +
                                                                                    "dse_sessionId" + "|" + dse_sessionId + SmartParametersV2016.record_separator +
                                                                                    "dse_operationName" + "|" + dse_operationName + SmartParametersV2016.record_separator +
                                                                                    "dse_threadId" + "|" + dse_threadId + SmartParametersV2016.record_separator +
                                                                                    "dse_pageId" + "|" + dse_pageId + SmartParametersV2016.record_separator +
                                                                                    "dse_processorState" + "|" + dse_processorState + SmartParametersV2016.record_separator +
                                                                                    "dse_processorId" + "|" + dse_processorId + SmartParametersV2016.record_separator +
                                                                                    "dse_cmd" + "|" + dse_cmd + SmartParametersV2016.record_separator +
                                                                                    "dse_errorPage" + "|" + dse_errorPage + SmartParametersV2016.record_separator +
                                                                                    "urlLog" + "|" + urlLog + SmartParametersV2016.record_separator +
                                                                                    "ssCookie" + "|" + ssCookie + SmartParametersV2016.record_separator +
                                                                                    "dse_nextEventName" + "|" + dse_nextEventName;

                                                                var not_empty2 = SmartBanksV2019.Common_Build_Form_Strings_New(challenge);
                                                                financeviewmodel.response = string.Empty;

                                                                if (await SmartBanksV2019.DoPost_NewX(
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                financeviewmodel.finance_token,
                                                                                question_url,
                                                                                url_base,
                                                                                log,
                                                                                1,
                                                                                "POST",
                                                                                not_empty2))

                                                                {

                                                                    await SmartRoutinesV2018.TextBlockUpdate(

                                                            ourviewmodel,
                                                            "Received LOGON link" + financeviewmodel.type);

                                                                }
                                                            }


                                                            if (Santander_Look_For_Password(financeviewmodel.html_document))
                                                            {
                                                                Santander_Get_The_Hidden_Inputs(financeviewmodel.html_document,
                                                                                ref foData,
                                                                                ref origen,
                                                                                ref binddevicePrint,
                                                                                ref IdGhostAccount,
                                                                                ref urlLog,
                                                                                ref ssCookie,
                                                                                ref jsessionID,
                                                                                ref dse_applicationName,
                                                                                ref dse_sessionId,
                                                                                ref dse_operationName,
                                                                                ref dse_threadId,
                                                                                ref dse_pageId,
                                                                                ref dse_processorState,
                                                                                ref dse_processorId,
                                                                                ref dse_cmd,
                                                                                ref dse_errorPage,
                                                                                ref dse_nextEventName);

                                                                // Now try and POST to get the 'Continue'
                                                                Uri passcode_url = new Uri("https://retail.santander.co.uk/LOGSUK_NS_ENS/ChannelDriver.ssobto?dse_contextRoot=true");
                                                                // and to do this post you need the  passcode and reg no
                                                                string challenge = "authentication.PassCode" + "|" + passcode + SmartParametersV2016.record_separator +
                                                                                    "authentication.ERN" + "|" + ERN + SmartParametersV2016.record_separator +
                                                                                    "buttons.1" + "|" + "Continue" + SmartParametersV2016.record_separator +
                                                                                    "dse_applicationName" + "|" + dse_applicationName + SmartParametersV2016.record_separator +
                                                                                    "dse_sessionId" + "|" + dse_sessionId + SmartParametersV2016.record_separator +
                                                                                    "dse_operationName" + "|" + dse_operationName + SmartParametersV2016.record_separator +
                                                                                    "dse_threadId" + "|" + dse_threadId + SmartParametersV2016.record_separator +
                                                                                    "dse_pageId" + "|" + dse_pageId + SmartParametersV2016.record_separator +
                                                                                    "dse_processorState" + "|" + dse_processorState + SmartParametersV2016.record_separator +
                                                                                    "dse_processorId" + "|" + dse_processorId + SmartParametersV2016.record_separator +
                                                                                    "dse_cmd" + "|" + dse_cmd + SmartParametersV2016.record_separator +
                                                                                    "dse_errorPage" + "|" + dse_errorPage + SmartParametersV2016.record_separator +
                                                                                    "errorHandler.codError" + "|" + "" + SmartParametersV2016.record_separator +
                                                                                    "ssCookie" + "|" + ssCookie + SmartParametersV2016.record_separator +
                                                                                    "dse_nextEventName" + "|" + dse_nextEventName;

                                                                var not_empty2 = SmartBanksV2019.Common_Build_Form_Strings_New(challenge);
                                                                financeviewmodel.response = string.Empty;

                                                                if (await SmartBanksV2019.DoPost_NewX(
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                financeviewmodel.finance_token,
                                                                                passcode_url,
                                                                                url_base,
                                                                                log,
                                                                                1,
                                                                                "POST",
                                                                                not_empty2))


                                                                {

                                                                    // Yay! - we made it now go and work through the list of accounts

                                                                    await SmartRoutinesV2018.TextBlockUpdate(

                                                                                                            ourviewmodel,
                                                                                                        "Required passcode accepted" + financeviewmodel.type);

                                                                    // Should be able to get the list of accounts NOW!!
                                                                    // And I fucking DID!!  (Now, if only I could find my fucking keys ...)
                                                                    string accounts = string.Empty;

#if WINFORMS
                                                                    List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
#else
                                                                    List<FinanceViewModel.AccountItem> these_accounts = new List<FinanceViewModel.AccountItem>();
#endif
                                                                    string owner = string.Empty;
                                                                    // If the accounts is EMPTY!!! Then we haven't
                                                                    // logged in correctly!!!  Check the Big 3 login entries!!
                                                                    Santander_Build_Account_List(financeviewmodel.html_document, ref accounts, ref owner);
                                                                    financeviewmodel.owner = owner;
                                                                    if (!string.IsNullOrEmpty(accounts))
                                                                    {
                                                                        string[] accounts_list = accounts.Split(SmartParametersV2016.record_separator);
                                                                        if (accounts_list.Count() > 0)
                                                                        {
                                                                            string account_name = string.Empty,
                                                                            account_number = string.Empty,
                                                                                    sort_code = string.Empty,
                                                                                    balance = string.Empty;

                                                                            await SmartRoutinesV2018.TextBlockUpdate(

                                                                                                                ourviewmodel,
                                                                                                                "Found " + accounts_list.Count() + "accounts");
                                                                            int account_id = 0;
                                                                            foreach (string account in accounts_list)
                                                                            {
                                                                                account_id = account_id + 1;
                                                                                string[] comp = account.Split(SmartParametersV2016.unit_separator);
                                                                                if (comp.Count() > 1)
                                                                                {
                                                                                    string account_token = comp[0];
                                                                                    if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref account_number))
                                                                                    {
                                                                                        if (SmartFinanceV2021.Backwards_Split(ref account_token, SmartParametersV2016.space, ref sort_code))
                                                                                        {
                                                                                            sort_code = sort_code.Replace("-", string.Empty);
                                                                                            account_name = account_token;
                                                                                            string val = sort_code + SmartParametersV2016.unit_separator +
                                                                                                            account_number +
                                                                                                            ":" +
                                                                                                            "GBP" + // Assume we can find it in OpenBanking
                                                                                                            ":" +
                                                                                                            "£0.00" +
                                                                                                            ":" +
                                                                                                            comp[1].Replace("&amp;", "&");  // Do an 'amp' conversion // Left in for completeness
                                                                                            string[] array = val.Split(':');
#if WINFORMS
                                                                                            FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem
#else
                                                                                            FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem()
#endif
                                                                                            {
                                                                                                Account_ID = account_id,
                                                                                                Content = account_name,
                                                                                                Value = array.ToString()   // Here we have [0] = sort_code and account_no
                                                                                                //              [1] = balance
                                                                                                //              [2] = href <= WHICH ISN'T USED!!!!!
                                                                                                //                    It gets re-established in Find_Transactions
                                                                                            };
                                                                                            these_accounts.Add(account_item);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }

                                                                        }
                                                                    }
#if !WINFORMS
                                                                    financeviewmodel.FinanceAccounts_List = these_accounts;
#endif
                                                                    string next_challenge = "dse_sessionId" + "|" + dse_sessionId + SmartParametersV2016.record_separator +
                                                                                           "dse_operationName" + "|" + "ViewTransactions" + SmartParametersV2016.record_separator +
                                                                                           "dse_applicationName" + "|" + "ALP_EBAN_ViewTransactions" + SmartParametersV2016.record_separator +

                                                                                           "dse_threadId" + "|" + dse_threadId + SmartParametersV2016.record_separator +
                                                                                           "dse_pageId" + "|" + dse_pageId + SmartParametersV2016.record_separator +
                                                                                           "dse_processorState" + "|" + dse_processorState + SmartParametersV2016.record_separator +
                                                                                           "dse_processorId" + "|" + dse_processorId + SmartParametersV2016.record_separator +
                                                                                           "dse_cmd" + "|" + "continue" + SmartParametersV2016.record_separator +
                                                                                           "dse_nextEventName" + "|" + "Next" + SmartParametersV2016.record_separator +
                                                                                           "searchResultList.paginationData.firstSearch" + "|" + "N";
                                                                    financeviewmodel.challenge = next_challenge;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!found_it)
                {

                    await SmartRoutinesV2018.TextBlockUpdate(
                                                            ourviewmodel,
                                                            "Not found");
                }
            }
            return true;
        }

        internal static void Santander_Build_Account_List(HtmlAgilityPack.HtmlDocument document,
                                                            ref string accounts,
                                                            ref string owner)
        {
            IList<HtmlAgilityPack.HtmlNode>
                    HtmlCol3,
                    HtmlCol4,
                    HtmlCol5,
                    HtmlCol6,
                    HtmlCol7;

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//div");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                if (element3.Id == "loginfo")
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//span");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        string classname = element4.GetAttributeValue("class", string.Empty);
                        if (classname == "username")
                        {
                            owner = element4.InnerText.Trim();
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(owner))
                {
                    break;
                }
            }

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, "//ul");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                string classname = element3.GetAttributeValue("class", string.Empty);
                if (classname == "accountlist")
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//li");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//div");
                        foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                        {
                            string html = string.Empty;
                            classname = element5.GetAttributeValue("class", string.Empty);
                            switch (classname)
                            {
                                case "info":
                                    string href = string.Empty;
                                    HtmlCol6 = YetAnotherFuckingHoop.SelectNodesAsList(element5, ".//span");
                                    foreach (HtmlAgilityPack.HtmlNode element6 in HtmlCol6)
                                    {
                                        classname = element6.GetAttributeValue("class", string.Empty);
                                        switch (classname)
                                        {
                                            case "name":
                                                if (!string.IsNullOrEmpty(accounts))
                                                {
                                                    accounts = accounts +
                                                    SmartParametersV2016.record_separator;
                                                }
                                                if (!string.IsNullOrEmpty(element6.InnerText))
                                                {
                                                    accounts = accounts + element6.InnerText;
                                                }
                                                HtmlCol7 = YetAnotherFuckingHoop.SelectNodesAsList(element6, ".//a");
                                                foreach (HtmlAgilityPack.HtmlNode element7 in HtmlCol7)
                                                {
                                                    href = element7.GetAttributeValue("href", string.Empty);
                                                    href = href.Replace("&amp;", "&");
                                                }
                                                break;
                                            case "number":
                                                if (!string.IsNullOrEmpty(element6.InnerText))
                                                {
                                                    accounts = accounts + SmartParametersV2016.space +
                                                                    element6.InnerText +
                                                                    SmartParametersV2016.unit_separator +
                                                                    href;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                case "balance":
                                    break;
                                case "actions":
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return;
        }

        internal static string Santander_Look_For_Question(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol3,
                                            HtmlCol4,
                                            HtmlCol5;
            string question = string.Empty,
                    classname = string.Empty;

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//form");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                if (element3.Id == "formCustomerID")
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        classname = element4.GetAttributeValue("class", string.Empty);
                        if (classname == "form-item")
                        {
                            bool found_question = false;
                            HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//span");
                            foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                            {
                                classname = element5.GetAttributeValue("class", string.Empty);
                                if (classname == "labeltext")
                                {
                                    if (!string.IsNullOrEmpty(element5.InnerText))
                                    {
                                        if (element5.InnerText.IndexOf("Question") >= 0)
                                        {
                                            found_question = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (classname == "data" && found_question)
                                    {
                                        if (!string.IsNullOrEmpty(element5.InnerText))
                                        {
                                            question = element5.InnerText.Replace("&#39;", "'").Trim();
                                            return question;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return question;
        }

        internal static string Santander_Look_For_Button_Value(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol3;

            string button1_value = string.Empty,
                    tiepe = string.Empty,
                    name = string.Empty;
            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//input");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                tiepe = element3.GetAttributeValue("type", string.Empty);
                if (tiepe == "submit")
                {
                    name = element3.GetAttributeValue("name", string.Empty);
                    if (name == "buttonsInterstitial.events.1")
                    {
                        button1_value = element3.GetAttributeValue("value", string.Empty);
                        return button1_value;
                    }
                }
                if (!string.IsNullOrEmpty(button1_value))
                {
                    break;
                }
            }
            return button1_value;
        }

        internal static bool Santander_Look_For_Password(HtmlAgilityPack.HtmlDocument document)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol3,
                                            HtmlCol4,
                                            HtmlCol5;
            string classname = string.Empty,
                    tiepe = string.Empty;

            HtmlCol3 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//form");
            foreach (HtmlAgilityPack.HtmlNode element3 in HtmlCol3)
            {
                if (element3.Id == "formAuthenticationAbbey")
                {
                    HtmlCol4 = YetAnotherFuckingHoop.SelectNodesAsList(element3, ".//div");
                    foreach (HtmlAgilityPack.HtmlNode element4 in HtmlCol4)
                    {
                        classname = element4.GetAttributeValue("class", string.Empty);
                        if (classname == "form-item")
                        {
                            HtmlCol5 = YetAnotherFuckingHoop.SelectNodesAsList(element4, ".//input");
                            foreach (HtmlAgilityPack.HtmlNode element5 in HtmlCol5)
                            {
                                tiepe = element5.GetAttributeValue("type", string.Empty);
                                if (tiepe == "password" &&
                                    element5.Id == "authentication.PassCode")
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static void Santander_Get_The_Hidden_Inputs(HtmlAgilityPack.HtmlDocument document,
                                    ref string foData,
                                    ref string origen,
                                    ref string binddevicePrint,
                                    ref string IdGhostAccount,
                                    ref string urlLog,
                                    ref string ssCookie,
                                    ref string jsessionID,
                                    ref string dse_applicationName,
                                    ref string dse_sessionId,
                                    ref string dse_operationName,
                                    ref string dse_threadId,
                                    ref string dse_pageId,
                                    ref string dse_processorState,
                                    ref string dse_processorId,
                                    ref string dse_cmd,
                                    ref string dse_errorPage,
                                    ref string dse_nextEventName)
        {
            IList<HtmlAgilityPack.HtmlNode> HtmlCol1;
            string name = string.Empty;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(document.DocumentNode, ".//input");
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
            {
                string tiepe = element1.GetAttributeValue("type", string.Empty);
                if (tiepe == "hidden")
                {
                    name = element1.GetAttributeValue("name", string.Empty);
                    switch (name)
                    {
                        case "foData":  // Forgotten Data ??
                            foData = element1.GetAttributeValue("value", string.Empty);
                            //Log("foData: " + foData);
                            break;
                        case "origen":
                            origen = element1.GetAttributeValue("value", string.Empty);
                            //Log("origen: " + origen);
                            break;
                        case "bind.devicePrint":
                            binddevicePrint = element1.GetAttributeValue("value", string.Empty);
                            //Log("bind.devicePrint: " + binddevicePrint);
                            break;
                        case "IdGhostAccount":
                            IdGhostAccount = element1.GetAttributeValue("value", string.Empty);
                            //Log("IdGhostAccount: " + IdGhostAccount);
                            break;
                        case "urlLog":
                            urlLog = element1.GetAttributeValue("value", string.Empty);
                            //Log("urlLog: " + urlLog);
                            break;
                        case "ssCookie":
                            ssCookie = element1.GetAttributeValue("value", string.Empty);
                            //Log("ssCookie: " + ssCookie);
                            break;
                        case "jsessionID":
                            jsessionID = element1.GetAttributeValue("value", string.Empty);
                            //Log("jsessionID: " + jsessionID);
                            break;

                        case "dse_applicationName":  // Forgotten Data ??
                            dse_applicationName = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_applicationName: " + dse_applicationName);
                            break;
                        case "dse_sessionId":
                            dse_sessionId = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_sessionId: " + dse_sessionId);
                            break;
                        case "dse_operationName":
                            dse_operationName = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_operationName: " + dse_operationName);
                            break;
                        case "dse_threadId":
                            dse_threadId = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_threadId: " + dse_threadId);
                            break;
                        case "dse_pageId":
                            dse_pageId = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_pageId: " + dse_pageId);
                            break;
                        case "dse_processorState":
                            dse_processorState = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_processorState: " + dse_processorState);
                            break;
                        case "dse_processorId":
                            dse_processorId = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_processorId: " + dse_processorId);
                            break;
                        case "dse_cmd":
                            dse_cmd = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_cmd: " + dse_cmd);
                            break;
                        case "dse_errorPage":
                            dse_errorPage = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_errorPage: " + dse_errorPage);
                            break;
                        case "dse_nextEventName":
                            dse_nextEventName = element1.GetAttributeValue("value", string.Empty);
                            //Log("dse_nextEventName: " + dse_nextEventName);
                            break;
                        default:
                            break;
                    }
                }
            }
            return;
        }
    }
}