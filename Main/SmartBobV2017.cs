//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Net.Http;        // No more of this absolute bollocks shit .. We are back using this bolocks SHIT!!!

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using iText.Kernel.Pdf;

namespace SmartCubeMobile
{
    public static class YetAnotherFuckingHoop
    {
        internal static IList<HtmlAgilityPack.HtmlNode> SelectNodesAsList(this HtmlAgilityPack.HtmlNode node, string extendedPath)
        {
            // Need to test this breaking change!!!!
            if (node != null)
            {
                HtmlAgilityPack.HtmlNodeCollection list = node.SelectNodes(extendedPath);
                if (list != null)
                {
                    IList<HtmlAgilityPack.HtmlNode> nodeList = new List<HtmlAgilityPack.HtmlNode>();
                    foreach (HtmlAgilityPack.HtmlNode element in list) // Checked
                    {
                        nodeList.Add(element);
                    }
                    return nodeList;
                }
            }
            return new List<HtmlAgilityPack.HtmlNode>();
        }
    }

    public class SmartBobV2017
    {
        // Taken these fuckers out because subsequent calls to PostAsync caused
        // the entire bollocks to fall over
        //internal static HttpClient client;
        //internal static StringContent stringContent;
        //internal static ByteArrayContent byteContent;
        //#if WPF
        //       internal static PdfStamper stamper;
        //#endif
        //        internal static MemoryStream memoryStream;


        /// <summary>
        /// JSON Deserialization
        /// </summary>
        internal static T JsonDeserialize<T>(string jsonString)
        {
            // This could break!!! Needs testing!!!
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            T obj = JsonSerializer.Deserialize<T>(jsonString);
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

        //internal static bool Ping(string url)
        //{
        //    // https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?redirectedfrom=MSDN&view=net-5.0
        //    // Applications use the Ping class to detect whether a remote computer is reachable.
        //    // Network topology can determine whether Ping can successfully contact a remote host.
        //    // The presence and configuration of proxies, network address translation (NAT) equipment,
        //    // or firewalls can prevent Ping from succeeding.
        //    // A successful Ping indicates only that the remote host can be reached on the network;
        //    // the presence of higher level services(such as a Web server) on the remote host is
        //    // not guaranteed.

        //    //    Ping pingSender = new Ping();
        //    //    PingOptions options = new PingOptions();

        //    //    // Use the default Ttl value which is 128,
        //    //    // but change the fragmentation behavior.
        //    //    options.DontFragment = true;

        //    //    // Create a buffer of 32 bytes of data to be transmitted.
        //    //    string data = "Hello sailor!";
        //    //    byte[] buffer = Encoding.ASCII.GetBytes(data);
        //    //    int timeout = 120;
        //    //    PingReply reply = pingSender.Send(url, timeout, buffer, options);
        //    //    if (reply.Status == IPStatus.Success)
        //    //    {
        //    //        ("Address: {0}", reply.Address.ToString());
        //    //        ("RoundTrip time: {0}", reply.RoundtripTime);
        //    //        ("Time to live: {0}", reply.Options.Ttl);
        //    //        ("Don't fragment: {0}", reply.Options.DontFragment);
        //    //        ("Buffer size: {0}", reply.Buffer.Length);
        //    //        return true;
        //    //    }
        //    //    else
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    //}
        //    //url = url.Replace("https", "http");
        //    try
        //    {
        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Timeout = 10000;
        //        request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
        //        request.Method = WebRequestMethods.Http.Connect;// "HEAD";

        //        using (var response = request.GetResponse())
        //        {
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        
        
        // This is going to be painful 1 ... try to make Generic
        // Http shit so that Kotlin bollocks is reduced
        internal static HttpClient BuildClient(CookieContainer cookies,
                                                TimeSpan timeout)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = cookies ?? new CookieContainer(),
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new HttpClient(handler)
            {
                Timeout = timeout
            };
            return client;
        }

        internal static HttpClient AddClientHeader(HttpClient client,
                                                    string header,
                                                    string value)
        {
            client.DefaultRequestHeaders.Add(header, value);
            return client;
        }

        internal static async Task<string> CallApiGET(string url,
                                                    CookieContainer cookies,
                                                    TimeSpan timeout,
                                                    CancellationToken token,
                                                    Action<string> seterror,
                                                    string bearerToken = "",
                                                    string cbversion = "")
        {
            try
            {
                HttpClient client = BuildClient(cookies, timeout);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                if (bearerToken != "")
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                    if (cbversion != "")
                    {
                        request.Headers.Add("CB-VERSION", cbversion);
                    }
                }
                HttpResponseMessage response = await client.SendAsync(request, token);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (response != null)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return "";
        }
        internal static async Task<string> GetClientResponseString(HttpClient client,
                                                            Uri requestUrl,
                                                            CancellationToken token,
                                                            Action<string> seterror)
        {
            string response = "";
            try
            {
                //requestUrl = new Uri(requestUrl.ToString().Replace("/Identity", "/C:/Identity"));
                HttpResponseMessage httpresponse = await client.GetAsync(requestUrl, token);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    response = await httpresponse.Content.ReadAsStringAsync();
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
               seterror(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }
            
            }
            return response;
        }

