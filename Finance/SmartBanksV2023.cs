using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

#if WINFORMS
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Windows.Foundation;
#endif

#if WPF
using System.Windows;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Windows.Foundation;
using Microsoft.Web.WebView2.Wpf;
#endif

#if WINUI
using System.Collections.Generic;
using System;
using System.Linq;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
#endif

#if ANDROIDX
//using System.Net;  // No fucking way, mate! Not after all this pain!!
// There is NO NhtmlUnit package which is compatible with MonoAndroid!!!
// So don't waste your time!
using Android.Content;
using Android.Util;
using Android.Net;
using AndroidX.AppCompat.App;
using System.Diagnostics.CodeAnalysis;
using Android.Webkit;
#endif

namespace SmartCubeMobile
{
    public class SmartBanksV2023
    {
#if WINFORMS || WPF  || WINUI
        internal static TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> notificationHandler;

        internal static UserNotificationListener notificationListener = UserNotificationListener.Current;
#endif

        
#if ANDROIDX
        private Dictionary<int, TaskCompletionSource<string>> pendingResults = new();

        //private int nextRequestCode = 1000;
        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
        
#endif
        internal static async Task<bool> FinanceInstitutionalPreLogin(
#if WINFORMS
                                                            WebView2 FinanceWebView,
                                                            SmartDashboard.MainProcess components,
                                                            RichTextBox textBoxConsole,
#endif
#if SMARTMAUI
                                                            WebView FinanceWebView,
#endif
#if WPF || WINUI 
                                                            WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            //List<SmartFinance.Logins> completeList,
                                                            List<SmartFinance.Parent> parents,
                                                            short institutionCode)
        {
            List<SmartFinance.Transaction_Groups> transaction_groupsFound = new List<SmartFinance.Transaction_Groups>();
            List<SmartFinance.Transaction_Types> transaction_typesFound = new List<SmartFinance.Transaction_Types>();

            List<SmartFinance.Institutions> institutionList = SmartSpikeFinanceV2017.Finance_Lookup_Institution(ourviewmodel,
                                                                                                                financeviewmodel,
                                                                                                                institutionCode);
            if (institutionList.Count == 0)
            {
                return false;
            }
            foreach (SmartFinance.Parent Info in parents)
            {
                if (Info.A.INSTITUTION_CODE == institutionCode &&
                    Info.A.ACTIVE_FLAG == 'Y')
                {
                    if (Convert.ToString(Info.B.TWOFACTOR_FLAG) == SmartParametersV2016.yesFlag)
                    {
                        if (string.IsNullOrEmpty(Info.B.URL_PREFIX) ||
                                string.IsNullOrEmpty(Info.B.NOTIFICATION_TITLE) ||
                                string.IsNullOrEmpty(Info.B.NOTIFICATION_PREFIX) ||
                                Info.B.NOTIFICATION_TAGLENGTH <= 0)
                        {
                            continue;
                        }
                        // Do the Two Factors
                        transaction_groupsFound = SmartSpikeFinanceV2017.Finance_Find_TransactionGroupsNew(ourviewmodel,
                                                                                                            financeviewmodel,
                                                                                                            institutionCode,
                                                                                                            Info.B.BRAND_CODE);
                        transaction_typesFound = SmartSpikeFinanceV2017.Finance_Find_TransactionTypesNew(ourviewmodel,
                                                                                                    financeviewmodel,
                                                                                                    institutionCode,
                                                                                                    Info.B.BRAND_CODE);



                        List<SmartFinance.Connections> ours = new List<SmartFinance.Connections>(
                                    from Connections in financeviewmodel.PLO.finance_connectionsList
                                    where Connections.INSTITUTION_CODE == institutionCode &&
                                            Connections.BRAND_CODE == Info.B.BRAND_CODE
                                    select Connections);
                        switch (Info.B.BRAND_CODE)
                        {
                            case 1001:  // Nationwide
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        if (con.Parameter2.Length < 8)
                                        {
#if WINFORMS
                                            SmartDashboard.MainProcess.Output_Message(textBoxConsole, "Invalid DOB " + con.Parameter2, false, false);
#else
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    ourviewmodel, "Invalid DOB " + con.Parameter2);
#endif
                                            return false;
                                        }
                                        if (con.Parameter3.Length < 6)
                                        {
#if WINFORMS
                                            SmartDashboard.MainProcess.Output_Message(textBoxConsole, "Invalid Passcode " + con.Parameter3, false, false);
#else
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    ourviewmodel, "Invalid Passcode " + con.Parameter3);
#endif
                                            return false;
                                        }
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1001 = new TaskCompletionSource<bool>();
                                        Task t = NationwideWPF.RunNationwideWPF(
                                                                                FinanceWebView,
#if WINFORMS
                                                                                components,
                                                                                textBoxConsole,
#endif
                                                                                tcs1001,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                con.Parameter1,
                                                                                con.Parameter2,
                                                                                con.Parameter3,
                                                                                con.LOGIN_METHOD);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if WINFORMS
                                        TaskCompletionSource<bool> tcs1001 = new TaskCompletionSource<bool>();
                                        var nationwide = new NationwideMAUI(FinanceWebView,
                                                                            ourviewmodel,
                                                                            financeviewmodel,
                                                                            tcs1001,
                                                                            Info.B.URL_PREFIX,
                                                                            Info.B.NOTIFICATION_TITLE,
                                                                            Info.B.NOTIFICATION_PREFIX,
                                                                            Info.B.NOTIFICATION_TAGLENGTH,
                                                                            institutionCode,
                                                                            Info.B.BRAND_CODE,
                                                                            transaction_groupsFound,
                                                                            transaction_typesFound);

                                        Task t = nationwide.RunNationwide(
#if WINFORMS
                                                                                components,
                                                                                textBoxConsole,
#endif
                                                                                newLogin,
                                                                                con.Parameter1,
                                                                                con.Parameter2,
                                                                                con.Parameter3,
                                                                                con.LOGIN_METHOD);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        TaskCompletionSource<bool> tcs1001 = new TaskCompletionSource<bool>();
                                        Task t = NationwideController.RunNationwideAsync(tcs1001,
                                                                                meterActivity,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                con.INSTITUTION_CODE,
                                                                                con.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,

                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                con.Parameter1,
                                                                                con.Parameter2,
                                                                                con.Parameter3,
                                                                                con.LOGIN_METHOD);
                                        financeviewmodel.taskList.Add(tcs1001.Task);
                                        Log.Debug("Nationwide", "Task status: " + t.Status);
#endif
                                    }
                                }
                                break;
                            case 1002:  // Santander                                
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string PersonalId = con.Parameter1;
                                        string DOB = con.Parameter2;
                                        string SecurityNumber = con.Parameter3;

                                        if (SecurityNumber.Length != 5)
                                        {
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    ourviewmodel, "Invalid SecurityNumber " + SecurityNumber);
                                            return false;
                                        }
                                        if (DOB.Length < 8)
                                        {
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                    meterActivity,
#endif
                                                                                    ourviewmodel, "Invalid DOB " + DOB);
                                            return false;
                                        }
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1002 = new TaskCompletionSource<bool>();
                                        Task t = SantanderWebView2V2025.RunSantanderWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF || WINUI 
                                                                                FinanceWebView,
#endif
                                                                                tcs1002,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                PersonalId,
                                                                                DOB,
                                                                                SecurityNumber);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        var tcs = new TaskCompletionSource<bool>();
                                        //intent.StartActivityForResult(intent, reqCode);
                                        
                                        //}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        PersonalId,
                                        //                                        DOB,
                                        //                                        SecurityNumber));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 1003:  // Barclays
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        switch (con.LOGIN_METHOD)
                                        {
                                            case 1:
                                                break;
                                            case 2:
                                                break;
                                            case 3:
                                                if (string.IsNullOrEmpty(con.Parameter1) || !con.Parameter1.Contains('/'))
                                                {
                                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                            meterActivity,
#endif
                                                                                            ourviewmodel, "Parameter1: missing or invalid");
                                                    return false;
                                                }

                                                if (string.IsNullOrEmpty(con.Parameter2) || !con.Parameter2.Contains('/'))
                                                {
                                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                            meterActivity,
#endif
                                                                                            ourviewmodel, "Parameter2: missing or invalid");
                                                    return false;
                                                }
                                                if (string.IsNullOrEmpty(con.Parameter3) || !con.Parameter3.Contains('/'))
                                                {
                                                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                                            meterActivity,
#endif
                                                                                            ourviewmodel, "Parameter3: missing or invalid");
                                                    return false;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        //if (DOB.Length < 8)
                                        //{
                                        //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Invalid DOB: " + DOB);
                                        //    return false;
                                        //}
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1003 = new TaskCompletionSource<bool>();
                                        Task t = BarclaysV2025.RunBarclaysWebView2(
                                                                                FinanceWebView,
#if WINFORMS
                                                                                components,
                                                                                textBoxConsole,
#endif
                                                                                tcs1003,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                con.Parameter1,
                                                                                con.Parameter2,
                                                                                con.Parameter3,
                                                                                con.LOGIN_METHOD);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        var tcs = new TaskCompletionSource<bool>();
                                        //intent.StartActivityForResult(intent, reqCode);
                                        
                                        //}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        con.Parameter1,
                                        //                                        con.Parameter2,
                                        //                                        con.Parameter3));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 1004:  // Lloyds                            
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string Username = con.Parameter1;
                                        string Password = con.Parameter2;
                                        string Memorable = con.Parameter3;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };

