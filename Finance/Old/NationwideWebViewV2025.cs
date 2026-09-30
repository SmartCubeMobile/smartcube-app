using Android.Renderscripts;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Java.Time;
using Xamarin.KotlinX.Coroutines;
using static Android.Media.MediaRouter;
using static Android.Service.Carrier.CarrierMessagingService;


namespace SmartCubeMobile
{
    internal static class WebViewHolder
    {
        internal static WebView SharedWebView { get; set; }
    }

    internal class NationwideWebView
    {
        internal WebView webView;

        internal const int RequestSmsId = 0;
        internal static FinanceViewModel financeviewmodel;

        // Defining this HERE ensures the Receiver is not registerd TWICE
        public static SmsReceiver receiver;

        int step = 0;
        bool stepInProgress = false;

        string startUrlOld = "https://nationwide.co.uk"; // Replace with your actual start URL

        string startUrl = "https://onlinebanking.nationwide.co.uk";///AccessManagement/IdentifyCustomer/IdentifyCustomer";

        string customerNumber = "4293636201";
        string birthDay = "19";
        string birthMonth = "September";
        string birthYear = "1948";
        string passcode = "750504";

        internal static async Task LoadPageAsync(string newLoginJson, //SmartFinance.Logins newLoginjson,
                                         string startUrl,
                                         string NOTIFICATION_TITLE,
                                         string NOTIFICATION_PREFIX,
                                         short NOTIFICATION_TAG_LENGTH,
                                         short institution_code,
                                         short brand_code,
                                         List<SmartFinance.Transaction_Groups> tgfjson,
                                         List<SmartFinance.Transaction_Types> ttfjson,
                                         string CustomerNumber,
                                         string DOB,
                                         string Passcode)
        {
            var tcs = new TaskCompletionSource<bool>();

            //WebView webView = FindViewById<WebView>(Resource.Id.webView);
            WebViewHolder.SharedWebView = webView;

            //SetContentView(financeviewmodel.webView);
            WebViewHolder.SharedWebView.Settings.JavaScriptEnabled = true;
            WebViewHolder.SharedWebView.SetWebViewClient(new CustomWebViewClient(startUrl, tcs));

            await tcs.Task;
        }

        
        public class CustomWebViewClient : WebViewClient
        {
            //NationwideActivity activity;
            private readonly string expectedUrl;
            private readonly TaskCompletionSource<bool> tcs;
            private readonly NationwideActivity activity;

            public CustomWebViewClient(NationwideActivity activity, string expectedUrl, TaskCompletionSource<bool> tcs)
            {
                this.activity = activity;

                this.expectedUrl = expectedUrl;
                this.tcs = tcs;
            }

            public async override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                if (activity.stepInProgress) return;

                activity.stepInProgress = true;