        internal static async Task<Stream> GetClientResponseStream(HttpClient client,
                                                            Uri requestUrl,
                                                            CancellationToken token,
                                                            Action<string> seterror)
        {
            Stream response = null;
            try
            {
                HttpResponseMessage httpresponse = await client.GetAsync(requestUrl, token);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    response = await httpresponse.Content.ReadAsStreamAsync();
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }
        internal static async Task<string> GetClientResponseMessage(HttpClient client,
                                                            Uri requestUrl,
                                                            CancellationToken token,
                                                            string authorizationToken,
                                                            Action<string> seterror)
        {
            string response = "";
            try
            {
                HttpResponseMessage httpresponse = await client.GetAsync(requestUrl, token);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    bool localJson = Is_jsonReturned(httpresponse);
                    if (localJson)
                    {
                        response = await httpresponse.Content.ReadAsStringAsync();
                        if (authorizationToken == "")
                        {
                            if (Get_Json_String(response, seterror))
                            {
                                return response;
                            }
                        }
                        else
                        {
                            return response;
                        }
                    }
                    else
                    {
                        // Get the document as a string
                        response = await httpresponse.Content.ReadAsStringAsync();
                    }
                    // Bestoke!! <= Bespoke
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }

        [RequiresUnreferencedCode("Calls SmartCubeMobile.SmartBobV2017.JsonDeserialize<T>(String)")]
        internal static async Task<string> PostClientLogin(HttpClient client,
                                                        Uri requestUrl,
                                                        HttpContent multipartFormDataContent,
                                                        CancellationToken cancellationToken,
                                                        Action<string> seterror,
                                                        Action<bool> setjson,
                                                        Action<Potential> setloggedin)
        {
            string response = "";
            try
            {
                HttpResponseMessage httpresponse = await client.PostAsync(requestUrl, multipartFormDataContent, cancellationToken);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    bool localJson = Is_jsonReturned(httpresponse);
                    setjson(localJson);
                    // Releases the resources of the response.
                    response = await httpresponse.Content.ReadAsStringAsync();
                    if (localJson)
                    {
                        try
                        {
                            setloggedin(JsonDeserialize<Potential>(response));
                        }
                        catch (Exception ex)
                        {
                            seterror(ex.Message);
                            return response;
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }

        internal static async Task<string> PostClientJson(HttpClient client,
                                                        Uri requestUrl,
                                                        HttpContent multipartFormDataContent,
                                                        CancellationToken cancellationToken,
                                                        Action<string> seterror)
        {
            string response = "";
            try
            {
                HttpResponseMessage httpresponse = await client.PostAsync(requestUrl, multipartFormDataContent, cancellationToken);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)   // This will do for the time being
                {
                    bool local_json = Is_jsonReturned(httpresponse);
                    if (local_json)
                    {
                        response = await httpresponse.Content.ReadAsStringAsync();
                        if (Get_Json_String(response, seterror))
                        {
                            return response;
                        }
                    }
                    else
                    {
                        // Get the document as a string
                        response = await httpresponse.Content.ReadAsStringAsync();
                    }
                    // Bestoke!! <= Bespoke
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning a 'list' and then
                // checking in the calling routine to see if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }
        internal static async Task<string> PostClientString(HttpClient client,
                                                        Uri requestUrl,
                                                        HttpContent multipartFormDataContent,
                                                        CancellationToken cancellationToken,
                                                        Action<string> seterror)
        {
            string response = "";
            try
            {
                HttpResponseMessage httpresponse = await client.PostAsync(requestUrl, multipartFormDataContent, cancellationToken);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    // Releases the resources of the response.
                    response = await httpresponse.Content.ReadAsStringAsync();
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes we want to Quit button or by a Timeout
                Check_InnerException(exception, seterror);
                // Churn, churn, churn, churn, churn endless, continuous running commentary ...
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ... by returning an 'list' and then
                // checking in the calling routine if it is empty
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }

        internal static async Task<Stream> PostClientStream(HttpClient client,
                                                        Uri requestUrl,
                                                        HttpContent multipartFormDataContent,
                                                        CancellationToken cancellationToken,
                                                        Action<string> seterror)
        {
            Stream response = null;
            try
            {
                HttpResponseMessage httpresponse = await client.PostAsync(requestUrl, multipartFormDataContent, cancellationToken);
                // Ensure the response is successful
                // response.EnsureSuccessStatusCode(); One day, maybe ...
                if (httpresponse.IsSuccessStatusCode)
                {
                    // Releases the resources of the response.
                    response = await httpresponse.Content.ReadAsStreamAsync();
                }
            }
            catch (TaskCanceledException exception)
            {
                // Cancelled by us pressing the Yes button or by a Timeout
                Check_InnerException(exception, seterror);
            }
            catch (Exception ex)
            {
                seterror(ex.Message);
            }
            return response;
        }

        // This is going to be painful 2 .... She NEVER shuts up ...

        // Used in SmartLogin so its a  Phase 1
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_GET_SYNC_CHALLENGE(
                                                                        SignInViewModel signinviewmodel,
                                                                        CancellationToken cancellationToken,
                                                                        Uri TargetUrl,
                                                                        string guidKey)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            HttpClient client = BuildClient(signinviewmodel.cookies, signinviewmodel.timespanTimeout);

            if (!string.IsNullOrEmpty(guidKey))
            {
                client = AddClientHeader(client, "Referer", guidKey);
            }
            client = AddClientHeader(client, "Accept", "text/html,application/xhtml+xml,application/xml,*/*; q=0.9");
            string response = await GetClientResponseString(client, TargetUrl, cancellationToken, err => signinviewmodel.errorMessage = err);
            if (signinviewmodel.errorMessage == "" &&
                response != "")
            {
                document.LoadHtml(response);
            }
            return document;
        }

#if WINFORMS
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_GET_SYNC_SPECIAL(MainViewModel ourviewmodel,
                                                                                CancellationToken cancellationToken,
                                                                                Uri url)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            string response = await GetClientResponseString(client, url, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                document.LoadHtml(response);
            }
            return document;
        }
#endif
        internal static bool CheckCancellation(SignInViewModel signinviewmodel,
                                               CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                signinviewmodel.errorMessage = "Cancellation token None";
                return false;
            }
            return true;
        }

        // Used in SmartLogin and British Gas so its a Phase 1 AND Phase 2
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_GET_ASYNC(MainViewModel ourviewmodel,
                                                                                    Uri TargetUrl,
                                                                                    CancellationToken cancellationToken,
                                                                                    List<string> headers,
                                                                                    string authorizationToken = "",
                                                                                    Guid guidKey = new Guid())
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            // Most important - check that the timeout isn't zero or some fantastic figure
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
            if (headers.Count > 0)
            {
                foreach (string header in headers) // Checked
                {
                    string[] comp = header.Split(SmartParametersV2016.unitSeparator);
                    if (comp.Length > 1)
                    {
                        client = AddClientHeader(client, comp[0], comp[1]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                client = AddClientHeader(client, "Authorization", authorizationToken);
            }
            string response = await GetClientResponseString(client, TargetUrl, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                document.LoadHtml(response);
            }
            return document;
        }

        internal static async Task<string> HTTPCLIENT_GET_ASYNC_JSONX(MainViewModel ourviewmodel,
                                                                    Uri TargetUrl,
                                                                    CancellationToken cancellationToken,
                                                                    string authorizationToken = "",
                                                                    Guid guidKey = new Guid())
        {
            string response = "";
            // Most important - check that the timeout isn't zero or some fantastic figure
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            if (!string.IsNullOrEmpty(authorizationToken))
            {
                client = AddClientHeader(client, "Authorization", authorizationToken);
            }
            response = await GetClientResponseMessage(client, TargetUrl, cancellationToken, authorizationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                    response != "")
            {
                return response;
            }
            return "";
        }

#if WINFORMS
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_POST_SYNC_SPECIAL(MainViewModel ourviewmodel,
                                                                                CancellationToken cancellationToken,
                                                                                List<KeyValuePair<string, string>> keyValues,                                                                                Uri url,
                                                                                string guidKey)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
            
            if (!string.IsNullOrEmpty(guidKey))
            {
                client = AddClientHeader(client, "Referer", guidKey);
            }
            client = AddClientHeader(client, "Accept", "text/xml, application/xhtml+xml, application/json, text/javascript, */*; q=0.01");
                
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues)
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            // by calling .Result you are performing a synchronous call
            string response = await PostClientJson(client, url, multipartFormDataContent, cancellationToken, err=>ourviewmodel.errorMessage=err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                document.LoadHtml(response);
            }
            // Its ONLY FUCKING WORKED!!!   Time for a cup of tea methinks!!!
            return document;
        }
#endif
        // Used in SmartLogin so its a Phase 1
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_POST_ASYNC_LOGIN(SignInViewModel signinviewmodel,
                                                                                        CancellationToken cancellationToken,
                                                                                        List<KeyValuePair<string, string>> keyValues, //System.Net.Http.FormUrlEncodedContent content,
                                                                                        Uri url,
                                                                                        string guidKey,
                                                                                        Action<Potential> setloggedin)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            HttpClient client = BuildClient(signinviewmodel.cookies, signinviewmodel.timespanTimeout);
            // Well ... according to chatGPT (which is often wrong)
            // this is the ONLY way to communicate 'Accept' headers
            // with Android. Actually, I don't believe a word
            // the fucker says, but I'm going to go with it just for
            // the moment ...
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            if (!string.IsNullOrEmpty(guidKey))
            {
                string referer = $"http://{guidKey}/";
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
            }
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            string response = await PostClientLogin(client,
                                                    url,
                                                    multipartFormDataContent,
                                                    cancellationToken,
                                                    err => signinviewmodel.errorMessage = err,
                                                    jsonreturned => signinviewmodel.jsonReturned = jsonreturned,
                                                    loggedin => signinviewmodel.loggedInUser = loggedin);
            if (signinviewmodel.errorMessage == "" &&
                (response != ""))
            {
                // Not so sure we ever get anything even remotely resembling
                // an HtmlDocument here ...its really just the login parameters?
                document.LoadHtml(response);
            }
            // Its ONLY FUCKING WORKED!!!   Time for a cup of tea methinks!!!
            return document;
        }

        // Used in SmartLogin and Scrapers so its a Phase 1 and Phase 2
        internal static async Task<HtmlAgilityPack.HtmlDocument> HTTPCLIENT_POST_ASYNC(MainViewModel ourviewmodel,
                                                    Uri TargetUrl,
                                                    CancellationToken cancellationToken,
                                                    List<KeyValuePair<string, string>> keyValues, //System.Net.Http.FormUrlEncodedContent content,
                                                    List<string> headers,
                                                    Guid guidKey = new Guid())

        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            if (headers.Count > 0)
            {
                foreach (string header in headers) // Checked
                {
                    string[] comp = header.Split(SmartParametersV2016.unitSeparator);
                    if (comp.Length > 1)
                    {
                        client = AddClientHeader(client, comp[0], comp[1]);
                    }
                }
            }
            else
            {
                client = AddClientHeader(client, "Accept", "application/json, text/javascript, text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            }

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            string response = await PostClientString(client, TargetUrl, multipartFormDataContent, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                document.LoadHtml(response);
            }
            // Its ONLY FUCKING WORKED!!!   Time for a cup of tea methinks!!!
            return document;
        }

        internal static async Task<string> HTTPCLIENT_POST_ASYNC_JSON(MainViewModel ourviewmodel,
                                                    Uri TargetUrl,
                                                    CancellationToken cancellationToken,
                                                    string jsonContent,
                                                    string postData,
                                                    string authorizationToken = "",
                                                    Guid guidKey = new Guid())

        {
            string response = "";
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            if (!string.IsNullOrEmpty(postData))
            {
                // We are expecting an Html document of some sort to be returned
                client = AddClientHeader(client, "Accept", "text/html, */*; q=0.01");
                // This is the __RequestVerificationToken string and its value
                string[] parts = postData.Split(SmartParametersV2016.spacechar);
                client = AddClientHeader(client, parts[0], parts[1]);
            }
            else
            {
                // We are just expecting json to be returned
                client = AddClientHeader(client, "Accept", "application/json, text/javascript, */*; q=0.01");
            }
            // Boy!!  Do we need this next line!  **NOTHING** fucking wotks without it!!
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                client = AddClientHeader(client, "Authorization", authorizationToken);
            }
            else
            {
                client = AddClientHeader(client, "X-Requested-With", "XMLHttpRequest");
            }

            StringContent stringContent = new StringContent(jsonContent, System.Text.UnicodeEncoding.UTF8, "application/json");

            response = await PostClientJson(client, TargetUrl, stringContent, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                return response;
                // Bestoke!! <= Bespoke
            }
            // Its ONLY FUCKING WORKED!!!   Time for a cup of tea methinks!!!
            return response;
        }

        internal static bool Is_jsonReturned(HttpResponseMessage response)
        {
            // Examine each header and it's key associated with the response.
            foreach (KeyValuePair<string, IEnumerable<string>> contentHeader in response.Content.Headers) // Checked
            {
                if (contentHeader.Key.Contains("Content-Type"))
                {
                    List<string> keyValues = new List<string>(contentHeader.Value);
                    foreach (string value_row in keyValues) // Checked
                    {
                        if (value_row.Contains("application/json"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        internal static bool Get_Json_String(string response, Action<string> seterror)
        {
            try
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(response);

                if (jsonDoc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    return true;
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                seterror(ex.Message);
            }
            return false;
        }
        // Try as I might, I could not get this routine working as a subroutine
        // So I had to embed it in the main code ... don't know why ..
        // Why?  WHY? Its because you didn't have a FUCKING 'AWAIT' on FU_MyMeter you twat
        // Used in FirstUtility so its a Phase 2
        internal static async Task<string[]> HTTPCLIENT_GET_FILE_ASYNC(MainViewModel ourviewmodel,
                                                                    Uri TargetUrl,
                                                                    CancellationToken cancellationToken,
                                                                    Guid guidKey = new Guid())
        {
            string[] string_array = Array.Empty<string>(); //new string[] { };
            // Most important - check that the timeout isn't zero or some fantastic figure
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
            string response = await GetClientResponseString(client, TargetUrl, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                string_array = response.Split('\n');
            }
            return string_array;
        }

        internal static async Task<bool> ListenerAsync(MainViewModel ourviewmodel,
                                                        CancellationToken cancellationToken,
                                                        short brand_code,
                                                        short supplier_code,
                                                        string routine,
                                                        string information)
        {

            // Note:  UserName SHOULD NEVER, EVER, **EVER** be 'null' or 'empty'!!
            // Note:  All times are in en-GB format

            // During Startup and Shutdown (via Quit/Yes) cancellationToken will equal 'CancellationToken.None'
            // so there is no check or test for this i.e. we can't cancel the Starup or Shutdown Listeners
            // (but they might still timeout ...)

            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());


                string timeNows = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNows, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNows, "", true);
                // Data is encrypted with 'Time Now'
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz("I", timeNows, "", true);
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz("SMARTMUM", timeNows, "", true); // <= SMARTMUM.Listener table
                string encryptedTableName = SmartEncryptionV2016.DoTheBiz("SmartData.Listener", timeNows, "", true); // <= SMARTMUM.Listener table
                int length = information.Length <= 100 ? information.Length : 100;
                string p1 = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) +   // Local time
                                SmartParametersV2016.unitSeparator +
                                brand_code.ToString() + SmartParametersV2016.unitSeparator +
                                supplier_code.ToString() + SmartParametersV2016.unitSeparator +
                                // Make sure we get the FIRST 16 and not the LAST 2!
                                routine + SmartParametersV2016.unitSeparator +
                                information.Substring(0, length);
                string encryptedP1 = SmartEncryptionV2016.DoTheBiz(p1, timeNows, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encryptedDatabaseName),
                    new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, encryptedTableName),
                    new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, encryptedP1)
                };
                if (await WebsiteHandlerPost_Listener(ourviewmodel,
                                                    ourviewmodel.listenerUrl,
                                                    cancellationToken,
                                                    guidKey,
                                                    keyValues))
                {
                    return true;
                }
            }
            // Red or error return false and turns Led8 red
            return false;
        }

        internal static async Task<bool> ListenerAsyncSignIn(SignInViewModel signinviewmodel,
                                                        CancellationToken cancellationToken,
                                                        short brand_code,
                                                        short supplier_code,
                                                        string information,
                                                        [CallerMemberName] string routine = "")
        {
            // Note:  UserName SHOULD NEVER, EVER, **EVER** be 'null' or 'empty'!!
            // Note:  Anything to do with DBServer and the CultureInfo is always en-GB

            // During Startup and Shutdown (via Quit/Yes) cancellationToken will equal 'CancellationToken.None'
            // so there is no check or test for this i.e. we can't cancel the Starup or Shutdown Listeners
            // (but they might still timeout ...)
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());

                string timeNows = (DateTime.Now + signinviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                                                                                                                            // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNows, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(signinviewmodel.antiTokenString, timeNows, "", true);
                // Data is encrypted with 'Time Now'
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz("I", timeNows, "", true);
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz("SMARTMUM", timeNows, "", true); // <= SMARTMUM.Listener table
                string encryptedTableName = SmartEncryptionV2016.DoTheBiz("SmartData.Listener", timeNows, "", true); // <= SMARTMUM.Listener table
                int length = information.Length <= 100 ? information.Length : 100;
                string p1 = (DateTime.Now + signinviewmodel.utcOffset).ToString(SmartParametersV2016.sqliteformat) + // Local time
                                SmartParametersV2016.unitSeparator +
                                brand_code.ToString() + SmartParametersV2016.unitSeparator +
                                supplier_code.ToString() + SmartParametersV2016.unitSeparator +
                                // Make sure we get the FIRST 16 and not the LAST 2!
                                routine + SmartParametersV2016.unitSeparator +
                                information.Substring(0, length);
                string encryptedP1 = SmartEncryptionV2016.DoTheBiz(p1, timeNows, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                        new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                        new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                        new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                        new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encryptedDatabaseName),
                        new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, encryptedTableName),
                        new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, encryptedP1)
                    };
                if (await WebsiteHandlerPost_SignIn(signinviewmodel,
                                                    signinviewmodel.listenerUrl,
                                                    cancellationToken,
                                                    guidKey,
                                                    keyValues))
                {
                    return true;
                }
            }
            // Red or error return false and turns Led8 red
            return false;
        }

#if WINFORMS
        internal static async Task<bool> DBServer_Async(MainViewModel ourviewmodel,
                                                        CancellationToken cancellationToken,
                                                        DashboardModel dashboardmodel,
                                                        string table_name,
                                                        string operation,
                                                        string withdrawn_date,
                                                        [CallerMemberName] string routine = "")
        {
            // From a security point of view, I have written the handler to check this;
            // although in fact a shithead (but not SFBI or SFBII) could mock up their HTTP POST request to mimic
            // this, it make ensures one more barrier within CORS is set-up (because - for
            // example - you cannot write a Sliverlight program which includes 'Referer')
            //

            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(SmartParametersV2016.usernameParameter, ourviewmodel.antiTokenString),
                new KeyValuePair<string, string>(SmartParametersV2016.operationParameter, operation),
                new KeyValuePair<string, string>(SmartParametersV2016.tableParameter, table_name),
                new KeyValuePair<string, string>(SmartParametersV2016.p1Parameter, withdrawn_date),    // May be empty
                new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, ourviewmodel.administrator.ToString())
            };

            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
            client = AddClientHeader(client, "Referer", SmartParametersV2016.permittedReferer);

            // This shit WORKS!  I ain't asking why or how ...
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues)
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            };
            string response = await PostClientString(client,
                                                    ourviewmodel.dbserverUrl,
                                                    multipartFormDataContent,
                                                    cancellationToken,
                                                    err=>ourviewmodel.errorMessage=err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                // When the Handler retrieves this message, it
                // 4. Retrieves the Referer from the header and checks its not empty (our Decoy)
                // 5. Retrieves all the encrypted parameter values and decrypts them
                // 6. It then tries to decrypt the PK parameter with the same private key
                // and compare it to the value found in the Referer.

                // Return codes don't need decrypting
                if (response.Substring(0, 1) != SmartParametersV2016.operationFailure.ToString())
                {
                    // But data does - decrypt it with the 'Time Now' key
                    dashboardmodel.table_count = response;
                    return true;    // We never check for this
                }
            }
            return false;
        }
