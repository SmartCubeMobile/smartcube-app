using System.Text.RegularExpressions;

#if WINFORMS
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
#endif

#if WPF
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Windows;
#endif

#if UWP || WINUI
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
#endif



#if ANDROIDX
using Android.Content;
using Android.Provider;
#endif


namespace SmartCubeMobile
{
    // Well, I have Dan to thank for showing me chatGPT because
    // it certainly saved my sorry arse at 20:20 today 8th August 2025
    // as I have been struggling for an entire FIVE DAYS to get this
    // bitch working ... but here it is: a routine that finds
    // the anchor containing the href ... clicks it and WAITS
    // for the fucking page to load!  Now .. can I use this bastard
    // anywhere else??  Time for a cuppa, Ray! Time for a cuppa ...

    internal class NationwideWPF
    {
        internal static string UDPRN = "";
        // Some 'necessary evil' globals
        internal static int step = 0;
        internal static string owner = "";
        
        internal static async Task RunNationwideWPF(
                                                    WebView2 FinanceWebView,
#if WINFORMS
                                                    SmartDashboard.MainProcess components,
                                                    RichTextBox textBoxConsole,
#endif
                                                    TaskCompletionSource<bool> tcs,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    SmartFinance.Logins loginInfo,
                                                    string startUrl,
                                                    string notificationTitle,
                                                    string notificationPrefix,
                                                    short notificationTagLength,
                                                    short institution_code,
                                                    short brand_code,
                                                    List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                                    List<SmartFinance.Transaction_Types> transaction_typesFound,
                                                    string Parameter1,
                                                    string Parameter2,
                                                    string Parameter3,
                                                    int loginMethod)
        {
#if WINFORMS
            FinanceWebView.Visible = true;            
#endif
#if WPF
            FinanceWebView.Visibility = Visibility.Visible;
            FinanceWebView.HorizontalAlignment = HorizontalAlignment.Stretch;
            FinanceWebView.VerticalAlignment = VerticalAlignment.Stretch;
#endif
#if WINFORMS
            // See chatGPT standard setup for WebView2 Forms
#endif
            try
            {
                await FinanceWebView.EnsureCoreWebView2Async();
                await FinanceWebView.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = "Ensure WebView2 failed: " + ex.Message;
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                return;
            }

            //financeviewmodel.notificationHandledSource = new TaskCompletionSource<bool>();
            
            FinanceWebView.CoreWebView2.NewWindowRequested += (sender, args) => args.Handled = true;
#if WPF || UWP || WINUI
            ourviewmodel.webviewLogging = false;
            if (ourviewmodel.webviewLogging)
            {
                FinanceWebView.CoreWebView2.OpenDevToolsWindow();
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "WebView2 logging: Enabled");
            }
#endif            
            string logAction = string.Empty;
            string errorMessage = string.Empty;

            string customerNumber = Parameter1;
            string birthDay = Parameter2.Substring(0, 2);
            string birthMonth = SmartParametersV2016.months[int.Parse(Parameter2.Substring(2, 2)) - 1];
            string birthYear = Parameter2.Substring(4,4);
            string passcode = Parameter3;

            //var webViewClient = new CustomNationwideClient(FinanceWebView,
            //                                            ourviewmodel,
            //                                            financeviewmodel,
            //                                            tcs,
            //                                            institution_code,
            //                                            brand_code,
            //                                            customerNumber,
            //                                            birthDay,
            //                                            birthMonth,
            //                                            birthYear,
            //                                            passcode,
            //                                            transaction_groupsFound,
            //                                            transaction_typesFound);
            //FinanceWebView.NavigationCompleted += webViewClient.OnNavigationCompleted;

            FinanceWebView.Source = new Uri(startUrl);

        }
    }
}
