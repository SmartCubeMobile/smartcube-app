using System.Net;
using SmartCubeMobile;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Linq;

#if WINFORMS
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using Microsoft.Web.WebView2.Wpf;
#endif

#if UWP || WINUI
using System;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Threading;
#endif

#if ANDROIDX
using System.Threading;
using Android.Content;
using Android.Util;
using Android.Webkit;
using Android.Net;
using Java.Net;
using System.Diagnostics.CodeAnalysis;
#endif

#if SMARTMAUI
using Microsoft.Maui.Controls;
#endif
namespace com.keasdon.messagereceiver
{
    public static class SmartReceiver
    {
        static HttpListener httpListener;
        static string Label = "SmartReceiver";

#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
        // ✅ Event to notify OTP was received
        public static event Action<MainViewModel, FinanceViewModel, WebView2, string> OnOtpReceived;
        // ✅ Event to notify timeout occurred
        public static event Action<MainViewModel, FinanceViewModel, WebView2, TaskCompletionSource<bool>, bool> OnTimeout;
#endif
#if ANDROIDX
        // ✅ Event to notify OTP was received
        public static event Action<MainViewModel, FinanceViewModel, WebView, string> OnOtpReceived;
        // ✅ Event to notify timeout occurred
        public static event Action<MainViewModel, FinanceViewModel, WebView, TaskCompletionSource<bool>, bool> OnTimeout;
#endif
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
        public static async Task StartHttpServer(WebView2 webView,
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    TaskCompletionSource<bool> tcs,
                                    bool loggedIn)
#endif
#if ANDROIDX
        public static void StartHttpServer(WebView webView,
                                    MainViewModel ourviewmodel,
                                    FinanceViewModel financeviewmodel,
                                    TaskCompletionSource<bool> tcs,
                                    bool loggedIn)
#endif
        {
            if (httpListener != null && httpListener.IsListening)
            {
#if WINFORMS || WPF
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, Label + " HTTP server is already running");
#endif
#if ANDROIDX
                Log.Info(Label, "HTTP server is already running");
#endif
                return;
            }

            string localIP = "";
#if ANDROIDX
            // Android 10+ blocks direct access to WiFi info (like IpAddress from WifiInfo)
            // So we use NetworkInterfaces as a fallback
            //foreach (var intf in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    foreach (var addrInfo in intf.GetIPProperties().UnicastAddresses)
            //    {
            //        if (addrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
            //            !IPAddress.IsLoopback(addrInfo.Address))
            //        {
            //            localIP = addrInfo.Address.ToString();


            //        }
            //    }
            //}

            // Android 10+ blocks direct access to WiFi info (like IpAddress from WifiInfo)
            // So we use NetworkInterfaces as a fallback
            var ipAddress = System.Net.NetworkInformation.NetworkInterface
                .GetAllNetworkInterfaces()
                .SelectMany(nic => nic.GetIPProperties().UnicastAddresses)
                .FirstOrDefault(ip =>
                    ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ip.Address));

            localIP = ipAddress != null ? ipAddress.Address.ToString() : string.Empty;
#endif

#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());

                localIP = host
                    .AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                    .ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting local IP: " + ex.Message);
                return;
            }

            //localIP ?? "Not found";

            string urlPrefix = $"http://{localIP}:8080/otp/";

            string args = $"http add urlacl url={urlPrefix} user={ourviewmodel.UserName}";

            //ProcessStartInfo psi = new ProcessStartInfo
            //{
            //    FileName = "netsh",
            //    Arguments = args,
            //    Verb = "runas", // Run as Administrator
            //    UseShellExecute = true,
            //    WindowStyle = ProcessWindowStyle.Hidden
            //};

            var psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = args,
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                Process.Start(psi);
                Console.WriteLine("URL reservation command sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running netsh: " + ex.Message);
            }
#endif
            if (string.IsNullOrEmpty(localIP))
            {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "LocalIP is empty - exiting");
#endif
#if ANDROIDX
                Log.Info(Label, "LocalIP is empty - exiting");
#endif
            }
            else
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://{localIP}:8080/otp/");
                httpListener.Start();
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, Label + " HTTP server started on: " + string.Join(", ", httpListener.Prefixes));
#endif
#if ANDROIDX
                Log.Info(Label, "HTTP server started on: " + string.Join(", ", httpListener.Prefixes));