                switch (activity.step)
                {
                    case 0:
                        activity.RunStep0();
                        break;
                    case 1:
                        activity.RunStep1();
                        break;
                    case 2:
                        activity.RunStep2();
                        break;
                    case 3:
                        activity.RunStep3();
                        break;
                    case 4:
                        await activity.RunStep4();
                        break;
                }
            }
        }
    }
    public class NationwideActivity : Activity
    {
        void RunStep0()
        {
            WebViewHolder.SharedWebView.EvaluateJavascript(@"
                (function() {
                    const el = document.querySelector('[class^=""LoginLinks__LoginLink""]');
                    return el ? el.href : null;
                })();", new ValueCallback(result =>
            {
                string loginHref = TrimQuotes(result);
                if (!string.IsNullOrEmpty(loginHref))
                {
                    if (loginHref != "null")    // Yes! The WORD null
                    {
                        WebViewHolder.SharedWebView.LoadUrl(loginHref);
                    }
                    step = 1;
                    RunStep1();
                }
                stepInProgress = false;
            }));
        }

        void RunStep1()
        {
            // Fill login form and click Continue

            string jsCustomer = $@"
            (function() {{
                var input = document.querySelector('input[name=""CustomerNumber""]');
                if (input) {{
                    input.focus();
                    input.value = '{customerNumber}';
                    input.dispatchEvent(new Event('input', {{bubbles: true }}));
                    input.dispatchEvent(new Event('change', {{bubbles: true }}));
                    input.blur();
                    return 'Input filled successfully';
                }}
                return 'Input not found';
            }})();";
            WebViewHolder.SharedWebView.EvaluateJavascript(jsCustomer, new ValueCallback(result =>
            {
                Android.Util.Log.Debug("WebView", "Customer Result: " + result);
            }));

            string jsDay = $@"
            (function() {{
                var input = document.querySelector('input[name=""DateOfBirthDay""]');
                if (input) {{
                    input.focus();
                    input.value = '{birthDay}';
                    input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                    input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                    input.blur();
                    return 'Input filled successfully';
                }}
                return 'Input not found';
            }})();";
            WebViewHolder.SharedWebView.EvaluateJavascript(jsDay, new ValueCallback(result =>
            {
                Android.Util.Log.Debug("WebView", "Day Result: " + result);
            }));

            string jsYear = $@"
                (function() {{var input = document.querySelector('input[name=""DateOfBirthYear""]');
                    if (input) {{
                        input.focus();
                        input.value = '{birthYear}';
                        input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        input.blur();
                        return 'Input filled successfully';
                    }}
                    return 'Input not found';
                }})();";
            WebViewHolder.SharedWebView.EvaluateJavascript(jsYear, new ValueCallback(result =>
            {
                Android.Util.Log.Debug("WebView", "Year Result: " + result);
            }));

            string jsMonth = $@"
                        (function() {{
                            var select = document.querySelector('select[name=""DateOfBirthMonth""]');
                            if (select) {{
                                select.value = '{birthMonth}';
                                var event = new Event('change', {{ bubbles: true }});
                                select.dispatchEvent(event);
                                return 'Month selected';
                            }}
                            return 'Month dropdown not found';
                        }})();";
            WebViewHolder.SharedWebView.EvaluateJavascript(jsMonth, new ValueCallback(result =>
            {
                Android.Util.Log.Debug("WebView", "Month Selection Result: " + result);
            }));

            step = 2;
            //Task.Delay(10000);
            RunStep2();
            stepInProgress = false;
        }

        void RunStep2()
        {
            // Click Continue
            string script = $@"
                    var btns = Array.from(document.querySelectorAll('button'));
                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
                    if (btn) btn.click();";

            WebViewHolder.SharedWebView.EvaluateJavascript(script, null);
            step = 3;
            stepInProgress = false;
        }

        void RunStep3()
        {
            string jsPasscode = @"
                                (() => {
                                    const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']"");
                                    if (radio && radio.offsetParent !== null) {  // Checks for visibility
                                        radio.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                        radio.click();
                                        return 'Clicked';
                                    } else {
                                        return 'NotFound';
                                    }
                                })()";
            WebViewHolder.SharedWebView.EvaluateJavascript(jsPasscode, new ValueCallback(result =>
            {
                Android.Util.Log.Debug("WebView", "Passcode Selection Result: " + result);
            }));
            step = 4;
            RunStep4();


            stepInProgress = false;
        }

        async Task<bool> RunStep4()
        {
            string logAction = "";
            string errorMessage = "";
            if (!await NationwideFindPasscodesAsync(WebViewHolder.SharedWebView,
                                                passcode,
                                                la => logAction = la,
                                                em => errorMessage = em))
            {
                // Some error
            }
            else
            {
                // Click Continue
                string script = $@"
                    var btns = Array.from(document.querySelectorAll('button'));
                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
                    if (btn) btn.click();";

                WebViewHolder.SharedWebView.EvaluateJavascript(script, null);


            }

            //Toast.MakeText(this, "Waiting for OTP...", ToastLength.Short).Show();
            stepInProgress = false;
            step = 5;
            return true;
            // Implement OTP monitoring here if needed
        }

        async Task<bool> RunStep5(FinanceViewModel financeviewmodel)
        {
            WebViewHolder.SharedWebView.EvaluateJavascript(@"
                                            (function() {
                                                const welcomeElem = document.getElementById('welcome-message');
                                                return welcomeElem && welcomeElem.innerText ? welcomeElem.innerText : '';
                                            })();", new ValueCallback(result =>
            {
                string welcomeTextRaw = result;
                string welcomeText = System.Text.Json.JsonDocument.Parse(welcomeTextRaw).RootElement.GetString() ?? "";
                string owner = welcomeText.Replace("Welcome back,", "").Trim();
                financeviewmodel.Welcome.Text = "Owner: " + owner;

                financeviewmodel.UDPRN = "";



                // Well, I have Dan to thank for showing me chatGPT because
                // it certainly saved my sorry arse at 07:37 today 27th May 2025
                // as I have been struggling for an entire TWO DAYS to get this
                // bitch working ... but here it is: a routine that finds
                // the anchor containing the href ... clicks it and WAITS
                // for the fucking page to load!  Now .. can I use this bastard
                // anyehere else??  Time for a cuppa, Ray! Time for a cuppa ...

                string hrefContains = "MaintainTelephoneAndAddress";
                string escapedHref = hrefContains.Replace("'", "\\'");
                string script = $@"
                            (() => {{
                                const link = Array.from(document.querySelectorAll('a[href]')).find(a => a.href.includes('{escapedHref}'));
                                if (link) {{
                                    link.click();
                                    return true;
                                }} else {{
                                    return false;
                                }}
                            }})()
                        ";
                // Navigate to the href and wait for navigation complete event
                WebViewHolder.SharedWebView.EvaluateJavascript(script, null);
                step = 6;
                stepInProgress = false;

                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Contact no.: " + loginInfo.CONTACT_PHONENO);
                //await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Email address: " + loginInfo.CONTACT_EMAIL);

                //financeviewmodel.account_id = 0;




            }));
            //step = 6;
            //stepInProgress = false;
            return true;
            // Implement OTP monitoring here if needed
        }

        async Task<bool> RunStep6(FinanceViewModel financeviewmodel)
        {
            string logAction = "";
            string errorMessage = "";
            string udprn = "";
            try
            {
                // Step 1: Find the address text inside <address> tags
                string getAddressScript = @"
                                    (function() {
                                        let addresses = document.getElementsByTagName('address');
                                        if (addresses.length === 0) return '';
                                        let addrText = addresses[0].innerText || '';
                                        addrText = addrText.replace(/\r?\n/g, ' ').trim();
                                        addrText = addrText.replace(/\s{2,}/g, ' ');
                                        return addrText;
                                    })();
                                ";
                string addressJson = await EvaluateJavascriptAsync(WebViewHolder.SharedWebView, getAddressScript);
                string bankaddress = System.Text.Json.JsonSerializer.Deserialize<string>(addressJson);

                financeviewmodel.Address.Text = bankaddress;

                if (string.IsNullOrWhiteSpace(bankaddress))
                {
                    financeviewmodel.errorMessage = "Bank address empty - cannot continue";
                    return false;
                }

                // Step 2: Lookup existing address in your database or cache
                //var addresses_found = SmartSpikeV2017.UsersLookupTextAddress(ourviewmodel, bankaddress);
                //if (addresses_found.Count > 0)
                //{
                //    if (string.IsNullOrEmpty(addresses_found.First().UDPRN))
                //    {
                //        financeviewmodel.errorMessage = "Address record has no UDPRN - cannot continue";
                //        return udprn;
                //    }
                //    udprn = addresses_found.First().UDPRN;
                //}
                //else
                //{
                //    // Step 5: New address - lookup via external service
                //    var ideal_address = await SmartRoutinesV2018.UsersUDPRNLookup(ourviewmodel, bankaddress);
                //    if (string.IsNullOrEmpty(ideal_address.UDPRN))
                //    {
                //        financeviewmodel.errorMessage = "Cannot determine UDPRN - cannot continue";
                //        return udprn;
                //    }
                //    udprn = ideal_address.UDPRN;

                //    // Add new address record logic here (similar to your original code) ...
                //    // ...
                // }

                // Step 7: Extract email and phone from page via JS
                string getEmailScript = @"
                                        (function() {
                                            let el = document.getElementById('CurrentEmailAddress');
                                            return el ? el.value : '';
                                        })();
                                    ";

                string emailJson = await EvaluateJavascriptAsync(WebViewHolder.SharedWebView, getEmailScript);
                string emailValue = System.Text.Json.JsonSerializer.Deserialize<string>(emailJson);

                financeviewmodel.Email.Text = emailValue;

                //if (loginInfo.CONTACT_EMAIL != emailValue)
                //{
                //    loginInfo.CONTACT_EMAIL = emailValue;
                //    loginInfo.Updated = true;
                //}

                string getPhoneScript = @"
                                        (function() {
                                            let el = document.getElementById('MobilePhoneNumber');
                                            return el ? el.value : '';
                                        })();
                                    ";
                string phoneJson = await EvaluateJavascriptAsync(WebViewHolder.SharedWebView, getPhoneScript);
                string phoneValue = System.Text.Json.JsonSerializer.Deserialize<string>(phoneJson);
                financeviewmodel.PhoneNo.Text = phoneValue;

                //if (loginInfo.CONTACT_PHONENO != phoneValue)
                //{
                //    loginInfo.CONTACT_PHONENO = phoneValue;
                //    loginInfo.Updated = true;
                //}
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }
        static string TrimQuotes(string input)
        {
            if (input == null) return null;
            return input.Trim('"');
        }

        public static Task<string> EvaluateJavascriptAsync(WebView webView, string script)
        {
            var tcs = new TaskCompletionSource<string>();

            webView.EvaluateJavascript(script, new JsResultCallback(tcs));//result =>
            //{
            //    // Clean up the string (removes quotes from JSON-encoded result)
            //    string cleanResult = result?.Trim('"');
            //    tcs.SetResult(cleanResult);
            //})

            return tcs.Task;
        }

        public class StringCallback : Java.Lang.Object, IValueCallback
        {
            private readonly Action<string> _callback;

            public StringCallback(Action<string> callback)
            {
                _callback = callback;
            }

            public void OnReceiveValue(Java.Lang.Object value)
            {
                _callback?.Invoke(value?.ToString());
            }


        }
        
        public class JsResultCallback : Java.Lang.Object, IValueCallback
        {
            private readonly TaskCompletionSource<string> _tcs;
            public JsResultCallback(TaskCompletionSource<string> tcs) => _tcs = tcs;

            public void OnReceiveValue(Java.Lang.Object value)
            {
                _tcs.TrySetResult(value?.ToString());


            }
        }

        internal static async Task<bool> NationwideFindPasscodesAsync(
#if WINFORMS
                                                                        MainProcess components,
                                                                        RichTextBox textBoxConsole,
#endif
                                                                //MainViewModel ourviewmodel,
                                                                //FinanceViewModel financeviewmodel,
                                                                WebView webView,
                                                                string PASSCODE,
                                                                Action<string> logAction,
                                                                Action<string> setError)
        {
            try
            {
                // Step 1: Get spans with class 'control__label__title'
                string script = @"
                                (() => {
                                    const spans = Array.from(document.querySelectorAll('span.control__label__title'));
                                    const result = [];
                                    for (let span of spans) {
                                        if (span.innerText.includes('digits from your passnumber')) {
                                            result.push(span.innerText);
                                        }
                                    }
                                    return result;
                                })()";

                string resultJson = await EvaluateJavascriptAsync(webView, script);


                var spanTexts = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resultJson);

                if (spanTexts == null || spanTexts.Count == 0)
                {
                    logAction("Couldn't find 'passcodes' span class");
                    return false;
                }

                string text = spanTexts[0]; // Use the first match
                int index = 0;
                int[] zzz = new int[3];

                if (text.Contains("1st")) zzz[index++] = 0;
                if (text.Contains("2nd")) zzz[index++] = 1;
                if (text.Contains("3rd")) zzz[index++] = 2;
                if (text.Contains("4th")) zzz[index++] = 3;
                if (text.Contains("5th")) zzz[index++] = 4;
                if (text.Contains("6th")) zzz[index++] = 5;

                string[] dropdownNames = { "FirstPassnumberValue", "SecondPassnumberValue", "ThirdPassnumberValue" };

                for (int j = 0; j < 3; j++)
                {
                    int digitIndex = zzz[j];
                    string digit = PASSCODE.Substring(digitIndex, 1);

                    string jsCode = $@"
                            (function() {{
                                const select = document.querySelector('select[name=""{dropdownNames[j]}""]');
                                if (select) {{
                                    select.value = '{digit}';
                                    const event = new Event('change', {{ bubbles: true }});
                                    select.dispatchEvent(event);
                                    return true;
                                }} else {{
                                    return false;
                                }}
                            }})();
                            ";

                    string selectionResult = await EvaluateJavascriptAsync(webView, jsCode);
                    Log.Debug("WebView", $"Dropdown set: {selectionResult}");



                    //webView.EvaluateJavascript(script, null);
                    //string selectionResult = await webView.ExecuteScriptAsync(selectScript);
                    if (selectionResult.Contains("Missing"))
                    {
                        logAction($"Couldn't find '{dropdownNames[j]}' drop-down input");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                setError(ex.Message);
                return false;
            }
        }
    }
}


