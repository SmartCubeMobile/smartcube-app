using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using com.keasdon.messagereceiver;
using Newtonsoft.Json;
using static SmartCubeMobile.SmartFinance;


namespace SmartCubeMobile
{
    public class NationwideController
    {
        internal static Context AppContext { get; private set; }
        internal static string loginHref { get; set; } = "";
        internal static int Step { get; set; } = 0;
        internal static bool StepInProgress { get; set; } = false;
        internal static List<AccountLink> AccountLinks { get; set; } = new();
        internal static int CurrentAccountIndex { get; set; } = 0;
        // The vitally important UDPRN!!
        // You CANNOT add ACCOUNTS, TRANSACTIONS or
        // TRANSACTIONSCATEGORIES without a valid
        //     ====> UDPRN <==== 
        // It is **all important**
        internal static string UDPRN = "";
        public class AccountLink
        {
            [JsonProperty("href")]
            public string href { get; set; }

            [JsonProperty("innerHTML")]
            public string innerHTML { get; set; }
        }
        internal static TaskCompletionSource<bool> tcs;
        internal static short institutionCode;
        internal static short brandCode;
        internal static string SortCode = "";
        internal static string AccountNo = "";
        internal static char categoryCode = SmartParametersV2016.defaultChar;
        internal static string symbol = "";
        internal static List<SmartFinance.Transaction_Groups> transaction_groupsFound = new List<SmartFinance.Transaction_Groups>();
        internal static List<SmartFinance.Transaction_Types> transaction_typesFound = new List<SmartFinance.Transaction_Types>();
        internal static string notificationTimer;
        internal static string customerNumber;
        internal static string birthDay;
        internal static string birthMonth;
        internal static string birthYear;
        internal static string passcode;

        internal FinanceWebViewFragment FinanceWebViewFragmentInstance { get; private set; }
        internal static Task RunNationwideAsync(TaskCompletionSource<bool> tcs1001,
#if ANDROIDX
                                                       AppCompatActivity meterActivity,
#endif
                                                       MainViewModel ourviewmodel,
                                                       FinanceViewModel financeviewmodel,
                                                       short institution_code,
                                                       short brand_code,
                                                       List<SmartFinance.Transaction_Groups> transactionGroupsFound,
                                                       List<SmartFinance.Transaction_Types> transactionTypesFound,
                                                       string URL_BASE_SANDBOX,
                                                       string NOTIFICATION_TITLE,
                                                       string Parameter1,
                                                       string Parameter2,
                                                       string Parameter3,
                                                       int LoginMethod)
        {
            try
            {
                tcs = tcs1001;
                institutionCode = institution_code;
                brandCode = brand_code;
                transaction_groupsFound = transactionGroupsFound;
                transaction_typesFound = transactionTypesFound;
                notificationTimer = NOTIFICATION_TITLE;

                customerNumber = Parameter1;
                birthDay = Parameter2.Substring(0, 2);
                birthMonth = SmartParametersV2016.months[int.Parse(Parameter2.Substring(2, 2)) - 1];
                birthYear = Parameter2.Substring(4, 4);
                passcode = Parameter3;
                Log.Debug("DEBUG", "This method was reached");
                FinanceWebViewFragment fragment = SmartFinanceV2025.financeAdapter.FinanceWebViewFragmentInstance;
                WebView webView = fragment.GetWebView();


                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.LoadWithOverviewMode = true;
                webView.Settings.UseWideViewPort = true;
                webView.Settings.BuiltInZoomControls = true;
                webView.Settings.DisplayZoomControls = false;

                // Disable scrollbars if desired
                webView.VerticalScrollBarEnabled = false;
                webView.HorizontalScrollBarEnabled = false;
                // Set the custom WebViewClient
                webView.SetWebViewClient(new CustomWebViewClient(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, financeviewmodel,
                                                                tcs,
                                                                institutionCode, brandCode,
                                                                customerNumber, birthDay, birthMonth, birthYear,
                                                                passcode,
                                                                transactionGroupsFound,
                                                                transactionTypesFound));
                // Load the URL
                fragment.GetWebView().LoadUrl(URL_BASE_SANDBOX);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tcs1001.Task;
        }        
    }
}