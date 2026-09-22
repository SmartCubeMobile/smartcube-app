using System.Net;
using Android.Content;
using Android.Util;
using Android.Webkit;
using Newtonsoft.Json;
using SmartCubeMobile;

namespace com.keasdon.messagereceiver
{
    public static class SmartReceiver
    {
        static HttpListener httpListener;
        static string Label = "SmartReceiver";
        // ✅ Event to notify OTP was received
        public static event Action<MainViewModel, FinanceViewModel, WebView, string, string> OnOtpReceived;
        public static void StartHttpServer(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            WebView webView, 
                                            Context context, 
                                            string notificationTimer)
        {
            if (httpListener != null && httpListener.IsListening)
            {
                Log.Info(Label, "HTTP server is already running.");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    httpListener = new HttpListener();
                    httpListener.Prefixes.Add("http://*:8080/otp/");
                    httpListener.Start();

                    Log.Info(Label, "HTTP server listening on: " + string.Join(", ", httpListener.Prefixes));

                    while (httpListener.IsListening)
                    {




                        HttpListenerContext context = httpListener.GetContext();
                        HandleRequest(ourviewmodel,
                                        financeviewmodel,
                                        webView, 
                                        context, 
                                        notificationTimer);
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(Label, $"HTTP Server error: {ex.Message}");
                }
            });
            return;
        }

        private static async void HandleRequest(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                WebView webView, 
                                                HttpListenerContext context, 
                                                string notificationTimer)
        {
            Log.Info(Label, $"HandleRequest entry: ");
            if (context.Request.HttpMethod == "POST")
            {
                StreamReader reader = new StreamReader(context.Request.InputStream);
                string json = await reader.ReadToEndAsync();
                try
                {
                    MessagePayload data = JsonConvert.DeserializeObject<MessagePayload>(json);
                    if (data != null &&
                        !string.IsNullOrEmpty(data.Origin) &&
                        !string.IsNullOrEmpty(data.Message))
                    {
                        string smsotp = null;

                        switch (data.Origin.ToUpperInvariant())
                        {
                            case "NATIONWIDE":
                                string prefix = "Use one-time code";
                                if (data.Message.Contains(prefix))
                                {
                                    string stripped = data.Message.Replace(prefix, "").Trim();
                                    smsotp = stripped.Substring(0, 6).Trim();
                                }
                                break;
                            case "TESTER":
                                smsotp = data.Message;
                                break;
                        }
                        if (!string.IsNullOrEmpty(smsotp))
                        {
                            Log.Info(Label, $"OTP received: {smsotp}");

                            // ✅ Notify listeners (e.g., NationwideActivity)
                            OnOtpReceived.Invoke(ourviewmodel,
                                                    financeviewmodel,
                                                    webView,
                                                    smsotp, 
                                                    notificationTimer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(Label, $"JSON parse error: {ex.Message}");
                }
                return;
            }

            // Respond back to HTTP client
            context.Response.StatusCode = 200;
            using (var output = context.Response.OutputStream)
            using (var writer = new StreamWriter(output))
            {
                writer.Write("Success");
            }
            return;
        }

        internal class MessagePayload
        {
            [JsonProperty("origin")]
            public string Origin { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}