//    internal static async Task NationwideScrape()
//    {

//        internal const int RequestSmsId = 0;

//        // Defining this HERE ensures the Receiver is not registerd TWICE
//        public static SmsReceiver receiver;

//        int step = 0;
//        bool stepInProgress = false;

//        //string startUrlOld = "https://nationwide.co.uk"; // Replace with your actual start URL

//        string startUrl = "";//"https://onlinebanking.nationwide.co.uk";///AccessManagement/IdentifyCustomer/IdentifyCustomer";

//        string customerNumber;// = "4293636201";
//        string dateOfBirth = "19091948";
//        string birthDay;// = "19";
//        string birthMonth;// = "September";
//        string birthYear;// = "1948";
//        string passcode;// = "750504";

//        //protected override void OnCreate(Bundle savedInstanceState)
//        //{
//        //    base.OnCreate(savedInstanceState);

//            startUrl = Intent.GetStringExtra("startUrl");
//            customerNumber = Intent.GetStringExtra("customerNumber");
//            dateOfBirth = Intent.GetStringExtra("dateOfBirth");
//            birthDay = dateOfBirth.Substring(0, 2);
//            string birthMonth = dateOfBirth.Substring(2, 2);
//            birthMonth = "September";
//            birthYear = dateOfBirth.Substring(4, 4);
//            passcode = Intent.GetStringExtra("Passcode");

