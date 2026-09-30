using Android.Content;
using Android.Provider;
using Android.Util;
using Android.Webkit;
//using static SmartCubeMobile.NationwideController;

namespace SmartCubeMobile
{
    // https://learn.microsoft.com/en-us/dotnet/api/android.manifest.permission.receivesms?view=xamarin-android-sdk-12
    // https://stackoverflow.com/questions/61451407/creating-default-sms-app-in-xamarin-forms
    // With some help from chatGPT
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class SmsReceiver : BroadcastReceiver
    {
        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";
        protected string message, address = "";
        
        
        public override async void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != INTENT_ACTION) // SmartParameters?
            {
                return;
            }
            WebView webView = MainMeter.financeviewmodel.financeadapter.FinanceWebViewFragmentInstance.GetWebView();
            if (webView == null)
            {
                return;
            }
            Bundle bundle = intent.Extras;
            if (bundle != null)
            {
                Java.Lang.Object[] smsArray = (Java.Lang.Object[])intent.Extras.Get("pdus");
                foreach (Java.Lang.Object item in smsArray)
                {
                    Android.Telephony.SmsMessage[] msgs = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                    List<Android.Telephony.SmsMessage> nwmsgs = new List<Android.Telephony.SmsMessage>();
                    foreach (Android.Telephony.SmsMessage msg in msgs)
                    {
                        if (msg.OriginatingAddress == "NATIONWIDE")
                        {
                            nwmsgs.Add(msg);
                        }
                    }
                    if (nwmsgs.Count > 0)
                    {
                        string smsotp = nwmsgs.Last().MessageBody.Replace("Use one-time code ", string.Empty); // 779RCJ to log into the Internet Bank. Never share this code with anyone, only a fr…"
                        smsotp = smsotp.Substring(0, 6).Trim();
                        MainMeter.financeviewmodel.SmsOTP.Text = "Received: " + smsotp;
                        //Toast.MakeText(context, "Received: " + smsotp, ToastLength.Short).Show();

                        string jsOTP = $@"
                        (function() {{
                            var input = document.querySelector('input[name=""OneTimePasscode""]');
                            if (input) {{
                                input.focus();
                                input.value = '{smsotp}';
                                input.dispatchEvent(new Event('input', {{bubbles: true }}));
                                input.dispatchEvent(new Event('change', {{bubbles: true }}));
                                input.blur();
                                return 'Input filled successfully';
                            }}
                            return 'Input not found';
                        }})();";
                        string jsOTPResult = await EvaluateJavaScriptAsync(webView, jsOTP);
                        Log.Debug("WebView", "JsOTP Result: " + jsOTPResult);                        

                        MainMeter.financeviewmodel.loggedIn = false;

                        // Click Continue
                        string jsClickLogin = $@"
                            var btns = Array.from(document.querySelectorAll('button'));
                            var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'log in');
                            if (btn) btn.click();";

                        string loginResult = await EvaluateJavaScriptAsync(webView, jsClickLogin);
                        Log.Debug("WebView", "Login Click Result: " + loginResult);

                        break;
                    }
                }
                return;
            }

            //long GetNotifyTime(DateTime notifyTime)
            //{
            //    DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            //    double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            //    long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            //    return utcAlarmTime; // milliseconds
            //}
        }
        public static async Task<string> EvaluateJavaScriptAsync(WebView webView, string script, int timeoutMs = 5000)
        {
            var tcs = new TaskCompletionSource<string>();

            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

                webView.EvaluateJavascript(script, new JsResultCallback(tcs));

                return await tcs.Task;
            }
        }

        // Helper class for the callback
        public class JsResultCallback : Java.Lang.Object, IValueCallback
        {
            private readonly TaskCompletionSource<string> _tcs;

            public JsResultCallback(TaskCompletionSource<string> tcs)
            {
                _tcs = tcs;
            }

            public void OnReceiveValue(Java.Lang.Object result)
            {
                try
                {
                    string json = result?.ToString();

                    if (!string.IsNullOrEmpty(json) && json != "null")
                    {
                        json = json.Trim('"').Replace("\\u002F", "/");
                    }
                    else
                    {
                        json = null;
                    }

                    _tcs.TrySetResult(json);
                }
                catch (Exception ex)
                {
                    _tcs.TrySetException(ex);
                }
            }
        }
    }
}