#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1004 = new TaskCompletionSource<bool>();
                                        Task t = LloydsWebView2.RunLloydsWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF || WINUI 
                                                                                FinanceWebView,
#endif
                                                                                tcs1004,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                Username,
                                                                                Password,
                                                                                Memorable);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        var tcs = new TaskCompletionSource<bool>();
                                        //intent.StartActivityForResult(intent, reqCode);
                                        
                                        //}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        Username,
                                        //                                        Password,
                                        //                                        Memorable));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 10041:  // BankOfScotland                            
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string Username = con.Parameter1;
                                        string Password = con.Parameter2;
                                        string Memorable = con.Parameter3;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs10041 = new TaskCompletionSource<bool>();
                                        Task t = BankofScotlandWebView2.RunBankofScotlandWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF || WINUI 
                                                                                FinanceWebView,
#endif
                                                                                tcs10041,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                Username,
                                                                                Password,
                                                                                Memorable);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);


#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        var tcs = new TaskCompletionSource<bool>();
                                        //intent.StartActivityForResult(intent, reqCode);
                                        
                                        //}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        Username,
                                        //                                        Password,
                                        //                                        Memorable));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 10042:  // Halifax                            
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string Username = con.Parameter1;
                                        string Password = con.Parameter2;
                                        string Memorable = con.Parameter3;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };

