//using System.Net;
//using SmartCubeMobile;
//using System.Net.Sockets;
//using System.Diagnostics;
//using System.IO;
//using System.Text.Json.Serialization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;


//using Microsoft.Maui.Controls;

//namespace com.keasdon.messagereceiver
//{
//    public static class SmartReceiver
//    {
//        static HttpListener httpListener;
//        static string Label = "SmartReceiver";

//        // ✅ Event to notify OTP was received
//        public static event Action<MainViewModel, FinanceViewModel, WebView, string> OnOtpReceived;
//        // ✅ Event to notify timeout occurred
//        public static event Action<MainViewModel, FinanceViewModel, WebView, TaskCompletionSource<bool>, bool> OnTimeout;

//        public static async Task StartHttpServer(WebView webView,
//                                    MainViewModel ourviewmodel,
//                                    FinanceViewModel financeviewmodel,
//                                    TaskCompletionSource<bool> tcs,
//                                    bool loggedIn)
//        {
//            if (httpListener != null && httpListener.IsListening)
//            {
//                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, Label + " HTTP server is already running");
//                return;
//            }

//            string localIP = "";

//            try
//            {
//                var host = Dns.GetHostEntry(Dns.GetHostName());

//                localIP = host
//                    .AddressList
//                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
//                    .ToString();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error getting local IP: " + ex.Message);
//                return;
//            }

//            //localIP ?? "Not found";

//            string urlPrefix = $"http://{localIP}:8080/otp/";

//            string args = $"http add urlacl url={urlPrefix} user={ourviewmodel.UserName}";

//            //ProcessStartInfo psi = new ProcessStartInfo
//            //{
//            //    FileName = "netsh",
//            //    Arguments = args,
//            //    Verb = "runas", // Run as Administrator
//            //    UseShellExecute = true,
//            //    WindowStyle = ProcessWindowStyle.Hidden
//            //};

//            var psi = new ProcessStartInfo
//            {
//                FileName = "netsh",
//                Arguments = args,
//                UseShellExecute = true,
//                CreateNoWindow = true,
//                WindowStyle = ProcessWindowStyle.Hidden
//            };
//            try
//            {
//                Process.Start(psi);
//                Console.WriteLine("URL reservation command sent.");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error running netsh: " + ex.Message);
//            }
//            if (string.IsNullOrEmpty(localIP))
//            {
//                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "LocalIP is empty - exiting");
//            }
//            else
//            {
//                httpListener = new HttpListener();
//                httpListener.Prefixes.Add($"http://{localIP}:8080/otp/");
//                httpListener.Start();

//                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, Label + " HTTP server started on: " + string.Join(", ", httpListener.Prefixes));

//                // Otherwise caller won't continue until the HTTP server exits,
//                // which is usually not what you want when waiting for an OTP.
//                _ = Task.Run(async () =>
//                {
//                    try
//                    {
//                        bool shouldStop = false;

//                        while (httpListener.IsListening)
//                        {
//                            using (CancellationTokenSource ctoken = new CancellationTokenSource())
//                            {
//                                try
//                                {
//                                    var contextTask = httpListener.GetContextAsync();
//                                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(SmartParametersV2016.otpTimeout), ctoken.Token);

//                                    var completedTask = await Task.WhenAny(contextTask, timeoutTask);

//                                    if (completedTask == timeoutTask)
//                                    {
//                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "HTTP server timed out after " + SmartParametersV2016.otpTimeout.ToString() + " seconds");
//                                        // ✅ Notify timeout
//                                        if (webView != null)
//                                        {
//                                            try
//                                            {
//                                                OnTimeout?.Invoke(ourviewmodel, financeviewmodel, webView, tcs, loggedIn);
//                                            }
//                                            catch (Exception ex)
//                                            {
//                                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Error during Timeout event: " + ex.Message);
//                                            }
//                                        }
//                                        shouldStop = true;
//                                        break;
//                                    }