#endif

        // Phase 1 and Phase 3
        public static async Task<bool> ConnectDisconnectAsync(MainViewModel ourviewmodel,
                                                                string operation,
                                                                StringBuilder multiuser,
                                                                string utcDates,
                                                                string exchangerates_lastdate)
        {
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());

                string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNow, "", true);
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz(operation, timeNow, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                };
                if (operation == SmartParametersV2016.connectSymbol)
                {
                    string encrypted_groups = SmartEncryptionV2016.DoTheBiz(multiuser.ToString(), timeNow, "", true);
                    // We don't encrypt the UTC dates or the last date
                    keyValues.Add(new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encrypted_groups));
                    if (utcDates != "")
                    {
                        keyValues.Add(new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, utcDates));
                        if (exchangerates_lastdate != "")
                        {
                            keyValues.Add(new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, exchangerates_lastdate));
                        }
                    }
                }
                if (await WebsiteHandlerPost_ConnectDisconnect(ourviewmodel,
                                                    ourviewmodel.connectUrl,
                                                    ourviewmodel.quitCts.Token,
                                                    guidKey,
                                                    keyValues,
                                                    operation))
                {
                    return true;
                }

                // If you get here when Led2 is RED, then have you got the host configured
                // as 'MARIAS' when it should be 'localhost' ???

                //  >====  YOU HAVE BEEN WARNED !!!!  ======<
                //      THIS MIGHT RETURN HTTP 500 Error in Fiddler
                //
                // THIS MIGHT BE CAUSED BY
                //    a) The READ access on Keasdon_Energy\web.config not being allowed for IIS_IUSRS (Read & Execute/List Folder/Read)
                //    b) The Read/Execute on Keasdon_Energy\bin not allowing IIS_IUSRS (Read & Execute/List Folder/Read)
                //
                //  >====  YOU HAVE BEEN WARNED !!!!  ======<
                //
                // Now then - we need to indicate an error, but we can't send
                // it (as we may not have a network on which to send).  We
                // could write a console message but we don't have a console
                // to write to ... we are a bit fucked really.  All we can
                // do is turn Led8 to 'red' ...
            }
            // Red or error return false and turns Led8 red
            // SOMETIMES ...we come here when we have sent a Listener!!!
            // ...but were not around to deal with the status that comes back!!
            return false;
        }

        // Use https://stackoverflow.com/questions/54892860/generate-pdf-for-download-using-itext-7-in-mvc-and-net

        //byte[] pdfBytes;
        //using (var stream = new MemoryStream())
        //using (var wri = new PdfWriter(stream))
        //using (var pdf = new PdfDocument(wri))
        //using (var doc = new Document(pdf))
        //{
        //    doc.Add(new Paragraph("Hello World!"));
        //    doc.Close();
        //    doc.Flush();
        //    pdfBytes = stream.ToArray();
        //}
        //return new FileContentResult(pdfBytes, "application/pdf");

        //        internal static async Task<bool> Httpclient_Download_Pdf_Async(MainViewModel ourviewmodel,
        //                                                                        CancellationToken cancellationToken,
        //                                                                        char resource_code,
        //                                                                        Guid guidKey,
        //                                                                        string filename,
        //#if WINUI || analdroid
        //                                                                        PdfDocument pdf_document,
        //#Xelse
        //                                                                        PdfReader pdf_document,
        //#endif
        //                                                                        Action<string> set_errorMessage)
        //        {
        //            // Could possible encrypt pdf_document HERE and the Uri.EscapeDataString it...
        //            // .. is .. silly to do that, but I should be able to (one day) enctypt the
        //            // outgoing parameters.  Hence the DateTime Now string on the front
        //            //
        //            // However I'm going to do the passwords in aspnet first .....

        //            // **==>  THIS ROUTINE FAILS IF YOU ARE TRYING TO CREATE A FILE WITH A FILENAME WHICH IS INVALID <==**

        //            // This is ABSOLUTELY BRILLIANT code, Ray - its the last thing you have to do!!
       //            // Create the memory storage area


        //            string guid = Guid.NewGuid().ToString();
        //            if (!string.IsNullOrEmpty(guid))
        //            {
        //                MemoryStream memoryStream = new MemoryStream();

        //                try
        //                {
        //                    // Store the timeNow with a terminator
        //                    string timeNow = DateTime.Now.ToString(SmartParametersV2016.defaultCulture) + SmartParametersV2016.unitSeparator;  // Local time
        //                    byte[] timeNow_bytes = new byte[timeNow.Length];
        //                    timeNow_bytes = Encoding.UTF8.GetBytes(timeNow);
        //                    memoryStream.Write(timeNow_bytes, 0, timeNow.Length);

        //                    // Store the username with a terminator
        //                    string username = ourviewmodel.UserName + SmartParametersV2016.unitSeparator;
        //                    byte[] username_bytes = new byte[username.Length];
        //                    username_bytes = Encoding.UTF8.GetBytes(username);
        //                    memoryStream.Write(username_bytes, 0, username.Length);

        //                    // Store the Energy resource with a terminator
        //                    // (This is in case we have two bills on the same date for different resources)
        //                    string resource = resource_code.ToString() + SmartParametersV2016.unitSeparator;
        //                    byte[] resource_bytes = new byte[resource.Length];
        //                    resource_bytes = Encoding.UTF8.GetBytes(resource);
        //                    memoryStream.Write(resource_bytes, 0, resource.Length);

        //                    // Store the filename with a terminator
        //                    filename += SmartParametersV2016.unitSeparator;
        //                    byte[] filename_bytes = new byte[filename.Length];
        //                    filename_bytes = Encoding.UTF8.GetBytes(filename);
        //                    memoryStream.Write(filename_bytes, 0, filename.Length);
        //                }
        //                catch (OutOfMemoryException exception)
        //                {
        //                    set_errorMessage(exception.Message);
        //                    return false;
        //                }

        //                //
        //                // So now we have passed four parameters into the memory stream before
        //                // the PDF file contents.  Its looks something like this:
        //                //
        //                // 2016/10/25 12:14:23~BOB~E~20 June 2013~%PDF_whatever ..... etc.
        //                // (where ~ represents the DC1 code for 'our_separator')
        //                //
        //                // Each parameter is terminated by a DC1 so we need to look for FOUR parameters
        //                // at the front of the stream when it all gets to PDF_Download handler
        //                //
        //                try
        //                {
        //#if WPF
        //                    //PdfDocument stamper = new PdfDocument(pdf_document, memoryStream, '\0', true);
        //                    // Do stuff      

        //                    //PdfStamper pdf_stamper = new PdfStamper(pdf_document, memoryStream, );
        //                    //stamper.Writer.CloseStream = false; // Leaves the memory stream Open
        //                    //pdf_stamper.Close();                    // even after the stamper is closed ..(what a pile of bollocks)
        //#endif
        //                }
        //                catch (NullReferenceException exception)
        //                {
        //                    // Well ... the pdf has been parsed and isn't null BUT the
        //                    // Pdf Stamper has found a null sending it into the memrory stream
        //                    // (for some reason
        //                    set_errorMessage(exception.Message);
        //                    return false;
        //                }

        //                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
        //                client = AddClientHeader(client, "Referer", guidKey);
        //
        //                
        //                memoryStream.Position = 0;
        //                byte[] tempBuffer = new byte[memoryStream.Length];
        //                memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
        //#if WINUI || analdroid
        //                memoryStream.Dispose();
        //#Xelse
        //                memoryStream.Close();
        //#endif
        //                ByteArrayContent byteContent = new ByteArrayContent(tempBuffer);
        //                string response = PostClientString(client, ourviewmodel.downloadpdfUrl, byteContent, cancellationToken, err=>ourviewmodel.errorMessage=err);
        //                if (ourviewmodel.errorMessage == "" &&
        //                     response != "")
        //                {
        //                    if (Convert.ToChar(response) == SmartParametersV2016.operationSuccess)
        //                    {
        //                       return true;    // We never check for this
        //                    }
        //                }        
        //            }
        //            // Red or error return false turns Led8 red
        //            return false;
        //        }

        internal static async Task<bool> Insert_COMMON_Async(MainViewModel ourviewmodel,
                                                            CancellationToken cancellationToken,
                                                            StringBuilder bollocks)
        {
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());

                string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNow, "", true);
                // All common inserts, updates etc. are to SmartSwitch
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz(SmartParametersV2016.MainDatabase, timeNow, "", true);

                string[] new_bollocks = bollocks.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                // Third parameter is the number of Bollocks
                string encrypted_bollocks = SmartEncryptionV2016.DoTheBiz(new_bollocks.Length.ToString(), timeNow, "", true);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
                    return false;
                }
                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedDatabaseName),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encrypted_bollocks)
                };
                int record_count = 0;
                foreach (string bollocks_record in new_bollocks) // Checked
                {
                    string table_name = "";
                    string operation = "";
                    string p1 = "";

                    string[] groups = bollocks_record.Split(SmartParametersV2016.groupSeparator);
                    int group_count = 0;
                    foreach (string group in groups) // Checked
                    {
                        switch (group_count)
                        {
                            case 0:
                                table_name += group;
                                break;
                            case 1:
                                operation += group;
                                break;
                            case 2:
                                // Only for EUsage and GUsage which might be long ... but are limited to 1000 rows in any case because
                                // SQL can't insert more than that in one go
                                // No <= use this for everything
                                p1 += group; // parameters HAS to be <= 32766 bytes long or it crashes
                                break;
                            default:
                                break;
                        }
                        group_count++;
                    }
                    string encryptedTableName = SmartEncryptionV2016.DoTheBiz(table_name, timeNow, "", true);
                    string encryptedOperation = SmartEncryptionV2016.DoTheBiz(operation, timeNow, "", true);
                    string encryptedP1 = SmartEncryptionV2016.DoTheBiz(p1, timeNow, "", true);
                    //if (encryptedTableName.Length <= 32766 &&
                    //    encryptedOperation.Length <= 32766) // &&
                    //    //encryptedP1.Length <= 32766)
                    //{
                    string parameter_prfix = SmartParametersV2016.fifthParameter + "." + record_count.ToString() + ".";
                    keyValues.Add(new KeyValuePair<string, string>(parameter_prfix + "1", encryptedTableName));
                    keyValues.Add(new KeyValuePair<string, string>(parameter_prfix + "2", encryptedOperation));
                    keyValues.Add(new KeyValuePair<string, string>(parameter_prfix + "3", encryptedP1));
                    //}
                    //else
                    //{
                    //   set_errorMessage("Parameter too long to attempt UriDataStringEscape");
                    //   return false;
                    //}
                    record_count++;
                }
                if (await WebsiteHandlerPost_InsertCommon(ourviewmodel,
                                            ourviewmodel.insertcommonUrl,
                                            cancellationToken,
                                            guidKey,
                                            keyValues))
                {
                    return true;
                }
            }
            return false;
        }

        internal static async Task<bool> WebsiteHandlerPost_Listener(MainViewModel ourviewmodel,
                                                                Uri TargetUrl,
                                                                CancellationToken cancellationToken,
                                                                string guidKey,
                                                                List<KeyValuePair<string, string>> keyValues)
        {
            // its a 'mustlog' at the Shutdown sequence
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            client = AddClientHeader(client, "Referer", guidKey);
            // This shit WORKS!  I ain't asking why or how ...
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    string.Format("\"{0}\"", keyValuePair.Key));
            }
            //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "CanBeCanceled " + cancellationToken.CanBeCanceled.ToString());
            try
            {
                string response = await PostClientString(client, TargetUrl, multipartFormDataContent, cancellationToken, err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != "")
                {
                    // Reset this little fucker because success from this
                    // routines is as good as a keep alive signal.  
                    // Note: we DON'T reset this in the other routines
                    // because when we are scraping we still want to count
                    // up the seconds to tell SmartDBServer we're still alive ..
                    ourviewmodel.keepaliveCount = 0;
                    string[] results = response.Split(SmartParametersV2016.fileSeparator);
                    int result_count = 0;
                    foreach (string result in results) // Checked
                    {
                        switch (result_count)
                        {
                            case 0:
                                if (Convert.ToChar(result) != SmartParametersV2016.operationSuccess)
                                {
                                    return false;
                                }
                                break;                            
                            default:
                                break;
                        }
                        result_count++;
                    }
                    return true;    // We never check for this
                }
            }
            catch (Exception ex)
            { 
                ourviewmodel.errorMessage = ex.Message; 
            }
            // Error here turns Led8 red
            return false;
        }
        internal static async Task<bool> WebsiteHandlerPost_ConnectDisconnect(MainViewModel ourviewmodel,
                                                                Uri TargetUrl,
                                                                CancellationToken cancellationToken,
                                                                string guidKey,
                                                                List<KeyValuePair<string, string>> keyValues,
                                                                string operation)
        {
            // its a 'mustlog' at the Shutdown sequence
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            client = AddClientHeader(client, "Referer", guidKey);
            // This shit WORKS!  I ain't asking why or how ...
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    string.Format("\"{0}\"", keyValuePair.Key));
            }
            try
            {
                string response = await PostClientString(client, TargetUrl, multipartFormDataContent, cancellationToken, err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != "")
                {
                    // Reset this little fucker because success from this
                    // routines is as good as a keep alive signal.  
                    // Note: we DON'T reset this in the other routines
                    // because when we are scraping we still want to count
                    // up the seconds to tell SmartDBServer we're still alive ..
                    ourviewmodel.keepaliveCount = 0;

                    // Now check the reply                    
                    string[] results = response.Split(SmartParametersV2016.fileSeparator);
                    if (results.Length == 1)
                    {
                        // Expecting a * on a Disconnect
                        if (Convert.ToChar(results[0]) == SmartParametersV2016.operationSuccess &&
                            operation != SmartParametersV2016.connectSymbol)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (operation == SmartParametersV2016.connectSymbol)
                        {
                            int result_count = 0;
                            foreach (string result in results) // Checked
                            {
                                switch (result_count)
                                {
                                    case 0:
                                        await CheckToken(ourviewmodel, result.ToString());
                                        break;
                                    case 1:
                                        if (operation == SmartParametersV2016.connectSymbol)
                                        {
                                            // Only if we are connecting and not disconnecting
                                            // Any Exchange Rates last ('cos there might not be any)
                                            ExchangeRates(ourviewmodel, result);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                result_count++;
                            }
                            return true;    // We never check for this
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
                // Turns Led8 red
            }
            return false;
        }

        internal static async Task<bool> CheckToken(MainViewModel ourviewmodel,
                                        string result)
        {
            switch (result)
            {
                case "T":   // True
                            // Set the UserName colour to Green ... unless its Green already?
#if WINFORMS || WPF || SMARTMAUI
                    if (ourviewmodel.UserNameColour.ToString() != ourviewmodel.greenColour.ToString())
#endif
#if WINUI
                    if (ourviewmodel.UserNameColour != ourviewmodel.greenColour)
#endif
#if ANDROIDX
                    if (ourviewmodel.UserNameColour != ourviewmodel.greenColour)
#endif
                    {
                        // Colour could be Black or Red here ... but we can turn it to Green
                        ourviewmodel.UserNameColour = ourviewmodel.greenColour;
#if ANDROIDX
                        ourviewmodel.AUsername.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.LawnGreen));
#endif
                    }
                    if (!await SmartRoutinesV2018.DefeatTheChimps(ourviewmodel,
                                                        result,
                                                        SmartParametersV2016.SmartUsersSchema,
                                                        "Consumers"))
                    {
                        return false;
                    }
                    break;
                case "F":   // False
                    ourviewmodel.UserNameColour = ourviewmodel.blackColour;
                    break;
                default:
                    // Set the UserName colour to Red ... only if its Black already (i.e. first Connect) 
#if WINFORMS || WPF || SMARTMAUI
                    if (ourviewmodel.UserNameColour.ToString() == ourviewmodel.blackColour.ToString())
#endif
#if WINUI
                    if (ourviewmodel.UserNameColour == ourviewmodel.blackColour)
#endif
#if ANDROIDX
                    if (ourviewmodel.UserNameColour == ourviewmodel.blackColour)
#endif
                    {
                        // Colour can only be Black here
                        ourviewmodel.UserNameColour = ourviewmodel.redColour;
#if ANDROIDX
                            ourviewmodel.AUsername.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
#endif
                    }
                    break;                    
            }
            return true;
        }

        internal static void ExchangeRates(MainViewModel ourviewmodel,
                                            string result)
        {
            if (ourviewmodel.Blanche.exchangeRatesList.Count > 0)
            {
                // Yes! And they might, just might be the latest!
                SmartUsers.ExchangeRates exchangerate_row = new SmartUsers.ExchangeRates();
                string[] erRates = result.Split(SmartParametersV2016.unitSeparator);
                foreach (string erRate in erRates) // Checked
                {
                    string[] newERates = erRate.Split(SmartParametersV2016.fieldSeparator);
                    if (newERates.Length >= 5)
                    {
                        exchangerate_row = new SmartUsers.ExchangeRates()
                        {
                            TRANSACTION_DATE = SmartTimeV2016.ConvertDateTime(newERates[0]),
                            ECB = Convert.ToInt16(newERates[1]),
                            CURRENCY_RATE_01 = Convert.ToDecimal(newERates[2]),
                            CURRENCY_RATE_02 = Convert.ToDecimal(newERates[3]),
                            CURRENCY_RATE_03 = Convert.ToDecimal(newERates[4]),
                            CURRENCY_RATE_04 = Convert.ToDecimal(newERates[5])
                        };
                    }
                }
                if (ourviewmodel.Blanche.exchangeRatesList.Last().TRANSACTION_DATE == exchangerate_row.TRANSACTION_DATE)
                {
                    ourviewmodel.Blanche.exchangeRatesList.Last().ECB = exchangerate_row.ECB;
                    ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_01 = exchangerate_row.CURRENCY_RATE_01;
                    ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_02 = exchangerate_row.CURRENCY_RATE_02;
                    ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_03 = exchangerate_row.CURRENCY_RATE_03;
                    ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_04 = exchangerate_row.CURRENCY_RATE_04;
                }
                else
                {
                    ourviewmodel.Blanche.exchangeRatesList.Add(exchangerate_row);
                }

                ourviewmodel.ECBDate = ourviewmodel.Blanche.exchangeRatesList.Last().TRANSACTION_DATE.ToString(SmartParametersV2016.sqliteformat);
                ourviewmodel.ECBValid = ourviewmodel.Blanche.exchangeRatesList.Last().ECB;
                ourviewmodel.GBPRate = ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_01.ToString();
                ourviewmodel.EURRate = ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_02.ToString();
                ourviewmodel.USDRate = ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_03.ToString();
                //ourviewmodel.CADRate = ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_04.ToString();
                ourviewmodel.JPYRate = ourviewmodel.Blanche.exchangeRatesList.Last().CURRENCY_RATE_04.ToString();

            }
            return;
        }
        internal static async Task<bool> WebsiteHandlerPost_InsertCommon(MainViewModel ourviewmodel,
                                                                Uri TargetUrl,
                                                                CancellationToken cancellationToken,
                                                                string guidKey,
                                                                List<KeyValuePair<string, string>> keyValues)
        {
            // its a 'mustlog' at the Shutdown sequence
            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            client = AddClientHeader(client, "Referer", guidKey);
            // This shit WORKS!  I ain't asking why or how ...
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    string.Format("\"{0}\"", keyValuePair.Key));
            }
            try
            {
                string response = await PostClientString(client, TargetUrl, multipartFormDataContent, cancellationToken, err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                 response != "")
                {
                    // Reset this little fucker because success from this
                    // routines is as good as a keep alive signal.  
                    // Note: we DON'T reset this in the other routines
                    // because when we are scraping we still want to count
                    // up the seconds to tell SmartDBServer we're still alive ..
                    ourviewmodel.keepaliveCount = 0;

                    // Now check the reply
                    string[] results = response.Split(SmartParametersV2016.fileSeparator);
                    int result_count = 0;
                    foreach (string result in results) // Checked
                    {
                        switch (result_count)
                        {
                            case 0:
                                if (Convert.ToChar(result) != SmartParametersV2016.operationSuccess)
                                {
                                    return false;
                                }
                                break;
                            case 1:
                                // Think I'm going to need this later one
                                string[] ray = result.Split(SmartParametersV2016.fieldSeparator);
                                if (ray[0] == ourviewmodel.UserName)
                                {
                                    SmartUsers.ConsumersSQLite consumerxy = new SmartUsers.ConsumersSQLite()
                                    {
                                        USERNAME = ray[0],
                                        LOCAL_ONLY = Convert.ToInt32(ray[1]),
                                        FULL_SCREEN = Convert.ToInt32(ray[2]),
                                        MULTIMETER_ACTIVE = Convert.ToInt32(ray[3]),
                                        CONSUMER_CREATED = Convert.ToDateTime(ray[4]),
                                        SmartUsersConsumers = Convert.ToDateTime(ray[5]),
                                        SmartProfileCubefaces = Convert.ToDateTime(ray[6]),
                                        SmartProfileGroups = Convert.ToDateTime(ray[7]),
                                        SmartProfileProfiles = Convert.ToDateTime(ray[8]),
                                        SmartProfileAddresses = Convert.ToDateTime(ray[9]),
                                        SmartFinanceAccounts = Convert.ToDateTime(ray[10]),
                                        SmartFinanceCryptoAccounts = Convert.ToDateTime(ray[11]),
                                        SmartFinanceCryptoAddresses = Convert.ToDateTime(ray[12]),
                                        SmartFinanceCryptoCurrencyRates = Convert.ToDateTime(ray[13]),
                                        SmartFinanceCryptoLedgers = Convert.ToDateTime(ray[14]),
                                        SmartFinanceCryptoTransactions = Convert.ToDateTime(ray[15]),
                                        SmartFinanceCryptoWallets = Convert.ToDateTime(ray[16]),
                                        SmartFinanceCryptoWalletTotals = Convert.ToDateTime(ray[17]),
                                        SmartFinanceCategories = Convert.ToDateTime(ray[18]),
                                        SmartFinanceCategoryTypes = Convert.ToDateTime(ray[19]),
                                        SmartFinanceConnections = Convert.ToDateTime(ray[20]),
                                        SmartFinanceLogins = Convert.ToDateTime(ray[21]),
                                        SmartFinanceSwitches = Convert.ToDateTime(ray[22]),
                                        SmartFinanceTransactions = Convert.ToDateTime(ray[23]),
                                        SmartFinanceTransactionsCategories = Convert.ToDateTime(ray[24]),
                                        SmartUtilityAccChargesCredits = Convert.ToDateTime(ray[25]),
                                        SmartUtilityAccounts = Convert.ToDateTime(ray[26]),
                                        SmartUtilityBankDetails = Convert.ToDateTime(ray[27]),
                                        SmartUtilityBills = Convert.ToDateTime(ray[28]),
                                        SmartUtilityBillsResource = Convert.ToDateTime(ray[29]),
                                        SmartUtilityECosts = Convert.ToDateTime(ray[30]),
                                        SmartUtilityEDiscounts = Convert.ToDateTime(ray[31]),
                                        SmartUtilityEReadings = Convert.ToDateTime(ray[32]),
                                        SmartUtilityEStandingCharges = Convert.ToDateTime(ray[33]),
                                        SmartUtilityEUnitCharges = Convert.ToDateTime(ray[34]),
                                        SmartUtilityEUsage = Convert.ToDateTime(ray[35]),
                                        SmartUtilityGCosts = Convert.ToDateTime(ray[36]),
                                        SmartUtilityGDiscounts = Convert.ToDateTime(ray[37]),
                                        SmartUtilityGReadings = Convert.ToDateTime(ray[38]),
                                        SmartUtilityGStandingCharges = Convert.ToDateTime(ray[39]),
                                        SmartUtilityGUnitCharges = Convert.ToDateTime(ray[40]),
                                        SmartUtilityGUsage = Convert.ToDateTime(ray[41]),
                                        SmartUtilityLogins = Convert.ToDateTime(ray[42]),
                                        SmartUtilityMeters = Convert.ToDateTime(ray[43]),
                                        SmartUtilityPayments = Convert.ToDateTime(ray[44]),
                                        SmartUtilityResources = Convert.ToDateTime(ray[45]),
                                        SmartUtilityResourcesTypes = Convert.ToDateTime(ray[46]),
                                        SmartUtilitySupChargesCredits = Convert.ToDateTime(ray[47]),
                                        SmartUtilitySwitches = Convert.ToDateTime(ray[48]),
                                        SmartUtilityTariffDetails = Convert.ToDateTime(ray[49]),
                                        SmartUtilityUnallocated = Convert.ToDateTime(ray[50]),
                                        Updated = true
                                    };
                                    ourviewmodel.Hamas.sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>();

                                    ourviewmodel.Hamas.sqliteConsumersList.Add(consumerxy);
                                    // Update the internal DB
                                    if (!await SmartRoutinesV2018.Update_SQLite_Users(ourviewmodel,
                                                                    SmartParametersV2016.SmartUsersSchema))
                                    {
                                        return false;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        result_count++;
                    }
                    return true;    // We never check for this
                }
            }
            catch (Exception ex)
            { 
                ourviewmodel.errorMessage = ex.Message;
            }
            // Turns Led8 red
            return false;
        }
        
        internal static async Task<bool> WebsiteHandlerPost_SignIn(SignInViewModel signinviewmodel,
                                                                Uri TargetUrl,
                                                                CancellationToken cancellationToken,
                                                                string guidKey,
                                                                List<KeyValuePair<string, string>> keyValues)
        {
            HttpClient client = BuildClient(signinviewmodel.cookies, signinviewmodel.timespanTimeout);

            client = AddClientHeader(client, "Referer", guidKey);
            // This shit WORKS!  I ain't asking why or how ...
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
            {
                multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            string response = await PostClientString(client, TargetUrl, multipartFormDataContent, cancellationToken, err => signinviewmodel.errorMessage = err);
            if (signinviewmodel.errorMessage == "" &&
                response != "")
            {
                if (Convert.ToChar(response) == SmartParametersV2016.operationSuccess)
                {
                    return true;    // We never check for this
                }
            }
            // Red or error return false and turns Led8 red
            return false;
        }

        internal static string EncodeString(string str)
        {
            //maxLengthAllowed .NET < 4.5 = 32765;
            //maxLengthAllowed .NET >= 4.5 = 65519;
            int maxLengthAllowed = 32765;
            StringBuilder sb = new StringBuilder();
            int loops = str.Length / maxLengthAllowed;

            for (int i = 0; i <= loops; i++)
            {
                sb.Append(Uri.EscapeDataString(i < loops
                    ? str.Substring(maxLengthAllowed * i, maxLengthAllowed)
                    : str.Substring(maxLengthAllowed * i)));
            }
            return sb.ToString();
        }

#if WINFORMS
        internal static async Task<bool> Stored_Procedure_Async(MainViewModel ourviewmodel,
                                                                string table_name,
                                                                string operation,
                                                                string parameters,
                                                                bool use_escape)
        {
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Username", ourviewmodel.antiTokenString),
                new KeyValuePair<string, string>("Procedure", table_name),
                new KeyValuePair<string, string>("Operation", operation)
            };
            if (use_escape)
            {
                keyValues.Add(new KeyValuePair<string, string>("P1", Uri.EscapeDataString(parameters)));
            }
            else
            {
                // Only for E_Usage and G_Usage which might be long ... but are limited to 1000 rows in any case because
                // SQL can't insert more than that in one go
                keyValues.Add(new KeyValuePair<string, string>("P1", parameters));
            }
            if (await WebsiteHandlerPost_Listener(ourviewmodel,
                                    ourviewmodel.storedproceduresUrl,
                                    ourviewmodel.quitCts.Token,
                                    SmartParametersV2016.permittedReferer,
                                    keyValues))
            {
                return true;
            }
            // Red or error return false and turns Led8 red
            return false;
        }
#endif
        internal static async Task<bool> Load_LOGO_Async(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        CancellationToken cancellationToken,
                                                        char cubefaceCode,
                                                        string brandName)
        {
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());

                string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNow, "", true);
                // DNO is encrypted with 'Time Now'
                string encrypted_brand = SmartEncryptionV2016.DoTheBiz(brandName, timeNow, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, cubefaceCode.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encrypted_brand)
                };

                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

                client = AddClientHeader(client, "Referer", guidKey);
                // This shit WORKS!  I ain't asking why or how ...
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
                Stream response = await PostClientStream(client,
                                                            ourviewmodel.lookuplogoUrl,
                                                            multipartFormDataContent,
                                                            cancellationToken,
                                                            err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response.Length > 0)// != null)
                {
                    financeviewmodel.dno_stream = response;
                    return true;    // We never check for this
                }
            }
            // Red or error return false and turns Led8 red
            return false;
        }

        internal static async Task<bool> Load_DNO_Async(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        CancellationToken cancellationToken,
                                                        char cubefaceCode,
                                                        string supply_dno)
        {
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());


                string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNow, "", true);
                // DNO is encrypted with 'Time Now'
                string encrypted_supply_dno = SmartEncryptionV2016.DoTheBiz(supply_dno, timeNow, "", true);
                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, cubefaceCode.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encrypted_supply_dno)
                };

                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
                client = AddClientHeader(client, "Referer", guidKey);
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
                Stream response = await PostClientStream(client,
                                                    ourviewmodel.lookuplogoUrl,//    lookupdnoUrl,
                                                    multipartFormDataContent,
                                                    cancellationToken,
                                                    err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != null)
                {
                    utilityviewmodel.dno_stream = response;
                    return true;    // We never check for this
                }
            }
            // Red or error return false and turns Led8 red
            return false;
        }

        internal static async Task<List<T>> Load_SingleList_Async<T>(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    CancellationToken cancellationToken,
                                                                    string database_name,
                                                                    string table_name,
                                                                    string operation,
                                                                    string sql)
        {
            List<T> status = new List<T>();
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());


                string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture); // Local time Always en-GB
                // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(ourviewmodel.antiTokenString, timeNow, "", true);
                // Operation is encrypted with 'Time Now'
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz(operation, timeNow, "", true);
                // Database Name is encrypted with 'Time Now'
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz(database_name, timeNow, "", true);
                // Table Name is encrypted with 'Time Now'
                string encryptedTableName = SmartEncryptionV2016.DoTheBiz(table_name, timeNow, "", true);
                // Sql is encrypted with 'Time Now'
                string encrypted_sql = SmartEncryptionV2016.DoTheBiz(sql, timeNow, "", true);
                // Encrypt is encrypted with 'Time Now'
                string encrypted_encrypt = SmartEncryptionV2016.DoTheBiz("False", timeNow, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encryptedDatabaseName),
                    new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, encryptedTableName),
                    new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, encrypted_sql),
                    new KeyValuePair<string, string>(SmartParametersV2016.seventhParameter, encrypted_encrypt)
                };

                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
                client = AddClientHeader(client, "Referer", guidKey);

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
                string response = await PostClientString(client,
                                                    ourviewmodel.loadtableUrl,
                                                    multipartFormDataContent,
                                                    cancellationToken,
                                                    err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != "")
                {

                    // Now ... it could be that the called procedure actually returns
                    // NOTHING or that one (or more) of the Parameters was 'null' (which
                    // is the default when calling a procedure programatically).
                    // For example: 'U', 16, 5, 'G', '', 'Standard', 'Standard'
                    // when passed to LOOKUP_TARIFF_NAMES will actually SUCCEED (so the
                    // operation in SmartDBServer is a 'success') at returning ... nothing
                    // (Because the resource_type parameter is missing)
                    // So 'ray' below could well be empty.
                    // NB: the procedure itself must NEVER fail because of any logic
                    // or table join or table lookup problem.

                    //
                    // ** Need to sort this out - this feels like bollocks **
                    // Well.. sometimes SmartDBServer doesn't return 'result'
                    // This is because Procedures have been tested beforehand and
                    // always return 'True' (although there might be no data records)
                    // so its pointless testing..
                    string[] ray = response.Split(SmartParametersV2016.recordSeparator);
                    status = SmartBouncer.LookLoadSingle<T>(table_name,
                                                        ray,
                                                        ourviewmodel,
                                                        utilityviewmodel);
                    if (ourviewmodel.errorMessage != "")
                    {
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, cancellationToken, 0, 0, "Problem 2: " + operation + " SQL: " + sql + " Error: " + ourviewmodel.errorMessage))
                        {
                            return status;
                        }
                    }
                }
                else
                {
                    // Either errorMessage holds something OR response is empty
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, cancellationToken, 0, 0, "Problem 3: " + operation + " SQL: " + sql + " Result: " + response))
                    {
                        return status;
                    }
                }
                // Note: if we DO stop here, check to see if KeasdonEnergyV2022 has been
                // built with PRODUCTION instead of DEVELOPMENT (its needs to be built with DEVELOPMENT!!)
                // (Building with Development uses 'localhost' whilst Production uses 'www.keadon.co.uk')
            }
            return status;
        }

        internal static List<object> NeverForgetYou(object viewmodel,
                                                    string result,
                                                    string sqlclient)
        {
            // How am I going to split out and RETAIN the
            // DEK keys??  Good question because you have
            // to extract them and propogate them out ...
            List<object> multipleList = new List<object>();

            string table_name;

            string[] bob = result.Split(SmartParametersV2016.groupSeparator);
            foreach (string bobbins in bob) // Checked
            {
                string[] ray = bobbins.Split(SmartParametersV2016.recordSeparator);
                if (ray.Length > 0)
                {
                    string designation = ray[0];
                    string[] split = designation.Split('.');

                    string schema_name = split[0];
                    table_name = split[1];
                    if (ray.Length > 1)
                    {
                        ray = ray[1].Split(SmartParametersV2016.unitSeparator);
                    }
                    else
                    {
                        ray = Array.Empty<string>(); //   new string[] { }; chimp 'advice'
                    }

#if WINFORMS || WPF || SMARTMAUI
                    if (table_name != table_name.ToUpper())
#endif
#if WINUI
                    if (!string.Equals(table_name, table_name.ToUpper()))
#endif
#if ANDROIDX
                    if (!string.Equals(table_name, table_name.ToUpper()))
#endif
                    {
                        if (table_name == "Logins")
                        {
                            Console.WriteLine("Here");
                        }
                        Lower_Case(viewmodel, schema_name, table_name, ray, multipleList, sqlclient);
                    }
                    else
                    {
                        Upper_Case(viewmodel, schema_name, table_name, ray, multipleList); //, rf withdrawn_date);
                    }
                }
            }
            return multipleList;
        }

        internal static async Task<List<object>> LoadMultipleListAsyncSignIn(SignInViewModel signinviewmodel,
                                                                        DateTime meter_timeNow,
                                                                        CancellationToken cancellationToken,
                                                                        Uri loadtableUrl,
                                                                        string anti_token_string,
                                                                        string operation,
                                                                        string database_name,
                                                                        string table_name,
                                                                        string sqlclient)
        {

            List<object> multipleList = new List<object>();
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());

                string timeNow = meter_timeNow.ToString(SmartParametersV2016.sqliteformat); // Always en-GB
                                                                                            // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(anti_token_string, timeNow, "", true);
                // Operation is encrypted with 'Time Now'
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz(operation, timeNow, "", true);
                // Database Name is encrypted with 'Time Now'
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz(database_name, timeNow, "", true);
                // Table Name is encrypted with 'Time Now'
                string encryptedTableName = SmartEncryptionV2016.DoTheBiz(table_name, timeNow, "", true);
                // Sql is encrypted with 'Time Now'
                string encrypted_sql = SmartEncryptionV2016.DoTheBiz(sqlclient, timeNow, "", true);
                // Encrypt is encrypted with 'Time Now'
                //string encrypted_encrypt = SmartEncryptionV2016.DoTheBiz(encrypt.ToString(), timeNow, "", true, set_errorMessage);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encryptedDatabaseName),
                    new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, encryptedTableName),
                    new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, encrypted_sql)
                    //new KeyValuePair<string, string>(SmartParametersV2016.seventhParameter, encrypted_encrypt)
                };

                HttpClient client = BuildClient(signinviewmodel.cookies, signinviewmodel.timespanTimeout);
                client = AddClientHeader(client, "Referer", guidKey);

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
#if SMARTMAUI
                var sw = System.Diagnostics.Stopwatch.StartNew();
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt"),
                    $"[{DateTime.Now:HH:mm:ss}] POST {loadtableUrl} timeout={signinviewmodel.timespanTimeout}\n");