#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs10042 = new TaskCompletionSource<bool>();
                                        Task t = HalifaxWebView2.RunHalifaxWebView2(
#if WINFORMS
                                                                                
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if WPF  || WINUI
                                                                                FinanceWebView,
#endif
                                                                                tcs10042,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                Username,
                                                                                Password,
                                                                                Memorable);//,
                                                                                //1);         // Login method!
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);


#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        var tcs = new TaskCompletionSource<bool>();
                                        //intent.StartActivityForResult(intent, reqCode);
                                        
                                        //}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        Username,
                                        //                                        Password,
                                        //                                        Memorable));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 1006:  // TSB                            
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string UserId = con.Parameter1;
                                        string Password = con.Parameter2;
                                        string Memorable = con.Parameter3;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1006 = new TaskCompletionSource<bool>();
                                        Task t = TSBWebView2.RunTSBWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF || WINUI 
                                                                                FinanceWebView,
#endif

                                                                                tcs1006,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                UserId,
                                                                                Password,
                                                                                Memorable);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX

                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        //var tcs = new TaskCompletionSource<bool>();
                                        ////intent.StartActivityForResult(intent, reqCode);

                                        ////}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context,
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        UserId,
                                        //                                        Password,
                                        //                                        Memorable));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 1007:  // NatWest                            
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string CustomerNo = con.Parameter1;
                                        string PIN = con.Parameter2;
                                        string Password = con.Parameter3;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1007 = new TaskCompletionSource<bool>();
                                        Task t = NatWestWebView2.RunNatWestWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF  || WINUI
                                                                                FinanceWebView,