#endif
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                await Task.Run(async () =>
#endif
#if ANDROIDX
                Task.Run(async () =>
#endif
                {
                    try
                    {
                        bool shouldStop = false;

                        while (httpListener.IsListening)
                        {
                            using (CancellationTokenSource ctoken = new CancellationTokenSource())
                            {
                                try
                                {
                                    var contextTask = httpListener.GetContextAsync();
                                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(SmartParametersV2016.otpTimeout), ctoken.Token);

                                    var completedTask = await Task.WhenAny(contextTask, timeoutTask);

                                    if (completedTask == timeoutTask)
                                    {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "HTTP server timed out after " + SmartParametersV2016.otpTimeout.ToString() + " seconds");
#endif
#if ANDROIDX
                                        Log.Warn(Label, "HTTP server timed out after " + SmartParametersV2016.otpTimeout.ToString() + " seconds");
#endif
                                        // ✅ Notify timeout
                                        if (webView != null)
                                        {
                                            try
                                            {
                                                OnTimeout?.Invoke(ourviewmodel, financeviewmodel, webView, tcs, loggedIn);
                                            }
                                            catch (Exception ex)
                                            {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Error during Timeout event: " + ex.Message);
#endif
#if ANDROIDX
                                                Log.Warn(Label, "Error during Timeout event: " + ex.Message);
#endif
                                            }
                                        }
                                        shouldStop = true;
                                        break;
                                    }

                                    if (completedTask == contextTask)
                                    {
                                        var contextResult = contextTask.Result;
                                        await HandleRequest(ourviewmodel, financeviewmodel, webView, contextResult, ctoken);
                                                                               
                                    }
                                }
                                catch (TaskCanceledException)
                                {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "HTTP server wait canceled");
#endif
#if ANDROIDX
                                    Log.Warn(Label, "HTTP server wait canceled");
#endif
                                    break;
                                }
                                catch (ObjectDisposedException odex)
                                {
                                    // Listener or token may be disposed mid-wait
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Disposed exception: {odex.Message}");
#endif
#if ANDROIDX
                                    Log.Warn(Label, "HTTP object disposed " + odex.Message);
#endif
                                    break;
                                }
                                catch (OperationCanceledException ocex)
                                {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Operation cancelled: {ocex.Message}");
#endif
#if ANDROIDX
                                    Log.Warn(Label, "HTTP operation cancelled " + ocex.Message);
#endif
                                    break;
                                }
                                catch (HttpListenerException hlex)
                                {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"HttpListener stopped: {hlex.Message}");
#endif
#if ANDROIDX
                                    Log.Warn(Label, $"HttpListener stopped: {hlex.Message}");
#endif
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    // Log but do not break unless needed
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Unexpected error: {ex.Message}");
#endif
#if ANDROIDX
                                    Log.Error(Label, $"Unexpected error: {ex.Message}");
#endif
                                }
                            }
                        }
                        if (shouldStop)
                        {
                            httpListener.Stop();
                        }
                    
                    }
                    finally
                    {
                        httpListener?.Close();
                        httpListener = null;

                    }
                });
            }
            return;
        }

#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
        private static async Task HandleRequest(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                WebView2 webView,
                                                HttpListenerContext context,
                                                CancellationTokenSource ctoken)
#endif
#if ANDROIDX
        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
        private static async Task HandleRequest(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        WebView webView,
                                        HttpListenerContext context,
                                        CancellationTokenSource ctoken)
#endif
        {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
            await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"HandleRequest entry: ");
#endif
#if ANDROIDX
    Log.Info(Label, $"HandleRequest entry: ");
#endif

            if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    string json = await reader.ReadToEndAsync();

                    try
                    {
                        // ✅ Use System.Text.Json instead of Newtonsoft.Json
                        var data = System.Text.Json.JsonSerializer.Deserialize<MessagePayload>(json);
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

                                default:
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"OTP message received but didn't match expected format: {data.Message}");
#endif
#if ANDROIDX
                            Log.Warn(Label, $"OTP message received but didn't match expected format: {data.Message}");
#endif
                                    break;
                            }

                            if (!string.IsNullOrEmpty(smsotp))
                            {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"OTP received: {smsotp}");
#endif
#if ANDROIDX
                                Log.Info(Label, $"OTP received: {smsotp}");
#endif

                                // ✅ Cancel timeout task
                                if (ctoken != null && !ctoken.IsCancellationRequested)
                                {
                                    ctoken.Cancel();
                                }

#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"Timeout wait cancelled");
#endif
#if ANDROIDX
                                Log.Info(Label, $"Timeout wait cancelled");
#endif

                                // ✅ Notify listeners
                                if (webView != null)
                                {
                                    try
                                    {
                                        OnOtpReceived?.Invoke(ourviewmodel, financeviewmodel, webView, smsotp);
                                    }
                                    catch (Exception ex)
                                    {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Error during OTP event: " + ex.Message);
#endif
#if ANDROIDX
                                        Log.Warn(Label, "Error during OTP event: " + ex.Message);
#endif
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, $"JSON parse error: {ex.Message}");
#endif
#if ANDROIDX
                Log.Warn(Label, $"JSON parse error: {ex.Message}");
#endif
                    }
                }
            }

            // Respond to the client
            context.Response.StatusCode = 200;
            using (var output = context.Response.OutputStream)
            using (var writer = new StreamWriter(output))
            {
                writer.Write("Success");
                writer.Flush(); // Ensures data is written
            }
        }
        internal class MessagePayload
        {
            [JsonPropertyName("origin")]
            public string Origin { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}