// Today is 16th October 2016 and my dear old Mum would have been 100 years old today!
// Happy Birthday Mum!!!  I loved you more than you ever knew, or would ever know.
// Everything I ever did, I did to please you and make you happy and proud of me.
// You were my entire world then, and you always will be - now and forever.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using BrotliSharpLib;

namespace Bridge
{
    public delegate void XHREventHandler();

    public sealed class XMLHttpRequest
    {
        internal HttpClientHandler handler;

        readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        Uri uri;
        string httpMethod;
        private int _readyState;

        public int ReadyState
        {
            get { return _readyState; }
            private set
            {
                _readyState = value;

                try
                {
                    Onreadystatechange?.Invoke();
                }
                catch
                {
                }
            }
        }

        public string Response => ResponseText;

        public string ResponseText
        {
            get; private set;
        }

        public string ResponseType
        {
            get; private set;
        }

        public bool WithCredentials { get; set; }

        public XHREventHandler Onreadystatechange { get; set; }

        public void SetRequestHeader(string key, string value)
        {
            headers[key] = value;
        }

        public string GetResponseHeader(string key)
        {
            if (headers.ContainsKey(key))
            {
                return headers[key];
            }

            return null;
        }

        public void Open(string method, string url)
        {
            httpMethod = method;
            uri = new Uri(url);
            ReadyState = 1;
        }

        public void Send(object data, object cookies, string url_base, CancellationToken cancellation_token)
        {
            SendAsync(data, cookies, url_base, cancellation_token);
        }