#endif
                                                                                tcs1007,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                CustomerNo,
                                                                                PIN,
                                                                                Password);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);

                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        //var tcs = new TaskCompletionSource<bool>();
                                        ////intent.StartActivityForResult(intent, reqCode);

                                        ////}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        CustomerNo,
                                        //                                        PIN,
                                        //                                        Password));
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            case 1500:  // AJBell                                
                                foreach (SmartFinance.Connections con in ours)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        string Username = con.Parameter1;
                                        string Password = con.Parameter2;
                                        SmartFinance.Logins newLogin = new SmartFinance.Logins()
                                        {
                                            OWNER = Info.A.OWNER,
                                            CONTACT_PHONENO = Info.A.CONTACT_PHONENO,
                                            CONTACT_EMAIL = Info.A.CONTACT_EMAIL
                                        };
#if WINFORMS || WPF  || WINUI
                                        TaskCompletionSource<bool> tcs1500 = new TaskCompletionSource<bool>();
                                        Task t = AJBellWebView2V2025.RunAJBellWebView2(
#if WINFORMS
                                                                                FinanceWebView,
                                                                                components,
                                                                                textBoxConsole,
#endif
#if SMARTMAUI
                                                                                FinanceWebView,
#endif
#if WPF || WINUI 
                                                                                FinanceWebView,
#endif
                                                                                tcs1500,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                newLogin,
                                                                                Info.B.URL_PREFIX,
                                                                                Info.B.NOTIFICATION_TITLE,
                                                                                Info.B.NOTIFICATION_PREFIX,
                                                                                Info.B.NOTIFICATION_TAGLENGTH,
                                                                                institutionCode,
                                                                                Info.B.BRAND_CODE,
                                                                                transaction_groupsFound,
                                                                                transaction_typesFound,
                                                                                Username,
                                                                                Password);
                                        financeviewmodel.taskList.Add(t);
                                        Debug.WriteLine("Added task: " + t.Status);
#endif
#if ANDROIDX
        
                                        //var intent = new Intent(financeviewmodel.context, typeof(NationwideActivity));
                                        string newLoginjson = JsonSerializer.Serialize(newLogin);
                                        //string tgfjson = JsonConvert.SerializeObject(transaction_groupsFound);
                                        //string ttfjson = JsonConvert.SerializeObject(transaction_typesFound);

                                        //var tasks = new List<Task>();


                                        //Toast.MakeText(this, "Results: " + string.Join(", ", results), ToastLength.Long).Show();
                                        //Task<string> resultTask = NationwideActivity(intent);
                                        //financeviewmodel.taskList.Add(resultTask);


                                        //financeviewmodel.context.StartActivity(intent);
                                        //var tcs = new TaskCompletionSource<bool>();
                                        ////intent.StartActivityForResult(intent, reqCode);

                                        ////}));
                                        //financeviewmodel.taskList.Add(NationwideActivity.RunNationwideAsync(financeviewmodel.context, 
                                        //                                        financeviewmodel,
                                        //                                        Info.B.URL_PREFIX,
                                        //                                        Username,
                                        //                                        Password,
                                        //                                        Password)); // <= Take out for AJBELL
                                        //tasks.Add(tcs.Task); // Add the Task to the list