//                                    if (completedTask == contextTask)
//                                    {
//                                        var contextResult = contextTask.Result;
//                                        await HandleRequest(ourviewmodel, financeviewmodel, webView, contextResult, ctoken);
                                                                               
//                                    }
//                                }
//                                catch (TaskCanceledException)
//                                {
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "HTTP server wait canceled");
//                                    break;
//                                }
//                                catch (ObjectDisposedException odex)
//                                {
//                                    // Listener or token may be disposed mid-wait
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Disposed exception: {odex.Message}");
//                                    break;
//                                }
//                                catch (OperationCanceledException ocex)
//                                {
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Operation cancelled: {ocex.Message}");
//                                    break;
//                                }
//                                catch (HttpListenerException hlex)
//                                {
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"HttpListener stopped: {hlex.Message}");
//                                    break;
//                                }
//                                catch (Exception ex)
//                                {
//                                    // Log but do not break unless needed
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Unexpected error: {ex.Message}");
//                                }
//                            }
//                        }
//                        if (shouldStop)
//                        {
//                            httpListener.Stop();
//                        }
                    
//                    }
//                    finally
//                    {
//                        httpListener?.Close();
//                        httpListener = null;

//                    }
//                });
//            }
//            return;
//        }

//        private static async Task HandleRequest(MainViewModel ourviewmodel,
//                                                FinanceViewModel financeviewmodel,
//                                                WebView webView,
//                                                HttpListenerContext context,
//                                                CancellationTokenSource ctoken)
//        {
//            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"HandleRequest entry: ");
//            if (context.Request.HttpMethod == "POST")
//            {
//                using (var reader = new StreamReader(context.Request.InputStream))
//                {
//                    string json = await reader.ReadToEndAsync();

//                    try
//                    {
//                        // ✅ Use System.Text.Json instead of Newtonsoft.Json
//                        var data = System.Text.Json.JsonSerializer.Deserialize<MessagePayload>(json);
//                        if (data != null &&
//                            !string.IsNullOrEmpty(data.Origin) &&
//                            !string.IsNullOrEmpty(data.Message))
//                        {
//                            string smsotp = null;

//                            switch (data.Origin.ToUpperInvariant())
//                            {
//                                case "NATIONWIDE":
//                                    string prefix = "Use one-time code";
//                                    if (data.Message.Contains(prefix))
//                                    {
//                                        string stripped = data.Message.Replace(prefix, "").Trim();
//                                        smsotp = stripped.Substring(0, 6).Trim();
//                                    }
//                                    break;

//                                case "TESTER":
//                                    smsotp = data.Message;
//                                    break;

//                                default:
//                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"OTP message received but didn't match expected format: {data.Message}");
//                                    break;
//                            }

//                            if (!string.IsNullOrEmpty(smsotp))
//                            {
//                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"OTP received: {smsotp}");
//                                // ✅ Cancel timeout task
//                                if (ctoken != null && !ctoken.IsCancellationRequested)
//                                {
//                                    ctoken.Cancel();
//                                }

//                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Timeout wait cancelled");

//                                // ✅ Notify listeners
//                                if (webView != null)
//                                {
//                                    try
//                                    {
//                                        OnOtpReceived?.Invoke(ourviewmodel, financeviewmodel, webView, smsotp);
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Error during OTP event: " + ex.Message);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"JSON parse error: {ex.Message}");
//                    }
//                }
//            }

//            // Respond to the client
//            context.Response.StatusCode = 200;
//            using (var output = context.Response.OutputStream)
//            using (var writer = new StreamWriter(output))
//            {
//                writer.Write("Success");
//                writer.Flush(); // Ensures data is written
//            }
//        }
//        internal class MessagePayload
//        {
//            [JsonPropertyName("origin")]
//            public string Origin { get; set; }

//            [JsonPropertyName("message")]
//            public string Message { get; set; }
//        }
//    }
//}