//            SetContentView(Resource.Layout.Nationwide);

//            if (CheckSelfPermission(Manifest.Permission.ReceiveSms) != Android.Content.PM.Permission.Granted)
//            {
//                RequestPermissions(new string[]
//                {
//                    Android.Manifest.Permission.ReceiveSms,
//                    Android.Manifest.Permission.ReadSms
//                },
//                RequestSmsId);
//            }

//            WebView webView = FindViewById<WebView>(Resource.Id.webView);
//            WebViewHolder.SharedWebView = webView;

//            //SetContentView(financeviewmodel.webView);
//            WebViewHolder.SharedWebView.Settings.JavaScriptEnabled = true;
//            WebViewHolder.SharedWebView.SetWebViewClient(new StepWebViewClient(this));

//            WebViewHolder.SharedWebView.LoadUrl(startUrl);
//        }





//        internal void RunStep0()
//        {
//            WebViewHolder.SharedWebView.EvaluateJavascript(@"
//                (function() {
//                    const el = document.querySelector('[class^=""LoginLinks__LoginLink""]');
//                    return el ? el.href : null;
//                })();", new ValueCallback(result =>
//            {
//                string loginHref = TrimQuotes(result);
//                if (!string.IsNullOrEmpty(loginHref))
//                {
//                    if (loginHref != "null")    // Yes! The WORD null
//                    {
//                        WebViewHolder.SharedWebView.LoadUrl(loginHref);
//                    }
//                    step = 1;
//                    RunStep1();
//                }
//                stepInProgress = false;
//            }));
//        }