        async void SendAsync(object data, object cookies, string url_base, CancellationToken cancellation_token)
        {
            bool json_found = false,
                    uri_found = false;
            string redirection = string.Empty;

            handler = new HttpClientHandler()
            {
                CookieContainer = (CookieContainer)cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false,
                UseCookies = true,
                UseDefaultCredentials = true
            };

            using (var httpClient = new HttpClient(handler))
            {
                foreach (var header in headers)
                {
                    if (header.Key.StartsWith("Content"))
                    {
                        continue;
                    }
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                // Its ALL ABOUT YOU, isn't it?  Tycoon <= Typhoon
                ReadyState = 2;

                HttpResponseMessage responseMessage = new HttpResponseMessage();
                int loop_302 = 10; // SmartParametersV2016.redirect_302s;

                switch (httpMethod)
                {
                    case "DELETE":
                        responseMessage = await httpClient.DeleteAsync(uri, cancellation_token);
                        break;
                    case "PATCH":
                    case "POST":
                        try
                        {
                            // This next line inhibts the "Expect 100-continue" bollocks which
                            // appears by default in the header
                            httpClient.DefaultRequestHeaders.ExpectContinue = false;
                            
                            responseMessage = await httpClient.PostAsync(uri, (StringContent)data, cancellation_token);
                            while (responseMessage.StatusCode == HttpStatusCode.Redirect ||
                                responseMessage.StatusCode == HttpStatusCode.Moved ||
                                (responseMessage.StatusCode == HttpStatusCode.Found && responseMessage.Headers.Location != null))
                            {
                                if (loop_302 == 0)  // SO we don't go on forever ....
                                {
                                    break;
                                }
                                Uri location = responseMessage.Headers.Location;
                                try
                                {
                                    redirection = location.ToString();

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                //redirection = responseMessage.Headers.Location.AbsoluteUri.ToString();
                                // Well ... we COULD have a loop which picks up lots of 302 re-directs but we are
                                // only expecting ONE, and this approach should be good enough in the short term
                                // Only the Santander fuckers could give us a 302 on a POST!!!
                                //string redirection = redirectUri.AbsolutePath.ToString(); //.Replace(@"file://", string.Empty);
                                if (!string.IsNullOrEmpty(redirection))
                                {
                                    if (redirection.Substring(0, 1) == "/")
                                    {
                                        uri = new Uri(url_base + redirection);
                                    }
                                    else
                                    {
                                        // Otherwise we pick up the PREVIOUS FUCKING URI!!!
                                        uri = new Uri(redirection);
                                    }
                                    responseMessage = await httpClient.GetAsync(uri, cancellation_token);
                                }
                                --loop_302;
                            }
                            json_found = Is_Json_Returned(responseMessage); // Can't seem to get content headers in main program?
                                                                            // So we can set the type of response
                        }
                        catch (HttpRequestException exception)
                        {
                            // Look at the INNER EXCEPTION to see the true cause of failure!!
                            ResponseText = exception.Message;
                            ReadyState = 4;
                        }
                        break;
                    case "GET":
                        try
                        {
                            // This next line inhibts the "Expect 100-continue" bollocks which
                            // appears by default in the header
                            httpClient.DefaultRequestHeaders.ExpectContinue = false;
                            
                            // You have to be careful with this bollocks Microshit crap
                            // If your Net Framework is BELOW 4.6 the it send some bollocks security protocol
                            // by default.  So always make sure the Net Framework is 4.6 or 4.7 or above and it
                            // will send TLS1.2 protocol.  Oh - and don't forget to update all the NuGEt packages
                            responseMessage = await httpClient.GetAsync(uri, cancellation_token);
                            while (responseMessage.StatusCode == HttpStatusCode.Redirect ||
                                responseMessage.StatusCode == HttpStatusCode.Moved ||
                                (responseMessage.StatusCode == HttpStatusCode.Found && responseMessage.Headers.Location != null))
                            {
                                if (loop_302 == 0)  // SO we don't go on forever ....
                                {
                                    break;
                                }
                                Uri location = responseMessage.Headers.Location;
                                try
                                {
                                    redirection = location.ToString();

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                //redirection = responseMessage.Headers.Location.AbsolutePath.ToString();
                                // Well ... we COULD have a loop which picks up lots of 302 re-directs but we are
                                // only expecting ONE, and this approach should be good enough in the short term
                                //string redirection = redirectUri.AbsolutePath.ToString();
                                if (!string.IsNullOrEmpty(redirection))
                                {
                                    if (redirection.Substring(0, 1) == "/")
                                    {
                                        uri = new Uri(url_base + redirection);
                                    }
                                    else
                                    {
                                        // Otherwise we pick up the PREVIOUS FUCKING URI!!!
                                        uri = new Uri(redirection);
                                    }
                                    responseMessage = await httpClient.GetAsync(uri, cancellation_token);
                                }
                                --loop_302;
                            }
                        }
                        catch (HttpRequestException exception)
                        {
                            // Look at the INNER EXCEPTION to see the true cause of failure!!
                            ResponseText = exception.Message;
                            ReadyState = 4;
                        }
                        break;
                }

                if (responseMessage != null)
                {
                    using (responseMessage)
                    {
                        using (var content = responseMessage.Content)
                        {
                            if (uri_found)
                            {
                                ResponseType = "uri";
                                ResponseText = redirection; // redirectUri.ToString();
                            }
                            else
                            {
                                if (json_found)
                                {
                                    ResponseType = "json";
                                }
                                else
                                {
                                    ResponseType = "text";
                                }
                                byte[] buffer = await content.ReadAsByteArrayAsync();
                                if (content.Headers.ContentEncoding.ToString() == "br")
                                {
                                    ByteArrayContent brotli = new ByteArrayContent(Brotli.DecompressBuffer(buffer, 0, buffer.Length));
                                    ResponseText = await brotli.ReadAsStringAsync();
                                }
                                else
                                {
                                    // Sometimes ... when the Content header says utf-8,
                                    // some of the characters inside it aren't!!! 
                                    ResponseText = Encoding.UTF8.GetString(buffer);
                                }                                
                                //Console.WriteLine(ResponseText);
                            }
                            ReadyState = 4;
                        }
                    }
                }
            }
            return;
        }        

        internal static bool Is_Json_Returned(HttpResponseMessage response)
        {
            // Examine each header and it's key associated with the response.
            foreach (KeyValuePair<string, IEnumerable<string>> content_header in response.Content.Headers)
            {
                if (content_header.Key.Contains("Content-Type"))
                {
                    List<string> key_values = content_header.Value.ToList();
                    foreach (string value_row in key_values)
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
    }
}