#endif
                string response = await PostClientString(client, loadtableUrl,
                                                            multipartFormDataContent,
                                                            cancellationToken,
                                                            err => signinviewmodel.errorMessage = err);
#if SMARTMAUI
                sw.Stop();
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt"),
                    $"[{DateTime.Now:HH:mm:ss}] Response: {sw.ElapsedMilliseconds}ms, length={response.Length}, error={signinviewmodel.errorMessage}\n");
#endif
                if (signinviewmodel.errorMessage == "" &&
                    response != "")
                {
                    if (response.Length == 1)
                    {
                        if (Convert.ToChar(response.Substring(0, 1)) != SmartParametersV2016.operationSuccess)
                        {
                            signinviewmodel.errorMessage = "Problem with Mother";
                            return multipleList;    // Which should be empty
                            // Something isn't right at the SmartDBServer end
                        }
                    }
                    else
                    {
                        if (response.Length > 0)
                        {
#if SMARTMAUI
                            System.IO.File.AppendAllText(
                                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt"),
                                $"[{DateTime.Now:HH:mm:ss}] Parsing {response.Length} bytes...\n");
#endif
                            multipleList = NeverForgetYou(signinviewmodel,
                                                        response,
                                                        sqlclient);
#if SMARTMAUI
                            System.IO.File.AppendAllText(
                                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_debug.txt"),
                                $"[{DateTime.Now:HH:mm:ss}] Parsed: {multipleList.Count} items, error={signinviewmodel.errorMessage}\n");
#endif
                            if (signinviewmodel.errorMessage != "")
                            {
                                await SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel, cancellationToken, "Problem 4: " + signinviewmodel.errorMessage);
                            }
                        }
                    }
                    
                }
                else
                {
                    // Either errorMessage contains something OR response is empty
                    await SmartRoutinesV2018.CheckTraceSignIn(signinviewmodel, cancellationToken, "Problem 5: " + sqlclient + "Result: " + response);
                }
            }
            return multipleList;   // Which may contain a Count of 0 or a Count > 0
        }

        internal static async Task<List<object>> LoadMultipleListAsyncX(MainViewModel ourviewmodel,
                                                                        DateTime meter_timeNow,
                                                                        CancellationToken cancellationToken,
                                                                        Uri loadtableUrl,
                                                                        string anti_token_string,
                                                                        string operation,
                                                                        string database_name,
                                                                        string table_name,
                                                                        string sqlclient,
                                                                        string schemasList = "")
        {
            List<object> multipleList = new List<object>();
            // Now we come to the neat part ...
            // 1. We create a Guid and send it across as the the Referer as a DECOY.
            // Lets get a Guid now -
            string guidKey = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guidKey))
            {
                // That's set now - the Referer will be seen to change every time
                // 2. Lets get a 'Time Now' key to use for encrypting/decrypting the parameters AND the Data
                int randomKey = SmartRoutinesV2018.GetNextRandomNew(new Random());


                string timeNow = meter_timeNow.ToString(SmartParametersV2016.sqliteformat); // Always en-GB
                                                                                            // 3. We encrypt the first two parameters using our private key
                string encryptedTimeNow = SmartEncryptionV2016.DoTheBiz(timeNow, randomKey.ToString(), "", true);
                // Username is encrypted with 'Time Now'
                string encryptedUsername = SmartEncryptionV2016.DoTheBiz(anti_token_string, timeNow, "", true);
                // Operation is encrypted with 'Time Now'
                string encryptedOperation = SmartEncryptionV2016.DoTheBiz(operation, timeNow, "", true);
                // Database Name is encrypted with 'Time Now'
                string encryptedDatabaseName = SmartEncryptionV2016.DoTheBiz(database_name, timeNow, "", true);
                // Table Name is encrypted with 'Time Now'
                string encryptedTableName = SmartEncryptionV2016.DoTheBiz(table_name, timeNow, "", true);
                // Sql is encrypted with 'Time Now'
                string encrypted_sql = SmartEncryptionV2016.DoTheBiz(sqlclient, timeNow, "", true);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(SmartParametersV2016.zeroParameter, randomKey.ToString()),
                    new KeyValuePair<string, string>(SmartParametersV2016.firstParameter, encryptedTimeNow),
                    new KeyValuePair<string, string>(SmartParametersV2016.secondParameter, encryptedUsername),
                    new KeyValuePair<string, string>(SmartParametersV2016.thirdParameter, encryptedOperation),
                    new KeyValuePair<string, string>(SmartParametersV2016.fourthParameter, encryptedDatabaseName),
                    new KeyValuePair<string, string>(SmartParametersV2016.fifthParameter, encryptedTableName),
                    new KeyValuePair<string, string>(SmartParametersV2016.sixthParameter, encrypted_sql)
                };
                if (schemasList != "")
                {
                    // Encrypt is encrypted with 'Time Now'
                    string encrypted_schemasList = SmartEncryptionV2016.DoTheBiz(schemasList, timeNow, "", true);
                    keyValues.Add(new KeyValuePair<string, string>(SmartParametersV2016.seventhParameter, encrypted_schemasList));
                }

                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);
                client = AddClientHeader(client, "Referer", guidKey);
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> keyValuePair in keyValues) // Checked
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
                string response = await PostClientString(client,
                                                            loadtableUrl,
                                                            multipartFormDataContent,
                                                            cancellationToken,
                                                            err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != "")
                {
                    //string result = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(response))
                    {
                        //if (response.Substring(0, 1) != SmartParametersV2016.operationFailure.ToString())
                        //{
                            if (response.Length > 0)
                            {
                                multipleList = NeverForgetYou(ourviewmodel,
                                                            response,
                                                            sqlclient);
                                if (ourviewmodel.errorMessage != "")
                                {
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, cancellationToken, 0, 0, "Problem 7: " + ourviewmodel.errorMessage))
                                    {
                                        return multipleList;
                                    }
                                }
                            }
                        //}
                        else
                        {
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 8: " + operation + " SQL: " + sqlclient + "Result: " + response))
                            {
                                return multipleList;
                            }
                        }
                    }
                }
            }
            return multipleList;   // Which may contain a Count of 0 or a Count > 0
        }
        internal static void Upper_Case(object viewmodel,
                                        string schema_name,
                                        string table_name,
                                        string[] ray,
                                        List<object> multipleList)
        {
            switch (schema_name)
            {
                case "SmartData":
                    SmartBouncer.DataMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList);
                    break;
                case "SmartUsers":
                    SmartBouncer.MainMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList);
                    break;
                case SmartParametersV2016.SmartFinanceSchema:
                    SmartBouncer.FinanceUpperCaseMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList);
                    break;
                case SmartParametersV2016.SmartUtilitySchema:
                    SmartBouncer.UtilityUpperCaseMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList);
                    break;
                default:
                    break;
            }
            return;
        }
        internal static void Lower_Case(object viewmodel,
                                        string schema_name,
                                        string table_name,
                                        string[] ray,
                                        List<object> multipleList,
                                        string username)
        {
            switch (schema_name)
            {
                case SmartParametersV2016.SmartUsersSchema:
                    SmartBouncer.UsersMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList,
                                                username);
                    break;
                case SmartParametersV2016.SmartProfileSchema:
                    SmartBouncer.ProfileMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList,
                                                username);
                    break;
                case SmartParametersV2016.SmartFinanceSchema:
                    SmartBouncer.FinanceLowerCaseMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList,
                                                username);
                    break;
                case SmartParametersV2016.SmartUtilitySchema:
                    // You are SO FUCKING CLEVER, Ray ... so FUCKING CLEVER
                    // You have balls the size of flying saucers my friend, you really do ..
                    //list_accounts_row.CLOSED = Convert.ToBoolean(items[3]); // Stop here =;-)
                    SmartBouncer.UtilityLowerCaseMultiple(viewmodel,
                                                table_name,
                                                ray,
                                                multipleList,
                                                username);
                    break;
                default:
                    break;
            }
            return;
        }

        //        internal void Dispose()
        //        {
        //            if (client != null)
        //            {
        //                client.Dispose();
        //            }
        //            if (handler != null)
        //            {
        //                handler.Dispose();
        //            }
        ////#if WINUI
        ////            if (pdfReader != null)
        ////            {
        ////                pdfReader = nll;
        ////            }
        ////#Xelse
        ////            if (pdfReader != null)
        ////            {
        ////                pdfReader.Dispose();
        ////            }
        ////#endif
        //            if (stringContent != null)
        //            {
        //                stringContent.Dispose();
        //            }
        //            if (response != null)
        //            {
        //                response.Dispose();
        //            }
        //            if (byteContent != null)
        //            {
        //                byteContent.Dispose();
        //            }
        //            //if (stamper != null)
        //            //{
        //            //    stamper.Dispose();
        //            //}
        //            //if (memoryStream != null)
        //            //{
        //            //    memoryStream.Dispose();
        //            //}
        //            GC.SuppressFinalize(this);
        //            return;
        //        }

        // Used in FirstUtility and NPower so its a Phase 2
        internal static async Task<string> Scraper_Generic_Post_Json(MainViewModel ourviewmodel,
                                                                    CancellationToken cancellationToken,
                                                                    string jsonString,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    string postData)
        {
            string result = "";
            ourviewmodel.errorMessage = "";
            if (!SmartRoutinesV2018.CreateUri(ourviewmodel,
                                                utilityviewmodel.prfix_xxx,
                                                utilityviewmodel.target_pathname))
            {
                ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                        ourviewmodel.errorMessage + SmartParametersV2016.space +
                                        ourviewmodel.errorMessage).Trim();
            }
            else
            {
                result = await HTTPCLIENT_POST_ASYNC_JSON(ourviewmodel,
                                                                ourviewmodel.TargetUrl,
                                                                cancellationToken,
                                                                jsonString,
                                                                postData,           // Usually empty if jsonString isn't
                                                                "",       // Authorization token
                                                                utilityviewmodel.guid);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    (string.IsNullOrEmpty(result)))
                {
                    // <= Contains the status in 'urgent_message'
                    ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                            ourviewmodel.errorMessage).Trim();
                }
            }
            return result;
        }

        internal static async Task<HtmlAgilityPack.HtmlDocument> Scraper_Generic_Post(MainViewModel ourviewmodel,
                                                                                    CancellationToken cancellationToken,
                                                                                    List<KeyValuePair<string, string>> keyValues,
                                                                                    UtilityViewModel utilityviewmodel)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            if (!SmartRoutinesV2018.CreateUri(ourviewmodel,
                                            utilityviewmodel.prfix_xxx,
                                            utilityviewmodel.target_pathname))
            {
                ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                        ourviewmodel.errorMessage + SmartParametersV2016.space +
                                        ourviewmodel.errorMessage).Trim();
            }
            else
            {
                htmlDocument = await HTTPCLIENT_POST_ASYNC(ourviewmodel,
                                                                ourviewmodel.TargetUrl,
                                                                cancellationToken,
                                                                keyValues,
                                                                utilityviewmodel.headers,
                                                                utilityviewmodel.guid);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    (htmlDocument.RemainderOffset == 0))
                {
                    // <= Contains the status in 'urgent_message'
                    ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                            ourviewmodel.errorMessage).Trim();
                }
            }
            return htmlDocument;
        }

        internal static async Task<HtmlAgilityPack.HtmlDocument> Scraper_Generic_Get(MainViewModel ourviewmodel,
                                                                                    CancellationToken cancellationToken,
                                                                                    UtilityViewModel utilityviewmodel)

        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            List<string> headers = new List<string>();  // Could possibly use utilityviewmodel.headers but unsure what it contains

            string uri_errorMessage = "";
            if (!SmartRoutinesV2018.CreateUri(ourviewmodel,
                                            utilityviewmodel.prfix_xxx,
                                            utilityviewmodel.target_pathname)) //<=== IServiceProvider DIFFERENT FROM THE POST!!                                                
            {
                ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                        uri_errorMessage + SmartParametersV2016.space +
                                        ourviewmodel.errorMessage).Trim();
            }
            else
            {
                htmlDocument = await HTTPCLIENT_GET_ASYNC(ourviewmodel,
                                                                    ourviewmodel.TargetUrl,
                                                                    cancellationToken,
                                                                    headers,
                                                                    "",   // Authenticity token
                                                                    utilityviewmodel.guid);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    (htmlDocument.RemainderOffset == 0))
                {
                    // <= Contains the status in 'urgent_message'
                    ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                            ourviewmodel.errorMessage).Trim();
                }
            }
            return htmlDocument;
        }

        // Used in EDF and Switcher so its a Phase 2
        internal static async Task<string> Scraper_Generic_Get_Json(MainViewModel ourviewmodel,
                                                                    CancellationToken cancellationToken,
                                                                    UtilityViewModel utilityviewmodel)

        {
            string reply = "";
            if (!SmartRoutinesV2018.CreateUri(ourviewmodel,
                                            utilityviewmodel.prfix_xxx,
                                            utilityviewmodel.target_pathname)) //<=== IServiceProvider DIFFERENT FROM THE POST!!
            {
                ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                        ourviewmodel.errorMessage + SmartParametersV2016.space +
                                        ourviewmodel.errorMessage).Trim();
            }
            else
            {
                reply = await HTTPCLIENT_GET_ASYNC_JSONX(ourviewmodel,
                                                                    ourviewmodel.TargetUrl,
                                                                    cancellationToken,
                                                                    "",   // Authorization token
                                                                    utilityviewmodel.guid);
                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage) ||
                    (string.IsNullOrEmpty(reply)))
                {
                    // <= Contains the status in 'urgent_message'
                    ourviewmodel.errorMessage = (utilityviewmodel.current_routine + SmartParametersV2016.bar +
                                            ourviewmodel.errorMessage).Trim();
                }
            }
            return reply;
        }

        // This is going to be painful .... She NEVER shuts up ...
        internal static async Task<string> HTTP_UDPRN_GET(MainViewModel ourviewmodel,
                                                    Uri TargetUrl,
                                                    TimeSpan timespanTimeout,
                                                    CancellationToken cancellationToken)
        {
            string response = "";

            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            client = AddClientHeader(client, "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36");
            client = AddClientHeader(client, "Accept", "text/html,application/xhtml+xml,application/xml; q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client = AddClientHeader(client, "Accept-Encoding", "gzip, deflate");
            client = AddClientHeader(client, "Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");

            response = await GetClientResponseMessage(client, TargetUrl, cancellationToken, "", err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != "")
            {
                return response;
            }
            return "";
        }

        internal static async Task<bool> Httpclient_Download_Pdf_Async(MainViewModel ourviewmodel,
                                                                        CancellationToken cancellationToken,
                                                                        char resource_code,
                                                                        Guid guidKey,
                                                                        string filename,
                                                                        PdfReader pdf_document)

        {
            // Could possible encrypt pdf_document HERE and the Uri.EscapeDataString it...
            // .. is .. silly to do that, but I should be able to (one day) enctypt the
            // outgoing parameters.  Hence the DateTime Now string on the front
            //
            // However I'm going to do the passwords in aspnet first .....

            // **==>  THIS ROUTINE FAILS IF YOU ARE TRYING TO CREATE A FILE WITH A FILENAME WHICH IS INVALID <==**

            // This is ABSOLUTELY BRILLIANT code, Ray - its the last thing you have to do!!
            // Create the memory storage area

            string guid = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(guid))
            {
                MemoryStream memoryStream = new MemoryStream();
                try
                {
                    // Store the timeNow with a terminator
                    string timeNow = (DateTime.Now + ourviewmodel.utcOffset).ToString() + SmartParametersV2016.ourSeparator; // Local time
                    byte[] timeNow_bytes = new byte[timeNow.Length];
                    timeNow_bytes = Encoding.UTF8.GetBytes(timeNow);
                    memoryStream.Write(timeNow_bytes, 0, timeNow.Length);

                    // Store the username with a terminator
                    string username = ourviewmodel.UserName + SmartParametersV2016.ourSeparator;
                    byte[] username_bytes = new byte[username.Length];
                    username_bytes = Encoding.UTF8.GetBytes(username);
                    memoryStream.Write(username_bytes, 0, username.Length);

                    // Store the Energy resource with a terminator
                    // (This is in case we have two bills on the same date for different resources)
                    string resource = resource_code.ToString() + SmartParametersV2016.ourSeparator.ToString();
                    byte[] resource_bytes = new byte[resource.Length];
                    resource_bytes = Encoding.UTF8.GetBytes(resource);
                    memoryStream.Write(resource_bytes, 0, resource.Length);

                    // Store the filename with a terminator
                    filename += SmartParametersV2016.ourSeparator;
                    byte[] filename_bytes = new byte[filename.Length];
                    filename_bytes = Encoding.UTF8.GetBytes(filename);
                    memoryStream.Write(filename_bytes, 0, filename.Length);
                }
                catch (OutOfMemoryException exception)
                {
                    ourviewmodel.pdfMessage = exception.Message;
                    return false;
                }

                //
                // So now we have passed four parameters into the memory stream before
                // the PDF file contents.  Its looks something like this:
                //
                // 2016/10/25 12:14:23~BOB~E~20 June 2013~%PDF_whatever ..... etc.
                // (where ~ represents the DC1 code for 'ourSeparator')
                //
                // Each parameter is terminated by a DC1 so we need to look for FOUR parameters
                // at the front of the stream when it all gets to PDF_Download handler
                //
                try
                {
                    //#if WINFORMS || WPF
                    //                    PdfStamper pdf_stamper = new PdfStamper(pdf_document, memoryStream);

                    //#endif
                    //
                    //var writer = new iText.Kernel.Pdf.PdfWriter(memoryStream);
                    //var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                    //var document = new Document(pdf);
                    PdfDocument pdf_stamper = new PdfDocument(pdf_document, new PdfWriter(memoryStream));

                    //#Xelse
                    // I'm unsure as to whether I should delete all this stuff below?
                    // Didn't I have terrible problems with the data not being made permanent
                    // until the memory stream was closed??

                    //stamper = new PdfStamper(pdf_document, memoryStream, '\0', true);
                    // Do stuff      

                    //stamper.Writer.CloseStream = false; // Leaves the memory stream Open
                    //pdf_stamper.Close();                    // even after the stamper is closed ..(what a pile of bollocks)
                }
                catch (NullReferenceException exception)
                {
                    // Well ... the pdf has been parsed and isn't null BUT the
                    // Pdf Stamper has found a null sending it into the memrory stream
                    // (for some reason
                    ourviewmodel.pdfMessage = exception.Message;
                    return false;
                }

                HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

                client = AddClientHeader(client, "Referer", guidKey.ToString());

                memoryStream.Position = 0;

                byte[] tempBuffer = new byte[memoryStream.Length];
                memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                memoryStream.Close();
                memoryStream.Dispose(); // Possible??

                ByteArrayContent byteContent = new ByteArrayContent(tempBuffer);

                string response = await PostClientString(client,
                                                        new Uri(ourviewmodel.website + "/Handlers/Download_PDF.ashx"),
                                                        byteContent,
                                                        cancellationToken,
                                                        err => ourviewmodel.errorMessage = err);
                if (ourviewmodel.errorMessage == "" &&
                    response != "")
                {
                    if (Convert.ToChar(response) == SmartParametersV2016.operationSuccess)
                    {
                        return true;    // We never check for this
                    }
                }
            }
            // Red or error return false turns Led8 red
            return false;
        }

        
        // YES I know we can combine this with the one after
        // I just haven't got fucking time at the moment!
        // Its done now 10-Aug-2024

        internal static bool Check_InnerException(Exception exception, Action<string> seterror)
        {
            bool request_timed_out = false;
            // Cancelled by us pressing the Quit/Yes button OR by a Timeout ... but which?
            // According to this .Net Core 5 bollocks the chimps - in their infinite wisdom
            // have decided to combine these two disparate functions into ONE exception!
            // You just COULDN'T make this fucking bollocks up!!!  The only way to find
            // out which of these two totally different cause - one caused by a human
            // and the other completely out of their control i.e. a network is by checking
            // the InnerExceptionMessage as show below ... What fucking numbskull decided
            // on this is a n absolute and utter chimp.
            if (exception.InnerException != null)
            {
                if (exception.InnerException.Message == "The operation was canceled.")
                {
                    request_timed_out = true;
                    seterror(exception.InnerException.Message + "(Timeout)");
                }
                else
                {
                    seterror(exception.Message);
                }
            }
            else
            {
                seterror(exception.Message);
            }
            return request_timed_out;
        }

        // Used in the Utility Scraper so its Phase 2
        internal static async Task<PdfReader> HTTPCLIENT_GET_PDF_ASYNC(MainViewModel ourviewmodel,
                                                                 CancellationToken cancellationToken,
                                                                 Uri TargetUrl,
                                                                 Guid guidKey = new Guid())
        {
            PdfReader pdfReader = new PdfReader("");

            HttpClient client = BuildClient(ourviewmodel.cookies, ourviewmodel.timespanTimeout);

            Stream response = await GetClientResponseStream(client, TargetUrl, cancellationToken, err => ourviewmodel.errorMessage = err);
            if (ourviewmodel.errorMessage == "" &&
                response != null)
            {
                pdfReader = new PdfReader(response);
            }
            return pdfReader;       // May be empty (or null)
        }
    }
}