//        internal void RunStep1()
//        {
//            // Fill login form and click Continue

//            string jsCustomer = $@"
//            (function() {{
//                var input = document.querySelector('input[name=""CustomerNumber""]');
//                if (input) {{
//                    input.focus();
//                    input.value = '{customerNumber}';
//                    input.dispatchEvent(new Event('input', {{bubbles: true }}));
//                    input.dispatchEvent(new Event('change', {{bubbles: true }}));
//                    input.blur();
//                    return 'Input filled successfully';
//                }}
//                return 'Input not found';
//            }})();";            
//            WebViewHolder.SharedWebView.EvaluateJavascript(jsCustomer, new ValueCallback(result => {
//                Android.Util.Log.Debug("WebView", "Customer Result: " + result);
//            }));

//            string jsDay = $@"
//            (function() {{
//                var input = document.querySelector('input[name=""DateOfBirthDay""]');
//                if (input) {{
//                    input.focus();
//                    input.value = '{birthDay}';
//                    input.dispatchEvent(new Event('input', {{ bubbles: true }}));
//                    input.dispatchEvent(new Event('change', {{ bubbles: true }}));
//                    input.blur();
//                    return 'Input filled successfully';
//                }}
//                return 'Input not found';
//            }})();";
//            WebViewHolder.SharedWebView.EvaluateJavascript(jsDay, new ValueCallback(result => {
//                Android.Util.Log.Debug("WebView", "Day Result: " + result);
//            }));