#endif
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // Do the APIs
                        switch (institutionCode)
                        {
                            case 2001:      // Coinbase
                                            //foreach (SmartFinance.LoginsComlete completeInfo in completeList)
                                            //{

                                //    transaction_groupsFound = SmartSpikeFinanceV2017.Finance_Find_TransactionGroupsNew(ourviewmodel,
                                //                                                                                financeviewmodel,
                                //                                                                                institution_code,
                                //                                                                                Info.B.BRAND_CODE);
                                //    transaction_typesFound = SmartSpikeFinanceV2017.Finance_Find_TransactionTypesNew(ourviewmodel,
                                //                                                                                financeviewmodel,
                                //                                                                                institution_code,
                                //                                                                                Info.B.BRAND_CODE);
                                foreach (SmartFinance.Connections con in financeviewmodel.PLO.finance_connectionsList)
                                {
                                    if (con.LOGIN_METHOD == Info.A.LOGIN_METHOD)
                                    {
                                        SmartFinance.DanApiKey key = new SmartFinance.DanApiKey()
                                        {
                                            Username = ourviewmodel.UserName,
                                            AccessId = con.Parameter1,
                                            ApiKey = con.Parameter2,
                                            ApiSecret = con.Parameter3,
                                            Exchange = Info.A.EXCHANGE,
                                        };
                                        Info.ApiKeys.Add(key);
                                        //char categoryCode = SmartParametersV2016.Cryptos;

                                        //                                    financeviewmodel.taskList.Add(CryptoBollocksV2025.CryptoMate(
                                        //#if WINFORMS
                                        //                                                                                                components,
                                        //                                                                                                textBoxConsole,
                                        //#endif
                                        //                                                                                                ourviewmodel,
                                        //                                                                                                financeviewmodel,
                                        //                                                                                                Info.B.URL_BASE_SANDBOX,
                                        //                                                                                                Info.A.EXCHANGE,
                                        //                                                                                                institution_code,
                                        //                                                                                                Info.B.BRAND_CODE,
                                        //                                                                                                categoryCode,
                                        //                                                                                                transaction_groupsFound,
                                        //                                                                                                transaction_typesFound,
                                        //                                                                                                Info.ApiKeys));
                                    }
                                }
                                //}
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            foreach (FinanceViewModel.AccountItem box_item in financeviewmodel.accountItems)
            {
                bool found_it = false;
                foreach (FinanceViewModel.AccountItem account_item in financeviewmodel.FinanceAccountsList)
                {
                    if ((box_item.AccountID == account_item.AccountID) &&
                        (box_item.Content == account_item.Content) &&
                        (box_item.Value.ToString() == account_item.Value.ToString()))
                    {
                        found_it = true;
                        break;
                    }
                }
                if (!found_it)
                {
                    FinanceViewModel.AccountItem account_item = new FinanceViewModel.AccountItem()
                    {
                        AccountID = box_item.AccountID,
                        Content = box_item.Content,
                        Value = box_item.Value,
                        Colour = ourviewmodel.blackColour,
                        IsChecked = false
                    };
                    financeviewmodel.FinanceAccountsList.Add(account_item);
                }
            }
            return true;
        }
        
        internal static void SetupOtpListener(

#if WINFORMS
                                            WebView2 FinanceWebView,
                                            SmartDashboard.MainProcess components,
                                            RichTextBox textBoxConsole,
#endif
#if SMARTMAUI
                                            WebView FinanceWebView,
#endif
#if WPF  || WINUI
                                            WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                            WebView FinanceWebView,
#endif

                                            TaskCompletionSource<bool> tcs,
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewModel,
                                            FinanceViewModel financeviewmodel,
                                            SmartFinance.Logins loginInfo,
                                            string notificationTitle,
                                            string notificationPrefix,
                                            short notificationTagLength,
                                            short institution_code,
                                            short brand_code,
                                            List<SmartFinance.Transaction_Groups> groups,
                                            List<SmartFinance.Transaction_Types> types)
        {
            //string key = $"Timer_{financeviewmodel.timerIdCounter++}";

            // Make it Bank specific
            string key = @"Timer_" + notificationTitle;
            AddWaitTimer(
#if ANDROIDX
                meterActivity, 
#endif
                ourviewModel, financeviewmodel, key, SmartParametersV2016.serverTimeoutSecs,
                onTick: k => { },
                onComplete: async (k, reason) =>
                {
                    if (reason == MainViewModel.TimerFinishReason.Completed)
                    {
                        if (await SmartBanksV2023.WebDriverDead(FinanceWebView,
                            tcs,
#if ANDROIDX
                            meterActivity, 
#endif
                            ourviewModel, financeviewmodel))
                        {
#if WINFORMS || WPF  || WINUI
                            financeviewmodel.notificationHandledSource.TrySetResult(true);
#endif
                        }
                    }
                });

#if WINFORMS || WPF  || WINUI
            // ✅ Set up the handler and keep a reference so it can be unsubscribed later
            notificationHandler = async (sender, e) =>
            {
                await ListenerNotificationChanged(FinanceWebView,
                                                sender, e,

#if WINFORMS
                                                components,
                                                textBoxConsole,
#endif
                                                ourviewModel,
                                                financeviewmodel,
                                                key,
                                                loginInfo,
                                                notificationTitle,
                                                notificationPrefix,
                                                notificationTagLength,
                                                institution_code,
                                                brand_code,
                                                groups,
                                                types);
            };
            // ✅ Subscribe to the event
            notificationListener.NotificationChanged += notificationHandler;
#endif
        }

        internal static async void AddWaitTimer(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, string key, int seconds, Action<string> onTick, Action<string, MainViewModel.TimerFinishReason> onComplete)
        {
            if (IsRunning(financeviewmodel, key))
            {
#if WINFORMS
                System.Windows.Forms.MessageBox.Show($"Timer {key} is already running.");
#endif

                //MessageBox.Show($"Timer {key} is already running.");
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel, $"Timer {key} is already running");
                return;
            }
            financeviewmodel.timers[key] = new MainViewModel.TimerEntry
            {
                RemainingSeconds = seconds,
                OnTick = onTick,
                OnComplete = onComplete
            };

            // Fire initial tick so UI can show full duration immediately            
            //onTick.Invoke(key); <= Don't think I need this here
        }

        internal static bool IsRunning(FinanceViewModel financeviewmodel, string key) => financeviewmodel.timers.ContainsKey(key);

        internal static void CancelTimer(FinanceViewModel financeviewmodel, string key)
        {
            if (financeviewmodel.timers.TryGetValue(key, out var timer))
            {
                timer.OnComplete.Invoke(key, MainViewModel.TimerFinishReason.Cancelled);
                financeviewmodel.timers.Remove(key);
            }
            return;
        }

        internal static async Task<bool> WebDriverDead(
#if SMARTMAUI
                                                WebView FinanceWebView,
#endif
#if WINFORMS || WPF  || WINUI
                                                WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                                WebView FinanceWebView,
#endif
                                                TaskCompletionSource<bool> tcs,
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (FinanceWebView != null)
            {
#if WINFORMS || WPF
                FinanceWebView.Stop(); // Stops navigation/loading
#endif
#if WINUI 
                FinanceWebView.Close(); // Stops navigation/loading
#endif
#if SMARTMAUI
                FinanceWebView.Source = "about:blank";
#endif
                //#if ANDROIDX
                //                FinanceWebViewX.Dispose(); // Stops navigation/loading Does it? Sure??
                //#endif
            }
#endif
            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "FinanceWebView: " + "Quit completed");
            tcs.SetResult(true);
            return true;
        }

