using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartCubeMobile.SmartFinance;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
#if WINFORMS
using SmartDashboard;
using System.Windows.Forms;
#endif

namespace SmartCubeMobile
{
    public static class RoyalBankofScotlandV2021
    {
        internal static async Task<bool> SetUp(
#if WINFORMS
                                        RichTextBox textBoxConsole,
#endif
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        string organization_id,
                                        string Client_Id,
                                        string Client_Secret,
                                        string url_base,
                                        string Redirect_Uri,
                                        short institution_code,
                                        short brand_code)
        //List<Bank> banksList,
        //List<SmartFinance.Accounts> accountsList,
        //List<SmartFinance.BankTransactions> transactionsList)
        {
            CancellationToken cancel_token = new CancellationToken();
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "RBS Open Banking Sandbox", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "RBS Open Banking Sandbox");

            string CustomerId = @"1234567890";
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "Customer Id: " + CustomerId, MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
            string AuthorizationUsername = @"djefferson@f96e79b7-1a96-4cdc-82d4-d79df7557002.example.org";
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "Authorization Username: " + AuthorizationUsername, MainProcess.Mally.examine.scrape, MainProcess.Mally.console);

            ourviewmodel.BankingSite = "Checking";
            //if (SmartBobV2017.Ping(url_base))
            //{
            ourviewmodel.BankingSite = "Open";
#endif
            if (!await Start(
#if WINFORMS
                        textBoxConsole,
#endif
                        ourviewmodel,
                     financeviewmodel,
                     Client_Id,
                     Client_Secret,
                     organization_id,
                     url_base,
                     Redirect_Uri,
                     AuthorizationUsername,
                     CustomerId,
                     cancel_token,
                     institution_code,
                     brand_code))
            {
                return false;
            }
            //banksList)
            //accountsList,
            //transactionsList);
            //ourviewmodel.BankingSite = "Closed";
            //}
            //else
            //{
            //    ourviewmodel.BankingSite = "Closed";
            //}
            return true;
        }

        internal static async Task<bool> Start(
#if WINFORMS
                                                RichTextBox textBoxConsole,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string Client_Id,
                                                string Client_Secret,
                                                string organization_id,
                                                string url_base,
                                                string Redirect_Uri,
                                                string AuthorizationUsername,
                                                string CustomerId,
                                                CancellationToken cancel_token,
                                                short institution_code,
                                                short brand_code)
        //List<Bank> banksList,
        //List<SmartFinance.Accounts> accountsList,
        //List<SmartFinance.BankTransactions> transactionsList)
        {
            // Setup Account Access Consent
            // Retrieve Access Token (POST)
#if WINFORMS
            MainProcess.Output_Message(textBoxConsole, "Step 1: Account Access", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 1: Account Access");

            financeviewmodel.rbs_access_token = string.Empty;

            short ordinal = 1;
            string targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                            financeviewmodel,
                                                                            ordinal,
                                                                            institution_code,
                                                                            brand_code);

            Uri token_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                        url_base,
                                                        targetUrl);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            if (! //await
                  RBS_ACCOUNT_ACCESS(ourviewmodel,
                                        financeviewmodel,
                                        token_uri,
                                        Client_Id,
                                        Client_Secret,
                                        cancel_token))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(financeviewmodel.rbs_access_token))
            {
                // Retrieve Account Access Consent (POST)

                // Increment the ordinal
                ordinal++;

#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 2: Account Access Consent", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 2: Account Access Consent");


                targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                        financeviewmodel,
                                                                        ordinal,
                                                                        institution_code,
                                                                        brand_code);

                Uri account_access_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                                    url_base,
                                                                    targetUrl);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }
                if (!await RBS_ACCOUNT_ACCESS_CONSENT(ourviewmodel,
                                    financeviewmodel,
                                    account_access_uri,
                                    organization_id,
                                    cancel_token))
                {
                    return false;
                }
                //string Redirect_Uri = "https://www.keasdon.co.uk/redirect";
                string Consent_Id = financeviewmodel.rbs_consentid;
                // Send Account Authorize Consent (GET)
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 3: Account Authorize Consent", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 3: Account Authorize Consent");

                // Increment the ordinal
                ordinal++;

                targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                        financeviewmodel,
                                                                        ordinal,
                                                                        institution_code,
                                                                        brand_code);
                Uri account_authorize_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                                        url_base,
                                                                        targetUrl);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }

                if (!await RBS_ACCOUNT_AUTHORIZE_CONSENT(ourviewmodel,
                                                        financeviewmodel,
                                                        account_authorize_uri,
                                                        Client_Id,
                                                        Redirect_Uri,
                                                        Consent_Id,
                                                        AuthorizationUsername,
                                                        CustomerId,
                                                        cancel_token))
                {
                    return false;
                }
                // Exchange Authorization Code for Access Token (POST)
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 4: Authorization Code Exchange", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 4: Authorization Code Exchange");

                // Increment the ordinal
                ordinal++;

                targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                        financeviewmodel,
                                                                        ordinal,
                                                                        institution_code,
                                                                        brand_code);
                Uri authorize_code_exchange_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                                            url_base,
                                                                            targetUrl);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }

                if (!await RBS_AUTHORIZE_CODE_EXCHANGE(ourviewmodel,
                                                    financeviewmodel,
                                                    authorize_code_exchange_uri,
                                                    Client_Id,
                                                    Client_Secret,
                                                    Redirect_Uri,
                                                    financeviewmodel.rbs_authorization_code,
                                                    cancel_token))
                {
                    return false;
                }

                // Send Account Details To Actually Retirve The Fucking Data (GET)
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 5: Account Data Request", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 5: Account Data Request");

                // Increment the ordinal
                ordinal++;

                targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                        financeviewmodel,
                                                                        ordinal,
                                                                        institution_code,
                                                                        brand_code);
                Uri request_data_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                                    url_base,
                                                                    targetUrl);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }

                List<string> accountsList = new List<string>();
                if (!await RBS_ACCOUNT_REQUEST_DATA(ourviewmodel,
                                                        financeviewmodel,
                                                        request_data_uri,
                                                        financeviewmodel.rbs_access_token,
                                                        accountsList,
                                                        cancel_token,
                                                        institution_code,
                                                        brand_code))
                {
                    return false;
                }
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 6: Account Transactions", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 6: Account Transactions");

                // Increment the ordinal
                ordinal++;

                targetUrl = SmartSpikeFinanceV2017.Finance_LookupUrl_Ordinal(ourviewmodel,
                                                                        financeviewmodel,
                                                                        ordinal,
                                                                        institution_code,
                                                                        brand_code);


                foreach (string account_id in accountsList)
                {
                    Uri request_transactions_uri = SmartNibbyV2016.Return_Uri(ourviewmodel,
                                                                                url_base,
                                                                                targetUrl + account_id + "/transactions");
                    if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                    {
                        return false;
                    }

                    // Account Id Transactions
                    if (!await RBS_ACCOUNT_REQUEST_TRANSACTIONS(ourviewmodel,
                                                    financeviewmodel,
                                                    request_transactions_uri,
                                                    financeviewmodel.rbs_access_token,
                                                    //transactionsList,
                                                    cancel_token,
                                                    institution_code,
                                                    brand_code))
                    {
                        return false;
                    }
                    else
                    {
#if WINFORMS
                        MainProcess.Output_Message(textBoxConsole, "Processed Account Id: " + account_id, MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Processed Account Id: " + account_id);

                    }
                }
#if WINFORMS
                MainProcess.Output_Message(textBoxConsole, "Step 7: Finished!", MainProcess.Mally.examine.scrape, MainProcess.Mally.console);
#endif
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Step 7: Finished!");

            }
            return true;
        }

        internal static
            //async Task<bool>
            bool RBS_ACCOUNT_ACCESS(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri account_access_uri,
                                                    string CLIENT_ID,
                                                    string CLIENT_SECRET,
                                                    CancellationToken cancel_token)
        {
            financeviewmodel.errorMessage = string.Empty;

            string[] param = new string[4];
            param[0] = "grant_type" + "|" + "client_credentials";
            param[1] = "client_id" + "|" + CLIENT_ID;
            param[2] = "client_secret" + "|" + CLIENT_SECRET;
            param[3] = "scope" + "|" + "accounts";

            string result = string.Empty;
            foreach (string vals in param)
            {
                string[] fields = vals.Split('|');
                if (fields.Length == 2)
                {
                    if (result.Length > 0)
                    {
                        result += "&";
                    }
                    string fields1 = WebUtility.UrlEncode(fields[1]);
                    if (fields1.Contains("("))
                    {
                        fields1 = fields1.Replace("(", "%28");
                    }
                    if (fields1.Contains(")"))
                    {
                        fields1 = fields1.Replace(")", "%29");
                    }
                    result = result + fields[0] + "=" + fields1;
                }
            }
            string stringFormParams = result;

            bool status = true; // await SmartBobV2017.RBS_POST_ACCESS(ourviewmodel,
                                //                    financeviewmodel,
                                //                    account_access_uri,
                                //                    SmartParametersV2016.timespanTimeout,
                                //                    string.Empty,
                                //                    stringFormParams,
                                //                    cancel_token);
            if (status)
            {
                try
                {
                    string token_type = string.Empty;
                    string scope = string.Empty;

                    dynamic jsonResponse = JObject.Parse(financeviewmodel.obs_post_result);
                    foreach (dynamic statement in jsonResponse)
                    {
                        string column = statement.Name;
                        switch (column)
                        {
                            case "token_type":
                                token_type = statement.Value;
#if WINFORMS
                                //("Token Type: " + token_type);
#endif
                                break;
                            case "access_token":
                                financeviewmodel.rbs_access_token = statement.Value;
#if WINFORMS
                                //("Access Token: " + financeviewmodel.rbs_access_token.Substring(0, 20));
#endif
                                break;
                            case "expires_in":
#if WINFORMS
                                //("Expires in: " + statement.Value + "secs");
#endif
                                break;
                            case "scope":
                                scope = statement.Value;
#if WINFORMS
                                //("Scope: " + scope);
#endif
                                break;
                            default:
                                break;
                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            return status;
        }

        internal static async Task<bool> RBS_ACCOUNT_ACCESS_CONSENT(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            Uri account_access_uri,
                                                            string organization_id,
                                                            CancellationToken cancel_token)
        {
            bool status = false;    // Assume failure first
            try
            {
                //Access_Consent access_consent = JsonConvert.DeserializeObject<Access_Consent>(ourviewmodel.jsonString);
                if (ourviewmodel.trace)
                {
#if WINFORMS
                    //(string.Concat("Access Consent ", access_consent));
#endif
                }
                // Submit Account Access Consent
                string data = string.Empty;
                string creationdatetime = string.Empty;
                string datastatus = string.Empty;
                string datastatusupdatedatetime = string.Empty;
                string links = string.Empty;
                string self = string.Empty;
                string meta = string.Empty;
                string risk = string.Empty;
                string totalpages = string.Empty;
                try
                {
                    Uri targetUrl = new Uri(@"Dummy");
                    CancellationToken canc = new CancellationToken();
                    List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
                    List<string> headers = new List<string>();
                    HtmlDocument html = await SmartBobV2017.HTTPCLIENT_POST_ASYNC(ourviewmodel,
                                                            targetUrl,
                                                            canc,
                                                            keys,
                                                            headers);
                    if (html != null)
                    {
                        try
                        {
                            dynamic jsonResponse1 = JObject.Parse(financeviewmodel.obs_post_result);
                            foreach (dynamic statement in jsonResponse1)
                            {
                                string column = statement.Name;
                                switch (column)
                                {
                                    case "Data":
                                        if (ourviewmodel.trace)
                                        {
#if WINFORMS
                                            //("Data: " + statement.Value);
#endif
                                        }
                                        foreach (dynamic data_view in statement.Value)
                                        {
                                            string data_column = data_view.Name;
                                            switch (data_column)
                                            {
                                                case "ConsentId":
                                                    financeviewmodel.rbs_consentid = data_view.Value;
#if WINFORMS
                                                    //("Consent Id: " + financeviewmodel.rbs_consentid);
#endif
                                                    break;
                                                case "CreationDateTime":
                                                    creationdatetime = data_view.Value;
                                                    break;
                                                case "Status":
                                                    datastatus = data_view.Value;
                                                    break;
                                                case "StatusUpdateDateTime":
                                                    datastatusupdatedatetime = data_view.Value;
                                                    break;
                                                case "Permissions":
                                                    string permissions = string.Empty;
#if WINFORMS
                                                    Console.Write("Permissions: ");
#endif
                                                    foreach (dynamic permissions_view in data_view.Value)
                                                    {
                                                        if (!string.IsNullOrEmpty(permissions))
                                                        {
                                                            permissions += ",";
                                                        }
                                                        permissions += permissions_view.ToString();
                                                    }
#if WINFORMS
                                                    //(permissions.ToString());
#endif
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "Links":
                                        foreach (dynamic links_view in statement.Value)
                                        {
                                            string links_column = links_view.Name;
                                            switch (links_column)
                                            {
                                                case "Self":
                                                    self = links_view.Value;
#if WINFORMS
                                                    //("Self: " + self);
#endif
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "Meta":
                                        foreach (dynamic meta_view in statement.Value)
                                        {
                                            string meta_column = meta_view.Name;
                                            switch (meta_column)
                                            {
                                                case "TotalPages":
                                                    totalpages = meta_view.Value;
#if WINFORMS
                                                    //("TotalPages: " + totalpages);
#endif
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "Risk":
                                        break;
                                    default:
                                        break;
                                }
                            }
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            ourviewmodel.errorMessage = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = "Bad json: " + ex.Message + " " + financeviewmodel.obs_post_result;
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Bad json: " + ex.Message + " " + ourviewmodel.jsonString;
            }
            return status;
        }

        internal static async Task<bool> RBS_ACCOUNT_AUTHORIZE_CONSENT(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri account_authorize_uri,
                                                    string CLIENT_ID,
                                                    string REDIRECT_URI,
                                                    string CONSENT_ID,
                                                    string AuthorizationUsername,
                                                    string CustomerId,
                                                    CancellationToken cancel_token)
        {
            financeviewmodel.errorMessage = string.Empty;

            // It would seem that the redirect_uri = "https://www.keasdon.co.uk/redirect"
            // parameter is STILL required
            // even though the authorization_mode = "AUTO_POSTMAN"
            // gives an automatic completion! 
            // I don't understand this shit!!  Why would you insist on a parameter
            // that you are never going to use?
            string[] param = new string[9];
            param[0] = "client_id" + "|" + CLIENT_ID;
            param[1] = "response_type" + "|" + "code id_token";
            param[2] = "scope" + "|" + "openid accounts";
            param[3] = "redirect_uri" + "|" + REDIRECT_URI;
            param[4] = "request" + "|" + CONSENT_ID;
            param[5] = "authorization_mode" + "|" + "AUTO_POSTMAN";
            param[6] = "authorization_result" + "|" + "APPROVED";
            param[7] = "authorization_username" + "|" + AuthorizationUsername;
            param[8] = "authorization_accounts" + "|" + "*";
            //"123456789012@YOUR_DOMAIN"; // {test-user-username}@{your-team-domain}";

            string result = string.Empty;
            foreach (string vals in param)
            {
                string[] fields = vals.Split('|');
                if (fields.Length == 2)
                {
                    if (result.Length > 0)
                    {
                        result += "&";
                    }
                    string fields1 = WebUtility.UrlEncode(fields[1]);
                    if (fields1.Contains("("))
                    {
                        fields1 = fields1.Replace("(", "%28");
                    }
                    if (fields1.Contains(")"))
                    {
                        fields1 = fields1.Replace(")", "%29");
                    }
                    result = result + fields[0] + "=" + fields1;
                }
            }

            Uri token_uri = new Uri(account_authorize_uri.ToString() + "?" + result);
            bool status = await RBS_GET_AUTHORIZE(//ourviewmodel,
                                                    financeviewmodel,
                                                     token_uri,
                                                    //SmartParametersV2016.timespanTimeout,
                                                    string.Empty,
                                                    cancel_token);
            if (status)
            {
                try
                {
                    dynamic jsonResponse = JObject.Parse(financeviewmodel.obs_post_result);
                    foreach (dynamic statement in jsonResponse)
                    {
                        string column = statement.Name;
                        switch (column)
                        {
                            case "redirectUri":
                                string redirect = statement.Value.ToString();
                                redirect = redirect.Replace(REDIRECT_URI, string.Empty);
                                if (ourviewmodel.trace)
                                {
#if WINFORMS
                                    //("RedirectUri: " + redirect);
#endif
                                }
                                string[] paramsx = redirect.Split('&');
                                if (paramsx.Length >= 2)
                                {
                                    int count = 0;
                                    foreach (string param_string in paramsx)
                                    {
                                        switch (count)
                                        {
                                            case 0:
                                                string[] code = param_string.Split('=');
                                                if (code.Length >= 2)
                                                {
                                                    if (code[0] == "#code")
                                                    {
                                                        financeviewmodel.rbs_authorization_code = code[1];
#if WINFORMS
                                                        //("Authorization code: " + financeviewmodel.rbs_authorization_code.Substring(0, 20));
#endif
                                                    }
                                                }
                                                break;
                                            case 1:
                                                string[] token = param_string.Split('=');
                                                if (token.Length >= 2)
                                                {
                                                    if (token[0] == "id_token")
                                                    {
                                                        financeviewmodel.rbs_id_token = token[1];
#if WINFORMS
                                                        //("Id Token: " + financeviewmodel.rbs_id_token.Substring(0, 20));
#endif
                                                    }
                                                }
                                                break;
                                            default:
                                                break;

                                        }
                                        count += 1;
                                    }
                                }
                                break;
                            default:
                                break;

                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            return status;
        }

        internal static async Task<bool> RBS_AUTHORIZE_CODE_EXCHANGE(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri authorize_code_exchange_uri,
                                                    string CLIENT_ID,
                                                    string CLIENT_SECRET,
                                                    string REDIRECT_URI,
                                                    string AUTHORIZATION_CODE,
                                                    CancellationToken cancel_token)
        {
            financeviewmodel.errorMessage = string.Empty;

            string[] param = new string[5];
            param[0] = "client_id" + "|" + CLIENT_ID;
            param[1] = "client_secret" + "|" + CLIENT_SECRET;
            param[2] = "redirect_uri" + "|" + REDIRECT_URI;
            param[3] = "grant_type" + "|" + "authorization_code";
            param[4] = "code" + "|" + AUTHORIZATION_CODE;

            string result = string.Empty;
            foreach (string vals in param)
            {
                string[] fields = vals.Split('|');
                if (fields.Length == 2)
                {
                    if (result.Length > 0)
                    {
                        result += "&";
                    }
                    string fields1 = WebUtility.UrlEncode(fields[1]);
                    if (fields1.Contains("("))
                    {
                        fields1 = fields1.Replace("(", "%28");
                    }
                    if (fields1.Contains(")"))
                    {
                        fields1 = fields1.Replace(")", "%29");
                    }
                    result = result + fields[0] + "=" + fields1;
                }
            }
            string stringFormParams = result;

            bool status = true;
            Uri targetUrl = new Uri(@"Dummy");
            CancellationToken canc = new CancellationToken();
            List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
            List<string> headers = new List<string>();
            HtmlDocument html = await SmartBobV2017.HTTPCLIENT_POST_ASYNC(ourviewmodel,
                                                    targetUrl,
                                                    canc,
                                                    keys,
                                                    headers);
            if (html != null)// await SmartBobV2017.RBS_POST_ACCESS(ourviewmodel,
                             //                   financeviewmodel,
                             //                    authorize_code_exchange_uri,
                             //                   SmartParametersV2016.timespanTimeout,
                             //                   string.Empty,
                             //                   stringFormParams,
                             //                   cancel_token);
                             //if (status)
            {
                try
                {
                    string refresh_token = string.Empty;
                    string token_type = string.Empty;
                    string id_token = string.Empty;
                    string scope = string.Empty;

                    dynamic jsonResponse = JObject.Parse(financeviewmodel.obs_post_result);
                    foreach (dynamic statement in jsonResponse)
                    {
                        string column = statement.Name;
                        switch (column)
                        {
                            case "refresh_token":
                                refresh_token = statement.Value;
#if WINFORMS
                                //("Refresh Token: " + refresh_token.Substring(0, 20));
#endif
                                break;
                            case "token_type":
                                token_type = statement.Value;
#if WINFORMS
                                //("Token Type: " + token_type);
#endif
                                break;
                            case "access_token":
                                financeviewmodel.rbs_access_token = statement.Value;
#if WINFORMS
                                //("Access Token: " + financeviewmodel.rbs_access_token.Substring(0, 20));
#endif
                                break;
                            case "id_token":
                                id_token = statement.Value;
#if WINFORMS
                                //("Id Token: " + id_token.Substring(0, 20));
#endif
                                break;
                            case "expires_in":
#if WINFORMS
                                //("Expires in: " + statement.Value + "secs");
#endif
                                break;
                            case "scope":
                                scope = statement.Value;
#if WINFORMS
                                //("Scope: " + scope);
#endif
                                break;
                            default:
                                break;
                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            return status;
        }

        internal static async Task<bool> RBS_ACCOUNT_REQUEST_DATA(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri request_data_uri,
                                                    string ACCESS_TOKEN,
                                                    List<string> accountsList,
                                                    CancellationToken cancel_token,
                                                    short institution_code,
                                                    short brand_code)
        {
            financeviewmodel.errorMessage = string.Empty;

            bool status = await RBS_GET_AUTHORIZE(//ourviewmodel,
                                                    financeviewmodel,
                                                    request_data_uri,
                                                    //SmartParametersV2016.timespanTimeout,
                                                    ACCESS_TOKEN,
                                                    cancel_token);
            if (status)
            {
                try
                {
                    string account_id = string.Empty;
                    string currency = string.Empty;
                    string account_type = string.Empty;
                    string account_subtype = string.Empty;
                    string description = string.Empty;
                    string nickname = string.Empty;
                    string self = string.Empty;
                    string totalpages = string.Empty;

                    FieldInfo[] fields = typeof(SmartFinance.Accounts).GetFields(SmartParametersV2016.bindingFlags);
                    string[] ingore_fields = "RANDOMKEY1|SEQUENCE_NO|RANDOMKEY2".Split('|');

                    dynamic jsonResponse1 = JObject.Parse(financeviewmodel.obs_post_result);
                    foreach (dynamic statement in jsonResponse1)
                    {
                        string column = statement.Name;
                        switch (column)
                        {
                            case "Data":
                                if (ourviewmodel.trace)
                                {
#if WINFORMS
                                    //("Data: " + statement.Value);
#endif
                                }
                                foreach (dynamic data_view in statement.Value)
                                {
                                    string data_column = data_view.Name;
                                    switch (data_column)
                                    {
                                        case "Account":
                                            foreach (dynamic main_account_view in data_view.Value)
                                            {
                                                int randomkey1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                                                int randomkey2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);

                                                SmartFinance.Accounts account = SmartFinanceV2025.Account_Template(ourviewmodel,
                                                                                                                    financeviewmodel,
                                                                                                                    institution_code,
                                                                                                                    brand_code,
                                                                                                                    
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    SmartParametersV2016.defaultDate,
                                                                                                                    'B',
                                                                                                                    0,
                                                                                                                    "",
                                                                                                                    0,
                                                                                                                    'O');


                                                string ACCOUNT_ID = string.Empty;
                                                foreach (dynamic account_view in main_account_view)
                                                {
                                                    string account_column = account_view.Name;
                                                    switch (account_column)
                                                    {
                                                        case "AccountId":
                                                            account_id = account_view.Value;
#if WINFORMS
                                                            //("Account Id: " + account_id);
#endif
                                                            ACCOUNT_ID = account_id; // account.ACCOUNT_ID = account_id;
                                                            // So we can loop around the Transactions
                                                            accountsList.Add(account_id);
                                                            break;
                                                        case "Currency":
                                                            currency = account_view.Value;
#if WINFORMS
                                                            //("Currency: " + currency);
#endif
                                                            account.CURRENCY_ORDINAL = Convert.ToInt16(currency);
                                                            break;
                                                        case "AccountType":
                                                            account_type = account_view.Value;
#if WINFORMS
                                                            //("Account Type: " + account_type);
#endif
                                                            account.CATEGORY_CODE = Convert.ToChar(account_type.Substring(0, 1));
                                                            break;
                                                        case "AccountSubType":
                                                            account_subtype = account_view.Value;
#if WINFORMS
                                                            //("Account SubType: " + account_subtype);
#endif
                                                            //account.ACCOUNT_SUBTYPE = account_subtype;
                                                            break;
                                                        case "Description":
                                                            description = account_view.Value;
#if WINFORMS
                                                            //("Description: " + description);
#endif
                                                            //account.DESCRIPTION = description;
                                                            break;
                                                        case "Nickname":
                                                            nickname = account_view.Value;
#if WINFORMS
                                                            //("Nickname: " + nickname);
#endif
                                                            //account.NICKNAME = nickname;
                                                            break;
                                                        case "Account":
                                                            string account_info = string.Empty;
#if WINFORMS
                                                            Console.Write("Account: ");
#endif
                                                            foreach (dynamic accounts_view in account_view.Value)
                                                            {
                                                                foreach (dynamic accounts_sub_view in accounts_view)
                                                                {
                                                                    if (!string.IsNullOrEmpty(account_info))
                                                                    {
                                                                        account_info += ",";
                                                                    }
                                                                    account_info += accounts_sub_view.ToString();
                                                                }
                                                            }
#if WINFORMS
                                                            //(account_info.ToString());
#endif
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                string sortcode = string.Empty;
                                                string account_no = string.Empty;
                                                ObservableCollection<SmartFinance.Accounts> accounts_found = null;
                                                //SmartSpikeFinanceV2017.Finance_Lookup_Accounts_New(ourviewmodel,
                                                //            financeviewmodel,
                                                //            institution_code,
                                                //            brand_code,
                                                //            //account_created,
                                                //            //account_id);
                                                //            sortcode,
                                                //            account_no);
                                                if (accounts_found.Count > 0)
                                                {
                                                    if (!SmartBanksV2023.Compare_Transactions<Accounts>(fields,
                                                                                                accounts_found[0],
                                                                                                account,
                                                                                                ingore_fields,
                                                                                                9)) // Start at CURRENCY

                                                    {
                                                        // One of the fields has changed
                                                        account.Updated = true; // Its there and we intend to update it
                                                                                // Because KEY_DETAILS is a Key, we will *not* update the RandomKey
                                                                                // used to generate it otherwise the Keys would not be consistent!
                                                                                // So set this back in
                                                        account.RANDOMKEY1 = accounts_found[0].RANDOMKEY1;
                                                        // Add in updated account
                                                        //financeviewmodel.PLO.finance_accounts_changesList.Add(account);
                                                    }
                                                }
                                                else
                                                {
                                                    // Its goes in changes in case we need to tell the Mothership
                                                    //financeviewmodel.PLO.finance_accounts_changesList.Add(account);
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Links":
                                foreach (dynamic links_view in statement.Value)
                                {
                                    string links_column = links_view.Name;
                                    switch (links_column)
                                    {
                                        case "Self":
                                            self = links_view.Value;
                                            //("Self: " + self);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Meta":
                                foreach (dynamic meta_view in statement.Value)
                                {
                                    string meta_column = meta_view.Name;
                                    switch (meta_column)
                                    {
                                        case "TotalPages":
                                            totalpages = meta_view.Value;
                                            //("TotalPages: " + totalpages);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            //case "Risk": <= Doesn't appear to be in Accounts??
                            //    break;
                            default:
                                break;
                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            return status;
        }

        internal static async Task<bool> RBS_ACCOUNT_REQUEST_TRANSACTIONS(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri request_transaction_uri,
                                                    string ACCESS_TOKEN,
                                                    //List<SmartFinance.BankTransactions> transactionsList,
                                                    CancellationToken cancel_token,
                                                    short institution_code,
                                                    short brand_code)
        {
            financeviewmodel.errorMessage = string.Empty;

            bool status = await RBS_GET_AUTHORIZE(//ourviewmodel,
                                                    financeviewmodel,
                                                    request_transaction_uri,
                                                    //SmartParametersV2016.timespanTimeout,
                                                    ACCESS_TOKEN,
                                                    cancel_token);
            if (status)
            {
                try
                {
                    string account_id = string.Empty;
                    string transaction_id = string.Empty;
                    string transaction_status = string.Empty;
                    string transaction_information = string.Empty;
                    string amount = string.Empty;
                    string currency = string.Empty;
                    string transaction_code = string.Empty;
                    string creditdebit_indicator = string.Empty;
                    //bool balance_type = false;
                    string balance_amount = string.Empty;
                    string balance_currency = string.Empty;
                    string balance_creditdebit_indicator = string.Empty;

                    string self = string.Empty;
                    string first = string.Empty;
                    string last = string.Empty;
                    string totalpages = string.Empty;

                    financeviewmodel.booking_datetime = string.Empty;

                    // When we Sort Transactions, we sort them on 
                    // Booking date and Sequence No (which is always unique)
                    // So suppose we do a suck on 29-Apr-2021 at 10:15:59
                    // we get:
                    // Suck Millisecs   Booking Date    Amount
                    // 1234567891       2020-08-14      £12.50
                    // 1234567892       2020-08-14      £ 8.60
                    // 1234567893       2020-08-14      £ 2.25
                    // 1234567894       2020-08-14      £30.00
                    // Then we  suck later that day at 10:20:59
                    // 1234567901       2020-08-14      £ 6.50
                    // 1234567902       2020-08-14      £ 9.60
                    // 1234567903       2020-08-14      £22.25
                    // 1234567904       2020-08-14      £ 3.00
                    //
                    // If we JUST sort them by Booking Date
                    // we get duplicates, but if we sort them by
                    // Booking Date and Suck Millisecs or
                    // Suck Millisecs and Booking Date
                    // ... they always come out in the 'right order ...
                    // (well - that's the theory!)
                    // This all relies on the fact that it takes MORE THAN ONE
                    // millisecond to record a Transaction! 

                    // Notice how we use the current UTC time WITHOUT the utcOffset, so
                    // even if they fuck about with the PC time, the Sequence No will
                    // ALWAYS increase from the previous i.e. we are ignoring their PC settings

                    FieldInfo[] fields = typeof(SmartFinance.TransactionsCategories).GetFields(SmartParametersV2016.bindingFlags);
                    string[] ingore_fields = "RANDOMKEY1|SEQUENCE_NO|RANDOMKEY2".Split('|');

                    dynamic jsonResponse1 = JObject.Parse(financeviewmodel.obs_post_result);
                    foreach (dynamic statement in jsonResponse1)
                    {
                        string column = statement.Name;
                        switch (column)
                        {
                            case "Data":
                                if (ourviewmodel.trace)
                                {
                                    //("Data: " + statement.Value);
                                }
                                foreach (dynamic data_view in statement.Value)
                                {
                                    string data_column = data_view.Name;
                                    switch (data_column)
                                    {
                                        case "Transaction":

                                            foreach (dynamic main_account_view in data_view.Value)
                                            {
#if WINFORMS
                                                //();
#endif
                                                int randomkey1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                                                int randomkey2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);

                                                SmartFinance.TransactionsCategories transaction = new SmartFinance.TransactionsCategories();

                                                //SmartFinanceV2021.Transaction_Template(ourviewmodel,
                                                //                                       financeviewmodel,
                                                //                                       institution_code,
                                                //                                       brand_code);
                                                //
                                                //
                                                string ACCOUNT_ID = string.Empty;
                                                string TRANSACTION_ID = string.Empty;
                                                foreach (dynamic account_view in main_account_view)
                                                {
                                                    string account_column = account_view.Name;
                                                    switch (account_column)
                                                    {
                                                        case "AccountId":
                                                            account_id = account_view.Value;
                                                            // We COULD check here that the accountid matches!
                                                            // But we have to rely on the bank to ensure its data is correct
                                                            // (it saves us a check!!)
                                                            // ("Account Id: " + account_id);
                                                            ACCOUNT_ID = account_id;
                                                            break;
                                                        case "TransactionId":
                                                            transaction_id = account_view.Value;
                                                            // ("Transaction Id: " + transaction_id);
                                                            TRANSACTION_ID = transaction_id;
                                                            break;
                                                        case "CreditDebitIndicator":
                                                            creditdebit_indicator = account_view.Value;
                                                            // ("CreditDebit Indicator: " + creditdebit_indicator);
                                                            if (creditdebit_indicator == "Debit")
                                                            {
                                                                transaction.CREDITDEBIT_INDICATOR = false;
                                                            }
                                                            else
                                                            {
                                                                transaction.CREDITDEBIT_INDICATOR = true;
                                                            }
                                                            break;
                                                        case "Status":
                                                            transaction_status = account_view.Value;
                                                            // ("Status: " + transaction_status);
                                                            //transaction.TRANSACTION_STATUS = transaction_status;
                                                            break;
                                                        case "TransactionInformation":
                                                            transaction_information = account_view.Value;
                                                            // ("Transaction Information: " + transaction_information);
                                                            //transaction.TRANSACTION_INFORMATION = transaction_information;
                                                            break;
                                                        case "BookingDateTime":
                                                            financeviewmodel.booking_datetime = account_view.Value;
                                                            // You couldn't fucking make this up, could you?
                                                            // Fucking Booking Date/Time is in fucking U.S. format i.e.
                                                            // FuckingMonth/FuckingDay/FuckingYear !!!!!
                                                            if (!SmartParseV2016.Generic_Parse_Datetime_Culture_enGB(financeviewmodel.booking_datetime, financeviewmodel))
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                transaction.TRANSACTION_DATE = financeviewmodel.genericDateTime;
                                                            }
                                                            break;
                                                        case "Amount":
                                                            foreach (dynamic amount_view in account_view.Value)
                                                            {
                                                                string amount_col = amount_view.Name;
                                                                switch (amount_col)
                                                                {
                                                                    case "Amount":
                                                                        amount = amount_view.Value;
                                                                        // ("Amount: " + amount);
                                                                        amount = amount.Replace(".", string.Empty);
                                                                        transaction.AMOUNT = Convert.ToInt32(amount);
                                                                        break;
                                                                    case "Currency":
                                                                        currency = amount_view.Value;
                                                                        // ("Currency: " + currency);
                                                                        transaction.AMOUNT_CURRENCY_ORDINAL = 1;
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                        case "Balance":
                                                            foreach (dynamic balance_view in account_view.Value)
                                                            {
                                                                string balance_col = balance_view.Name;
                                                                switch (balance_col)
                                                                {
                                                                    case "Amount":
                                                                        foreach (dynamic balance_amount_view in balance_view)
                                                                        {
                                                                            foreach (dynamic balance_amount_view_view in balance_amount_view)
                                                                            {
                                                                                string balance_amount_col = balance_amount_view_view.Name;
                                                                                switch (balance_amount_col)
                                                                                {
                                                                                    case "Amount":
                                                                                        balance_amount = balance_amount_view_view.Value;
#if WINFORMS
                                                                                        //("Balance Amount: " + balance_amount);
#endif
                                                                                        balance_amount = balance_amount.Replace(".", string.Empty);
                                                                                        transaction.BALANCE_AMOUNT = Convert.ToInt32(balance_amount);
                                                                                        break;
                                                                                    case "Currency":
                                                                                        balance_currency = balance_amount_view_view.Value;
#if WINFORMS
                                                                                        //("Currency: " + balance_currency);
#endif
                                                                                        transaction.AMOUNT_CURRENCY_ORDINAL = 1;
                                                                                        break;
                                                                                    default:
                                                                                        break;
                                                                                }
                                                                            }
                                                                        }
                                                                        break;
                                                                    case "CreditDebitIndicator":
                                                                        balance_creditdebit_indicator = balance_view.Value;
#if WINFORMS
                                                                        //("Balance CreditDebit Indicator: " + balance_creditdebit_indicator);
#endif
                                                                        if (balance_creditdebit_indicator == "Debit")
                                                                        {
                                                                            transaction.BALANCE_CREDITDEBIT_INDICATOR = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            transaction.BALANCE_CREDITDEBIT_INDICATOR = true;
                                                                        }
                                                                        break;
                                                                    case "Type":
                                                                        //balance_type = Convert.ToBoolean(balance_view.Value);
#if WINFORMS
                                                                        //("Type: " + balance_type);
#endif
                                                                        //transaction.BALANCE_TYPE = balance_type;
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                        case "ProprietaryBankTransactionCode":
                                                            foreach (dynamic transaction_view in account_view.Value)
                                                            {
                                                                string transaction_col = transaction_view.Name;
                                                                switch (transaction_col)
                                                                {
                                                                    case "Code":
                                                                        transaction_code = transaction_view.Value;
#if WINFORMS
                                                                        //("Code: " + transaction_code);
#endif
                                                                        transaction.TRANSACTION_CODE = 0;
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                                //    ObservableCollection<SmartFinance.BankTransactions> transactions_found = 
                                                //        SmartSpikeFinanceV2017.Finance_Lookup_Transaction_New(//ourviewmodel,
                                                //                                                            financeviewmodel,
                                                //                                                            transaction);
                                                //    if (transactions_found.Count > 0)
                                                //    {
                                                //        if (!SmartRoutinesV2018.Compare_Transactions<SmartFinance.BankTransactions>(fields,
                                                //                                                    transactions_found[0],
                                                //                                                    transaction,
                                                //                                                    ingore_fields,
                                                //                                                    9)) // Start at SORTCODE

                                                //        {
                                                //            // One of the fields has changed
                                                //            transaction.Updated = true; // Its there and we intend to update it
                                                //                                        // Because KEY_DETAILS is a Key, we will *not* update the RandomKey
                                                //                                        // used to generate it otherwise the Keys would not be consistent!
                                                //                                        // So set this back in
                                                //            transaction.RANDOMKEY1 = transactions_found[0].RANDOMKEY1;
                                                //            // Add in updated transaction
                                                //            financeviewmodel.PLO.bank_transactionsList.Add(transaction);
                                                //        }
                                                //    }
                                                //    else
                                                //    {
                                                //        // Add in new transaction
                                                //        financeviewmodel.PLO.bank_transactionsList.Add(transaction);
                                                //    }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Links":
                                foreach (dynamic links_view in statement.Value)
                                {
                                    string links_column = links_view.Name;
                                    switch (links_column)
                                    {
                                        case "First":
                                            first = links_view.Value;
#if WINFORMS
                                            //("First: " + first);
#endif
                                            break;
                                        case "Last":
                                            last = links_view.Value;
#if WINFORMS
                                            //("Last: " + last);
#endif
                                            break;
                                        case "Self":
                                            self = links_view.Value;
#if WINFORMS
                                            //("Self: " + self);
#endif
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Meta":
                                foreach (dynamic meta_view in statement.Value)
                                {
                                    string meta_column = meta_view.Name;
                                    switch (meta_column)
                                    {
                                        case "TotalPages":
                                            totalpages = meta_view.Value;
#if WINFORMS
                                            //("TotalPages: " + totalpages);
#endif
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            //case "Risk": <= Doesn't appear to be in Accounts??
                            //    break;
                            default:
                                break;
                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            return status;
        }



        internal static async Task<bool> RBS_GET_AUTHORIZE(//MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    Uri targetUrl,
                                                    //TimeSpan timespanTimeout,
                                                    string access_token,
                                                    CancellationToken cancellation_token)
        {
            financeviewmodel.errorMessage = string.Empty;

            bool status = false;    // Assume the worst (initially)

            //            HttpClientHandler handler = new HttpClientHandler()
            //            {
            //                CookieContainer = ourviewmodel.cookies
            //                ,ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            //            };
            //            HttpClient client = new HttpClient(handler)
            //            {
            //                Timeout = timespanTimeout
            //            };

            HttpClient client = new HttpClient();// SmartBobV2017.ClientCore(timespanTimeout,
            //                               ourviewmodel.cookies);
            if (!string.IsNullOrEmpty(access_token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            }
            try
            {
                HttpResponseMessage response = await client.GetAsync(targetUrl, cancellation_token);
                if (response.IsSuccessStatusCode)
                {
                    MemoryStream dataStream = new MemoryStream();

                    await response.Content.CopyToAsync(dataStream);// Only V5 Bollocks, cancellation_token);
                    int length = dataStream.ToArray().Length;
                    if (length > 0)
                    {
                        financeviewmodel.obs_post_result = Encoding.UTF8.GetString(dataStream.ToArray(), 0, length);
                        status = true;
                    }
                    else
                    {
                        financeviewmodel.errorMessage = "Document is empty";
                    }
                }
                else
                {
                    financeviewmodel.errorMessage = response.StatusCode.ToString();
                }
            }
            catch (AggregateException exception)
            {
                financeviewmodel.errorMessage = exception.Message;
            }
            catch (TaskCanceledException exception)
            {
                financeviewmodel.errorMessage = exception.Message;
            }
            catch (HttpRequestException exception)
            {
                financeviewmodel.errorMessage = exception.Message;
            }
            return status;
        }
    }
}