//            string jsYear = $@"
//                (function() {{var input = document.querySelector('input[name=""DateOfBirthYear""]');
//                    if (input) {{
//                        input.focus();
//                        input.value = '{birthYear}';
//                        input.dispatchEvent(new Event('input', {{ bubbles: true }}));
//                        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
//                        input.blur();
//                        return 'Input filled successfully';
//                    }}
//                    return 'Input not found';
//                }})();";
//            WebViewHolder.SharedWebView.EvaluateJavascript(jsYear, new ValueCallback(result => {
//                Android.Util.Log.Debug("WebView", "Year Result: " + result);
//            }));

//            string jsMonth = $@"
//                        (function() {{
//                            var select = document.querySelector('select[name=""DateOfBirthMonth""]');
//                            if (select) {{
//                                select.value = '{birthMonth}';
//                                var event = new Event('change', {{ bubbles: true }});
//                                select.dispatchEvent(event);
//                                return 'Month selected';
//                            }}
//                            return 'Month dropdown not found';
//                        }})();";
//            WebViewHolder.SharedWebView.EvaluateJavascript(jsMonth, new ValueCallback(result => {
//                Android.Util.Log.Debug("WebView", "Month Selection Result: " + result);
//            }));

//            step = 2;
//            //Task.Delay(10000);
//            RunStep2();
//            stepInProgress = false;
//        }

//        internal void RunStep2()
//        {
//            // Click Continue
//            string script = $@"
//                    var btns = Array.from(document.querySelectorAll('button'));
//                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
//                    if (btn) btn.click();";