#if WINFORMS || WPF  || WINUI
        internal static async Task ListenerNotificationChanged(
                                WebView2 FinanceWebView,

                                UserNotificationListener sender,
                                UserNotificationChangedEventArgs args,
#if WINFORMS
                                SmartDashboard.MainProcess components,
                                RichTextBox textBoxConsole,
#endif
                                MainViewModel ourviewmodel,
                                FinanceViewModel financeviewmodel,
                                string key,
                                SmartFinance.Logins loginInfo,
                                string notificationTitle,
                                string notificationPrefix,
                                short notificationTagLength,
                                short institutionCode,
                                short brandCode,
                                List<SmartFinance.Transaction_Groups> transaction_groupsFound,
                                List<SmartFinance.Transaction_Types> transaction_typesFound)
        {
            try
            {
#if WINFORMS  || ANDROID
                await Task.Run(async () =>  
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Unsubscribed"));
#endif
#if WPF
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {                
                    // ✅ Unsubscribe this exact handler
                    if (notificationHandler != null)
                    {
                        if (notificationListener != null)
                        {
                            notificationListener.NotificationChanged -= notificationHandler;
                            notificationHandler = null; // prevent reuse
                        }
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Unsubscribed");
                    }
                });
#endif
#if WINUI
                // Ensure this line is within a method marked as async
                if (!ourviewmodel.Dispatcher.HasThreadAccess)
                {
                    await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                    {
                        if (notificationListener != null && notificationHandler != null)
                        {
                            notificationListener.NotificationChanged -= notificationHandler;
                            notificationHandler = null; // prevent reuse
                        }

                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Unsubscribed");
                    });
                }
#endif
                CancelTimer(financeviewmodel, key);

#if WINUI
                await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Delivered");
                });
