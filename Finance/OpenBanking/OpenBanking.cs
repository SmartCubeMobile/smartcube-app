using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCubeMobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SmartCubeMobileV2023
{
    internal static class OpenBanking
    {

        internal static string username = "ray_chapman48";
        internal static string password = "anni82+DANI85";

        internal static async Task GetWanking(string[] args)
        {
            CancellationToken cancel_token = new CancellationToken();

            //("Here we go again!");
            Uri obp_uri = new Uri("https://apisandbox.openbankproject.com");


            MainMeter.financeviewmodel.consumer_key = "c5k5kkjxfsgeyj3g4vc2s1bcc3bfgpcpdchoreah";
            MainMeter.financeviewmodel.consumer_secret = "1ag0tws21rdedsm4sxpx2khoaknzkkwn452paksx";

            //("Step 0");
            //("oauth_consumer_key      : " + MainPage.financeviewmodel.consumer_key);
            //("oauth_consumer_secret   : " + MainPage.financeviewmodel.consumer_secret);

            await Start(MainMeter.ourviewmodel, MainMeter.financeviewmodel, obp_uri, cancel_token);
            Console.ReadLine();
        }

        internal static async Task<bool> Start(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                Uri obp_uri,
                                                CancellationToken cancel_token)
        {
            Uri token_uri = new Uri(SmartParametersV2016.localWebsite);

            //("Step 1");
            Create_Uri(obp_uri.ToString(), "/oauth/initiate", ref token_uri, ref ourviewmodel.error_message);

            if (! //await
                  Step1(ourviewmodel, financeviewmodel, token_uri, cancel_token))
            {
                //("Failed - possibly because the oauth tokens are invalid (or you have no network! or you don't have enough msecs in the timeout)");
            }
            else
            {
                //("oauth_token             : " + financeviewmodel.oauth_token_step1);
                //("oauth_token_secret      : " + financeviewmodel.oauth_token_secret1);
                //("oauth_callback_confirmed: " + financeviewmodel.oauth_callback_confirmed);
                Create_Uri(obp_uri.ToString(), "/oauth/authorize", ref token_uri, ref ourviewmodel.error_message);

                //("Step 2");
                if (!await Step2(ourviewmodel, financeviewmodel, token_uri, cancel_token))
                {
                    //("Failed - probably because the username/password are invalid (or you have no network!)");
                }
                else
                {
                    //("oauth_token             : " + financeviewmodel.oauth_token_step2);
                    //("oauth_verifier          : " + financeviewmodel.oauth_verifier);
                    if (financeviewmodel.oauth_token_step1 != financeviewmodel.oauth_token_step2)
                    {
                        //("Failed because the oauth tokens don't match");
                    }
                    else
                    {
                        //("Step 3");
                        Create_Uri(obp_uri.ToString(), "/oauth/token", ref token_uri, ref ourviewmodel.error_message);

                        if (!await Step3(ourviewmodel, financeviewmodel, token_uri, cancel_token))
                        {
                            //("Failed - probably because the username/password are invalid (or you have no network!)");
                        }
                        else
                        {
                            //("oauth_token             : " + financeviewmodel.oauth_token_step3);
                            //("oauth_token_secret      : " + financeviewmodel.oauth_token_secret3);

                            //("Step 4");
                            Create_Uri(obp_uri.ToString(), "/obp/v1.2.1/banks/rbs/accounts/private", ref token_uri, ref ourviewmodel.error_message);

                            if (!await Step4(ourviewmodel, financeviewmodel, token_uri, cancel_token))
                            {
                                //("Failed - probably because the username/password are invalid (or you have no network!)");
                            }
                            else
                            {
                                // Step 5 post something to "Ray Chapmans" account id
                                //("oauth_token             : " + financeviewmodel.oauth_token_step3);
                                //("oauth_token_secret      : " + financeviewmodel.oauth_token_secret3);

                                //("Step 5");
                                Create_Uri(obp_uri.ToString(), "/obp/v1.2.1", ref token_uri, ref ourviewmodel.error_message);

                                if (!await Step5(ourviewmodel, financeviewmodel, token_uri, "rbs", "RaysTest", "owner"))
                                {
                                    //("Failed - probably because the username/password are invalid (or you have no network!)");
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal static 
            //async Task<bool>
            bool Step1(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            Uri token_uri,
                                            CancellationToken cancel_token)
        {
            bool status = false;

            // Use your own callback URL
            string oauth_callback = "http://localhost";
            // Use your own oauth_consumer_key
            string oauth_consumer_key = financeviewmodel.consumer_key;
            // Use your own oauth_nonce
            Guid guid = Guid.NewGuid();
            string oauth_nonce = guid.ToString();
            // Leave this blank for now
            string oauth_signature = "";
            // "HMAC-SHA1" or "HMAC-SHA256"
            string oauth_signature_method = "HMAC-SHA256";
            // Use your own oauth_timestamp
            string oauth_timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            // "1.0" or "1"
            string oauth_version = "1.0";
            // Use your own oauth_consumer_secret
            string oauth_consumer_secret = financeviewmodel.consumer_secret;

            string method = "POST";
            string uri = token_uri.ToString();

            // Create a list of OAuth parameters
            List<KeyValuePair<string, string>> oauthparameters = new List<KeyValuePair<string, string>>();
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_callback",
                    UrlEncodeCapitalized(oauth_callback)));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_consumer_key", oauth_consumer_key));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_nonce", oauth_nonce));
            oauthparameters.Add(new KeyValuePair<string, string>
                ("oauth_signature_method", oauth_signature_method));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_timestamp", oauth_timestamp));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_version", oauth_version));

            // Sort the OAuth parameters on the key
            oauthparameters.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Construct the Base String
            string basestring = method.ToUpper() + "&" + UrlEncodeCapitalized(uri) + "&";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                if (pair.Key == oauthparameters[oauthparameters.Count - 1].Key)
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value);
                }
                else
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value) + "%26";
                }
            }

            // Encrypt with either SHA1 or SHA256, creating the Signature
            var enc = Encoding.ASCII;
            if (oauth_signature_method == "HMAC-SHA1")
            {
                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(oauth_consumer_secret + "&"));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha1 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
                byte[] resultantArray = new byte[hmacsha1.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha1.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }
            else if (oauth_signature_method == "HMAC-SHA256")
            {
                HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(oauth_consumer_secret + "&"));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha256 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "")
                    .ToLower();
                byte[] resultantArray = new byte[hmacsha256.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha256.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }

            // Create the Authorization string for the WebRequest header
            string authorizationstring = "";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                authorizationstring += pair.Key;
                authorizationstring += "=";
                authorizationstring += pair.Value;
                authorizationstring += ",";
            }
            authorizationstring += "oauth_signature=" + oauth_signature;

            status = true; // await SmartBobV2017.OBS_POST(ourviewmodel,
                           //                 financeviewmodel,
                           //                cancel_token,
                           //                token_uri,
                           //                SmartParametersV2016.timespan_timeout,
                           //                string.Empty,
                           //                "OAuth " + authorizationstring);
            if (status)
            {
                if (!string.IsNullOrEmpty(financeviewmodel.obs_post_result))
                {
                    string[] tokens = financeviewmodel.obs_post_result.Split('&');
                    if (tokens.Count() == 3)
                    {
                        string[] oauth_token_array = tokens[0].Split('=');
                        if (oauth_token_array.Count() > 1)
                        {
                            financeviewmodel.oauth_token_step1 = oauth_token_array[1];
                        }
                        string[] oauth_token_secret_array = tokens[1].Split('=');
                        if (oauth_token_secret_array.Count() > 1)
                        {
                            financeviewmodel.oauth_token_secret1 = oauth_token_secret_array[1];
                        }
                        string[] oauth_callback_confirmed_array = tokens[2].Split('=');
                        if (oauth_callback_confirmed_array.Count() > 1)
                        {
                            financeviewmodel.oauth_callback_confirmed = oauth_callback_confirmed_array[1];
                        }
                    }
                    else
                    {
                        //("Step1: Not enough tokens");
                        status = false;
                    }
                }
            }
            else
            {
                //("Step1 failed");
            }

            return status;  // May be empty if there has been an error
        }
        internal static async Task<bool> Step2(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            Uri token_uri,
                                            CancellationToken cancel_token)
        {
            bool status = false;

            //string method = "GET";

            token_uri = new Uri(token_uri + "?" + "oauth_token=" + financeviewmodel.oauth_token_step1);

            //    //List<KeyValuePair<string, string>> keyvalues = new List<KeyValuePair<string, string>>();
            //    //keyvalues.Add(new KeyValuePair<string, string>("oauth_token",financeviewmodel.oauth_token_step1));

            HtmlAgilityPack.HtmlDocument document = await SmartBobV2017.OBS_GETHTML(ourviewmodel,
                                financeviewmodel,
                               cancel_token,
                               token_uri,
                               SmartParametersV2016.timespanTimeout,
                               //keyvalues,
                               string.Empty,
                               string.Empty); // Authorization string
            if (string.IsNullOrEmpty(financeviewmodel.error_message))
            {
                IList<HtmlAgilityPack.HtmlNode>
                            HtmlCol1;

                bool username_found = false,
                        password_found = false,
                        login_found = false;
                string login_code = string.Empty;

                HtmlCol1 = SelectNodesAsList(document.DocumentNode, ".//input");
                foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1)
                {
                    string name = element1.GetAttributeValue("name", string.Empty);
                    if (!string.IsNullOrEmpty(name))
                    {
                        switch (name)
                        {
                            case "username":
                                username_found = true;
                                break;
                            case "password":
                                password_found = true;
                                break;
                            default:
                                string value = element1.GetAttributeValue("value", string.Empty);
                                if (value == "Log In")
                                {
                                    login_found = true;
                                    login_code = name;
                                }
                                break;
                        }
                    }
                }
                string login_token = string.Empty;
                if (username_found &&
                    password_found &&
                    login_found &&
                    !string.IsNullOrEmpty(login_code))
                {
                    //("all three found");
                    login_token = "username=" + Uri.EscapeDataString(username) + "&" +
                                                "password=" + Uri.EscapeDataString(password) + "&" +
                                                "brand=" + "&" +
                                                login_code + "=Login";
                }
                if (!string.IsNullOrEmpty(login_token))
                {
                    //string method = "POST";
                    token_uri = new Uri("https://apisandbox.openbankproject.com/user_mgt/login");

                    token_uri = new Uri(token_uri + "?" + login_token);

                    status = true; // await SmartBobV2017.OBS_POST_RESPONSE(ourviewmodel,
                                   //         financeviewmodel,
                                   //        cancel_token,
                                   //        token_uri,
                                   //        SmartParametersV2016.timespan_timeout,
                                   //        string.Empty,
                                   //        string.Empty);
                    if (status)
                    {
                        if (!string.IsNullOrEmpty(financeviewmodel.obs_post_result))
                        {
                            // Look for (new) oauth_token and oauth_verifier
                            string[] temps = Uri.UnescapeDataString(financeviewmodel.obs_post_result).Split('?');
                            if (temps.Count() > 1)
                            {
                                // I have NO IDEA why I have to Unescape it TWICE ... but I do
                                string[] question_mark = Uri.UnescapeDataString(temps[1]).Split('?');
                                if (question_mark.Count() > 1)
                                {
                                    string[] equals = question_mark[1].Split('&');
                                    if (equals.Count() > 1)
                                    {
                                        string[] oauth_token_step2 = equals[0].Split('=');
                                        string[] oauth_verifier_step2 = equals[1].Split('=');
                                        if (oauth_token_step2.Count() > 1 &&
                                            (oauth_token_step2[0] == "oauth_token"))
                                        {
                                            financeviewmodel.oauth_token_step2 = oauth_token_step2[1];
                                            if (oauth_verifier_step2.Count() > 1 &&
                                                (oauth_verifier_step2[0] == "oauth_verifier"))
                                            {
                                                financeviewmodel.oauth_verifier = oauth_verifier_step2[1];
                                                status = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        //("Bad");
                    }


                }
            }
            return status;
        }

        internal static IList<HtmlAgilityPack.HtmlNode> SelectNodesAsList(this HtmlAgilityPack.HtmlNode node, string extendedPath)
        {
            // Need to test this breaking change!!!!
            if (node != null)
            {
                HtmlAgilityPack.HtmlNodeCollection list = node.SelectNodes(extendedPath);
                if (list != null)
                {
                    IList<HtmlAgilityPack.HtmlNode> node_list = new List<HtmlAgilityPack.HtmlNode>();
                    foreach (HtmlAgilityPack.HtmlNode element in list)
                    {
                        node_list.Add(element);
                    }
                    return node_list;
                }
            }
            return new List<HtmlAgilityPack.HtmlNode>();
        }


        internal static async Task<bool> Step3(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            Uri token_uri,
                                            CancellationToken cancel_token)
        {
            bool status = false;

            // Use your previous oauth_verifier
            string oauth_verifier = financeviewmodel.oauth_verifier;
            // Use your previous oauth_token
            string oauth_token = financeviewmodel.oauth_token_step2;
            // Use your own oauth_consumer_key
            string oauth_consumer_key = financeviewmodel.consumer_key;
            // Use your own oauth_nonce
            Guid guid = Guid.NewGuid();
            string oauth_nonce = guid.ToString();
            // Leave this blank for now
            string oauth_signature = "";
            // "HMAC-SHA1" or "HMAC-SHA256"
            string oauth_signature_method = "HMAC-SHA256";
            // Use your own oauth_timestamp
            string oauth_timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            // "1.0" or "1"
            string oauth_version = "1.0";

            // Use your own oauth_consumer_secret
            string oauth_consumer_secret = financeviewmodel.consumer_secret;

            // Use your previous oauth_token-secret
            string oauth_token_secret = financeviewmodel.oauth_token_secret1;

            string method = "POST";
            string uri = token_uri.ToString();

            // Create a list of OAuth parameters
            List<KeyValuePair<string, string>> oauthparameters = new List<KeyValuePair<string, string>>();
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_verifier", oauth_verifier));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_token", oauth_token));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_consumer_key", oauth_consumer_key));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_nonce", oauth_nonce));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_signature_method", oauth_signature_method));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_timestamp", oauth_timestamp));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_version", oauth_version));

            // Sort the OAuth parameters on the key
            oauthparameters.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Construct the Base String
            string basestring = method.ToUpper() + "&" + UrlEncodeCapitalized(uri) + "&";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                if (pair.Key == oauthparameters[oauthparameters.Count - 1].Key)
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value);
                }
                else
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value) + "%26";
                }
            }

            // Encrypt with either SHA1 or SHA256, creating the Signature
            var enc = Encoding.ASCII;
            if (oauth_signature_method == "HMAC-SHA1")
            {
                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha1 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
                byte[] resultantArray = new byte[hmacsha1.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha1.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }
            else if (oauth_signature_method == "HMAC-SHA256")
            {
                HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha256 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "")
                    .ToLower();
                byte[] resultantArray = new byte[hmacsha256.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha256.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }

            // Create the Authorization string for the WebRequest header
            string authorizationstring = "";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                authorizationstring += pair.Key;
                authorizationstring += "=";
                authorizationstring += pair.Value;
                authorizationstring += ",";
            }
            authorizationstring += "oauth_signature=" + oauth_signature;

            status = true; // await SmartBobV2017.OBS_POST(ourviewmodel,
                           //                 financeviewmodel,
                           //                cancel_token,
                           //                token_uri,
                           //                SmartParametersV2016.timespan_timeout,
                           //                string.Empty,
                           //                "OAuth " + authorizationstring);
            if (status)
            {
                if (!string.IsNullOrEmpty(financeviewmodel.obs_post_result))
                {
                    string[] tokens = financeviewmodel.obs_post_result.Split('&');
                    if (tokens.Count() == 2)
                    {
                        string[] oauth_token_array = tokens[0].Split('=');
                        if (oauth_token_array.Count() > 1)
                        {
                            financeviewmodel.oauth_token_step3 = oauth_token_array[1];
                        }
                        string[] oauth_token_secret_array = tokens[1].Split('=');
                        if (oauth_token_secret_array.Count() > 1)
                        {
                            financeviewmodel.oauth_token_secret3 = oauth_token_secret_array[1];
                        }
                    }
                    else
                    {
                        //("Step3: Not enough tokens");
                        status = false;
                    }
                }
            }
            return status;  // May be empty if there has been an error
        }

        internal static async Task<bool> Step4(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            Uri token_uri,
                                            CancellationToken cancel_token)
        {
            bool status = false;

            // Use your previous oauth_token
            string oauth_token = financeviewmodel.oauth_token_step3;
            // Use your own oauth_consumer_key
            string oauth_consumer_key = financeviewmodel.consumer_key;
            // Use your own oauth_nonce
            Guid guid = Guid.NewGuid();
            string oauth_nonce = guid.ToString();
            // Leave this blank for now
            string oauth_signature = "";
            // "HMAC-SHA1" or "HMAC-SHA256"
            string oauth_signature_method = "HMAC-SHA256";
            // Use your own oauth_timestamp
            string oauth_timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            // "1.0" or "1"
            string oauth_version = "1.0";

            // Use your own oauth_consumer_secret
            string oauth_consumer_secret = financeviewmodel.consumer_secret;

            // Use your previous oauth_token-secret
            string oauth_token_secret = financeviewmodel.oauth_token_secret3;

            string method = "GET";
            string uri = token_uri.ToString();

            // Create a list of OAuth parameters
            List<KeyValuePair<string, string>> oauthparameters = new List<KeyValuePair<string, string>>();
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_token", oauth_token));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_consumer_key", oauth_consumer_key));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_nonce", oauth_nonce));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_signature_method", oauth_signature_method));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_timestamp", oauth_timestamp));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_version", oauth_version));

            // Sort the OAuth parameters on the key
            oauthparameters.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Construct the Base String
            string basestring = method.ToUpper() + "&" + UrlEncodeCapitalized(uri) + "&";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                if (pair.Key == oauthparameters[oauthparameters.Count - 1].Key)
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value);
                }
                else
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value) + "%26";
                }
            }

            // Encrypt with either SHA1 or SHA256, creating the Signature
            var enc = Encoding.ASCII;
            if (oauth_signature_method == "HMAC-SHA1")
            {
                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha1 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
                byte[] resultantArray = new byte[hmacsha1.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha1.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }
            else if (oauth_signature_method == "HMAC-SHA256")
            {
                HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha256 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "")
                    .ToLower();
                byte[] resultantArray = new byte[hmacsha256.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha256.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }

            // Create the Authorization string for the WebRequest header
            string authorizationstring = "";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                authorizationstring += pair.Key;
                authorizationstring += "=";
                authorizationstring += pair.Value;
                authorizationstring += ",";
            }
            authorizationstring += "oauth_signature=" + oauth_signature;

            status = true;  // await SmartBobV2017.OBS_GET(ourviewmodel,
                            //                financeviewmodel,
                            //               cancel_token,
                            //               token_uri,
                            //               SmartParametersV2016.timespan_timeout,
                            //               string.Empty,
                            //               "OAuth " + authorizationstring);
            if (status)
            {
                //("here");
                try
                {
                    dynamic jsonResponse = JObject.Parse(financeviewmodel.obs_post_result);

                    string bank;
                    //bool bank_account_balance,
                    //     bank_account_bank_name,
                    //     bank_account_currency,
                    //     bank_account_label,
                    //     bank_account_number,
                    //     bank_account_owners,
                    //     bank_account_type;

                    foreach (dynamic statement in jsonResponse)
                    {
                        string statement_name = statement.Name;
                        if (statement_name == "accounts")
                        {
                            foreach (dynamic transactions in statement)
                            {
                                foreach (dynamic transaction in transactions)
                                {
                                    foreach (dynamic individual in transaction)
                                    {
                                        string column = individual.Name;
                                        switch (column)
                                        {
                                            case "bank_id":
                                                bank = individual.Value;
                                                //("Bank Id: " + bank);
                                                break;
                                            case "id":
                                                bank = individual.Value;
                                                //("Bank Name: " + bank);
                                                break;
                                            case "views_available":
                                                foreach (dynamic view in individual)
                                                {
                                                    foreach (dynamic sub_view in view)
                                                    {
                                                        foreach (dynamic sub_sub_view in sub_view)
                                                        {
                                                            string sub_sub_view_column = sub_sub_view.Name;
                                                            switch (sub_sub_view_column)
                                                            {
                                                                case "can_see_bank_account_balance":
                                                                    //bank_account_balance = true;
                                                                    //("Bank Account balance: " + bank_account_balance);
                                                                    break;
                                                                case "can_see_bank_account_bank_name":
                                                                    //bank_account_bank_name = true;
                                                                    //("Bank Account bank name: " + bank_account_bank_name);
                                                                    break;
                                                                case "can_see_bank_account_currency":
                                                                    //bank_account_currency = true;
                                                                    //("Bank Account currency: " + bank_account_currency);
                                                                    break;
                                                                case "can_see_bank_account_label":
                                                                    //bank_account_label = true;
                                                                    //("Bank Account label: " + bank_account_label);
                                                                    break;
                                                                case "can_see_bank_account_number":
                                                                    //bank_account_number = true;
                                                                    //("Bank Account number: " + bank_account_number);
                                                                    break;
                                                                case "can_see_bank_account_owners":
                                                                    //bank_account_owners = true;
                                                                    //("Bank Account owners: " + bank_account_owners);
                                                                    break;
                                                                case "can_see_bank_account_type":
                                                                    //bank_account_type = true;
                                                                    //("Bank Account type: " + bank_account_type);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    status = true;
                }
                catch (Exception ex)
                {
                    ourviewmodel.error_message = ex.Message;
                }
            }


            return status;  // May be empty if there has been an error
        }

        internal static async Task<bool> Step5(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            Uri token_uri,
                                            string Bank_Id,
                                            string Account_Id,
                                            string View_Id)
        {
            bool status = false;

            string BANK_ID = Bank_Id;       // Use the BANK_ID of your paying account
            string ACCOUNT_ID = Account_Id; // Use the ACCOUNT_ID of your paying account
            string VIEW_ID = View_Id;       // Use "VIEW_ID = owner".  Read documentations about Views
            // Use your previous oauth_token
            string oauth_token = financeviewmodel.oauth_token_step3;
            //("Step5: oauth_token: " + financeviewmodel.oauth_token_step3);
            // Use your own oauth_consumer_key
            string oauth_consumer_key = financeviewmodel.consumer_key;
            //("Step5: oauth_consumer_key: " + financeviewmodel.consumer_key);
            // Use your own oauth_nonce
            Guid guid = Guid.NewGuid();
            string oauth_nonce = guid.ToString();
            // Leave this blank for now
            string oauth_signature = "";
            // "HMAC-SHA1" or "HMAC-SHA256"
            string oauth_signature_method = "HMAC-SHA256";
            // Use your own oauth_timestamp
            string oauth_timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            // "1.0" or "1"
            string oauth_version = "1.0";

            // Use your own oauth_consumer_secret
            string oauth_consumer_secret = financeviewmodel.consumer_secret;
            //("Step5: oauth_consumer_secret: " + oauth_consumer_secret);

            // Use your previous oauth_token-secret
            string oauth_token_secret = financeviewmodel.oauth_token_secret3;
            //("Step5: oauth_token_secret: " + oauth_token_secret);

            string method = "POST";
            string uri = token_uri.ToString() +
                                    "/banks/" +
                                    BANK_ID +
                                    "/accounts/" +
                                    ACCOUNT_ID +
                                    "/" +
                                    VIEW_ID +
                                    "/" +
                                    "transactions";

            // Create a list of OAuth parameters
            List<KeyValuePair<string, string>> oauthparameters = new List<KeyValuePair<string, string>>();
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_token", oauth_token));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_consumer_key", oauth_consumer_key));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_nonce", oauth_nonce));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_signature_method", oauth_signature_method));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_timestamp", oauth_timestamp));
            oauthparameters.Add(new KeyValuePair<string, string>("oauth_version", oauth_version));

            // Sort the OAuth parameters on the key
            oauthparameters.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Construct the Base String
            string basestring = method.ToUpper() + "&" + UrlEncodeCapitalized(uri) + "&";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                if (pair.Key == oauthparameters[oauthparameters.Count - 1].Key)
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value);
                }
                else
                {
                    basestring += pair.Key + "%3D" + UrlEncodeCapitalized(pair.Value) + "%26";
                }
            }

            // Encrypt with either SHA1 or SHA256, creating the Signature
            var enc = Encoding.ASCII;
            if (oauth_signature_method == "HMAC-SHA1")
            {
                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha1 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
                byte[] resultantArray = new byte[hmacsha1.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha1.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }
            else if (oauth_signature_method == "HMAC-SHA256")
            {
                HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(oauth_consumer_secret + "&" + oauth_token_secret));
                hmac.Initialize();
                byte[] buffer = enc.GetBytes(basestring);
                string hmacsha256 = BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "")
                    .ToLower();
                byte[] resultantArray = new byte[hmacsha256.Length / 2];
                for (int i = 0; i < resultantArray.Length; i++)
                {
                    resultantArray[i] = Convert.ToByte(hmacsha256.Substring(i * 2, 2), 16);
                }
                string base64 = Convert.ToBase64String(resultantArray);
                oauth_signature = UrlEncodeCapitalized(base64);
            }

            // Create the Authorization string for the WebRequest header
            string authorizationstring = "";
            foreach (KeyValuePair<string, string> pair in oauthparameters)
            {
                authorizationstring += pair.Key;
                authorizationstring += "=";
                authorizationstring += pair.Value;
                authorizationstring += ",";
            }
            authorizationstring += "oauth_signature=" + oauth_signature;

            MemoryStream dataStream = new MemoryStream();

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.ContentType = "application/json";
            webReq.Method = method;
            webReq.CookieContainer = ourviewmodel.cookies;
            webReq.Timeout = SmartParametersV2016.serverTimeoutMsecs;
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            webReq.Headers.Add("Authorization", "OAuth " + authorizationstring);
            try
            {
                using (var streamWriter = new StreamWriter(webReq.GetRequestStream()))
                {
                    string json = "{\"bank_id\":\"rbs\",\"account_id\":\"RaysTest\",\"amount\":\"1.00\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                WebResponse response = await webReq.GetResponseAsync();
                await response.GetResponseStream().CopyToAsync(dataStream);
                int length = dataStream.ToArray().Length;
                if (length > 0)
                {
                    string result_step3 = Encoding.UTF8.GetString(dataStream.ToArray(), 0, length);
                    // Well ... a result came back. Is it a JSON array and can I convert it into
                    // something useful?
                    try
                    {
                        dynamic jsonResponse = JObject.Parse(result_step3);

                        foreach (dynamic statement in jsonResponse)
                        {
                            string statement_name = statement.Name;
                            if (statement_name == "accounts")
                            {

                                //("Here");
                            }
                        }
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        ourviewmodel.error_message = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                ourviewmodel.error_message = ex.Message;
            }
            return status;  // May be empty if there has been an error
        }

        internal static string UrlEncodeCapitalized(string input)
        {
            char[] outputchar = HttpUtility.UrlEncode(input).ToCharArray();
            for (int i = 0; i < outputchar.Length - 2; i++)
            {
                if (outputchar[i] == '%')
                {
                    outputchar[i + 1] = char.ToUpper(outputchar[i + 1]);
                    outputchar[i + 2] = char.ToUpper(outputchar[i + 2]);
                }
            }
            return new string(outputchar);
        }

        internal static bool Create_Uri(string base_string, string relative_string, ref Uri base_uri, ref string error_message)
        {
            // DON'T FUCK WITH THIS ROUTINE <=== THIS MEANS ***YOU***
            bool status = true;
            error_message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(base_string))
                {
                    base_uri = new Uri(base_string);

                    if (!string.IsNullOrEmpty(relative_string))
                    {
                        if (relative_string.IndexOf(base_string) == -1)
                        {
                            Uri relative_uri = new Uri(relative_string, UriKind.Relative);
                            base_uri = new Uri(base_uri, relative_uri);
                        }
                        else
                        {
                            base_uri = new Uri(relative_string);
                        }
                    }
                }
                else
                {
                    error_message = "Uri base string is empty";
                    status = false;
                }
            }
            catch (ArgumentNullException exception)
            {
                error_message = exception.Message;
                status = false;
            }
            catch (UriFormatException exception)
            {
                error_message = exception.Message;
                status = false;
            }
            if (!status)
            {
                error_message = "Cannot create Uri from: " + base_string + relative_string + " " + error_message;

            }
            return status;
        }

        internal static T JsonDeserialize<T>(string jsonString)
        {
            // This could break!!! Needs testing!!!
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            var obj = JsonConvert.DeserializeObject<T>(jsonString);

            //MemoryStream ms = nll;
            //T obj = default(T);
            //try
            //{
            //    ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            //    obj = (T)ser.ReadObject(ms);
            //    //return obj;
            //}
            //finally
            //{
            //    if (ms != null)
            //    {
            //        ms.Dispose();
            //    }
            //}
            return obj;
        }
    }
}