//            WebViewHolder.SharedWebView.EvaluateJavascript(script, null);
//            step = 3;
//            stepInProgress = false;
//        }

//        internal async void RunStep3()
//        {
//            string jsPasscode = @"
//                                (() => {
//                                    const radio = document.querySelector(""input[type='radio'][value='PassNumberAndSMSOTP']"");
//                                    if (radio && radio.offsetParent !== null) {  // Checks for visibility
//                                        radio.scrollIntoView({ behavior: 'smooth', block: 'center' });
//                                        radio.click();
//                                        return 'Clicked';
//                                    } else {
//                                        return 'NotFound';
//                                    }
//                                })()";
//            WebViewHolder.SharedWebView.EvaluateJavascript(jsPasscode, new ValueCallback(result => {
//                Android.Util.Log.Debug("WebView", "Passcode Selection Result: " + result);
//            }));
//            step = 4;
//            await RunStep4();


//            stepInProgress = false;
//        }

//        internal async Task<bool> RunStep4()
//        {
//            string logAction = "";
//            string errorMessage = "";
//            if (!await NationwideFindPasscodesAsync(WebViewHolder.SharedWebView,
//                                                passcode,
//                                                la => logAction = la,
//                                                em => errorMessage = em))
//            {
//                // Some error
//            }
//            else
//            {
//                // Click Continue
//                string script = $@"
//                    var btns = Array.from(document.querySelectorAll('button'));
//                    var btn = btns.find(b => b.textContent.trim().toLowerCase() === 'continue');
//                    if (btn) btn.click();";

//                WebViewHolder.SharedWebView.EvaluateJavascript(script, null);


//            }

//            Toast.MakeText(this, "Waiting for OTP...", ToastLength.Short).Show();
//            stepInProgress = false;
//            return true;
//            // Implement OTP monitoring here if needed
//        }

//        internal static string TrimQuotes(string input)
//        {
//            if (input == null) return null;
//            return input.Trim('"');
//        }

//        public class ValueCallback : Java.Lang.Object, IValueCallback
//        {
//            private readonly Action<string> callback;
//            public ValueCallback(Action<string> callback)
//            {
//                this.callback = callback;
//            }

//            public void OnReceiveValue(Java.Lang.Object value)
//            {
//                callback?.Invoke(value?.ToString());

//            }
//        }
//}


//        public static Task<string> EvaluateJavascriptAsync(WebView webView, string script)
//        {
//            var tcs = new TaskCompletionSource<string>();

//            webView.EvaluateJavascript(script, new WebViewExtensions.JsResultCallback(tcs));//result =>
//            //{
//            //    // Clean up the string (removes quotes from JSON-encoded result)
//            //    string cleanResult = result?.Trim('"');
//            //    tcs.SetResult(cleanResult);
//            //})

//            return tcs.Task;
//        }
//    }


//    public class StringCallback : Java.Lang.Object, IValueCallback
//    {
//        private readonly Action<string> _callback;

//        public StringCallback(Action<string> callback)
//        {
//            _callback = callback;
//        }

//        public void OnReceiveValue(Java.Lang.Object value)
//        {
//            _callback?.Invoke(value?.ToString());
//        }


//    }
//    public static class WebViewExtensions
//    {
//        //public static Task<string> EvaluateJavascriptAsyncX(this WebView webView, string script)
//        //{
//        //    var tcs = new TaskCompletionSource<string>();
//        //    webView.EvaluateJavascript(script, new JsResultCallback(tcs));
//        //    return tcs.Task;
//        //}



//        public class JsResultCallback : Java.Lang.Object, IValueCallback
//        {
//            private readonly TaskCompletionSource<string> _tcs;
//            public JsResultCallback(TaskCompletionSource<string> tcs) => _tcs = tcs;

//            public void OnReceiveValue(Java.Lang.Object value)
//            {
//                _tcs.TrySetResult(value?.ToString());
//            }
//        }
//    }
//#pragma warning restore CA1416 // Validate platform compatibility

//}