#else
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Handler Event: Delivered");
#endif
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institutionCode, brandCode, "Handler: event delivered"))
                {
                    return;
                }
                var notifs = await sender.GetNotificationsAsync(NotificationKinds.Toast);
                var notifsSorted = notifs.OrderByDescending(n => n.CreationTime).ToList();

                if (notifsSorted.Count == 0)
                {
                    financeviewmodel.errorMessage = "No User Notifications found";
#if WINUI
                    await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    });
#else
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
#endif
                    return;
                }

                foreach (UserNotification notif in notifsSorted)
                {
                    var toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                    if (toastBinding == null) continue;

                    var textElements = toastBinding.GetTextElements();
                    string titleText = textElements.FirstOrDefault()?.Text;
                    if (notificationTitle != titleText) continue;

#if WINUI
                    await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found Notification: " + titleText);
                    });
#else
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Found Notification: " + titleText);
#endif
                    string oneTimePasscode = "";
                    // I would think we would need a different storage
                    // for each of the different Banks because if we
                    // use financeviewmodel.oneTimePasscode to pass it
                    // back, then its going to get over-written isn't it?

                    // financeviewmodel.oneTimePasscode1001 = "";
                    // financeviewmodel.string oneTimePasscode1002 = "";
                    // financeviewmodel.string oneTimePasscode1003 = "";
                    // financeviewmodel.string oneTimePasscode1004 = "";
                    // financeviewmodel.string oneTimePasscode1005 = "";
                    // financeviewmodel.string oneTimePasscode1006 = "";
                    string bodyText = "";
                    switch (brandCode)
                    {
                        case 1001:  // Nationwide
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;

                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get first 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        case 1002:  // Santander
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));

                            if (!bodyText.Contains(notificationPrefix)) continue;

                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get last 8?
                            oneTimePasscode = bodyText.Substring(bodyText.Length - notificationTagLength, notificationTagLength);
                            break;
                        case 1003:  // Barclays
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;

                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get first 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        case 1004:  // Lloyds
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;
                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get last 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        case 10041:  // Bank of Scotland
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;
                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get last 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        case 10042:  // Halifax
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;
                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get last 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        case 1006:  // TSB
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;
                            // Find out where the prefix ends
                            int indexTSB = bodyText.IndexOf(notificationPrefix);
                            // Neither of these should never happen
                            if (indexTSB < 0 ||
                                 bodyText.Length <= notificationTagLength) continue;
                            indexTSB += notificationPrefix.Length;
                            // Make sure notificationPrefix ends with a space in BRANDS!!
                            // Extract important 6?
                            // Not sure why we need to bump this, but we do ... (in fact, I'm past caring!)
                            indexTSB++;
                            oneTimePasscode = bodyText.Substring(indexTSB, notificationTagLength);
                            break;
                        case 1007:  // NatWest
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
                            if (!bodyText.Contains(notificationPrefix)) continue;
                            // Find out where the prefix ends
                            int indexNatWest = bodyText.IndexOf(notificationPrefix);
                            // Neither of these should never happen
                            if (indexNatWest < 0 ||
                                 bodyText.Length <= notificationTagLength) continue;
                            indexNatWest += notificationPrefix.Length;
                            // Make sure notificationPrefix ends with a space in BRANDS!!
                            // Extract important 6?
                            // Not sure why we need to bump this, but we do ... (in fact, I'm past caring!)
                            indexNatWest++;
                            oneTimePasscode = bodyText.Substring(indexNatWest, notificationTagLength);
                            break;
                        case 1500:  // AJBell
                            bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));

                            if (!bodyText.Contains(notificationPrefix)) continue;
                            bodyText = bodyText.Replace(notificationPrefix, "").Trim();
                            if (bodyText.Length <= notificationTagLength) continue;
                            // Get last 6?
                            oneTimePasscode = bodyText.Substring(0, notificationTagLength);
                            break;
                        default:
                            break;
                    }
#if WINUI
                    await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OneTimePasscode: " + oneTimePasscode);
                    });
#else
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "OneTimePasscode: " + oneTimePasscode);
#endif
                    if (oneTimePasscode == "") return;

                    // Otherwise .. try and do a login ...

                    // === UI Thread Dispatcher handling ===
#if WINFORMS
                    await Task.Run(async () =>
#elif WPF
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
#elif WINUI
                    // DispatcherQueue dispatcher = DispatcherQueue.GetForCurrentThread(); 
                    await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
#endif
                    {
                        try
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Inserting: " + oneTimePasscode);

                            switch (brandCode)
                            {
                                case 1001:  // Nationwide
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 1002:  // Santander - notice how the Passcode ISN'T stored in financeviewmodel
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 1003:  // Barclays
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;                                    
                                case 1004:  // Lloyds
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 10041: // Bank Of Scotland
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 10042: // Halifax
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 1006: // TSB
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 1007:  // NatWest
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                case 1500:
                                    financeviewmodel.Passcode = oneTimePasscode;
                                    await FinanceWebView.ExecuteScriptAsync(@"
                                    (function () {
                                        window.chrome.webview.postMessage({ status: 'success_passcode', text: '" + oneTimePasscode + @"'});
                                    })();");
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Error in Notification handler: " + financeviewmodel.errorMessage);
                            await Task.CompletedTask;
                        }
                    });
                    break; // ✅ process only one notification
                }
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
#if WINUI
                await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
                {
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Error in Notification handler: " + financeviewmodel.errorMessage);

                });
#else
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Error in Notification handler: " + financeviewmodel.errorMessage);
               
#endif
                await Task.CompletedTask;
            }
#if WINUI
            await ourviewmodel.Dispatcher.EnqueueAsync(async () =>
            {
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Leaving Notification handler");
            });
#else
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Leaving Notification handler");
#endif
            return;
        }
#endif
    }
}