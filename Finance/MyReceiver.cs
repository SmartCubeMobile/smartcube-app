
#if MAUI

using System;
using System.Collections.Generic;
using System.Linq;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
#endif

#if ANDROIDX
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Widget;
#endif
//using Microsoft.Playwright;

namespace SmartCubeMobile
{
#if ANDROID
    // https://learn.microsoft.com/en-us/dotnet/api/android.manifest.permission.receivesms?view=xamarin-android-sdk-12
    // https://stackoverflow.com/questions/61451407/creating-default-sms-app-in-xamarin-forms
    [Android.Content.BroadcastReceiver(Exported = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)Android.Content.IntentFilterPriority.HighPriority)]
    public class MyReceiverX : ISMSOTP
    {
        // Quit has no relevance here - Android is always full-screen
        public void MyReceiver()
        {
            return;
        }
    }

    public class MyReceiver : Android.Content.BroadcastReceiver
    {
        public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";
        protected string message, address = "";

        public override async void OnReceive(Android.Content.Context context, Android.Content.Intent intent)
        {
            MainMeter.financeviewmodel.onetimepasscode = string.Empty;
            if (intent.HasExtra("pdus"))
            {
                Java.Lang.Object[] smsArray = (Java.Lang.Object[])intent.Extras.Get("pdus");
                foreach (Java.Lang.Object item in smsArray)
                {
                    Android.Telephony.SmsMessage[] msgs = Android.Provider.Telephony.Sms.Intents.GetMessagesFromIntent(intent);
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
                        string onetimecode = nwmsgs.Last().MessageBody.Replace("Use one-time code ", string.Empty); // 779RCJ to log into the Internet Bank. Never share this code with anyone, only a fr…"
                        onetimecode = onetimecode.Substring(0, 6).Trim();
                        MainMeter.financeviewmodel.onetimepasscode = onetimecode;
#if !WINFORMS
                        await SmartRoutinesV2018.TextBlockUpdate(MainMeter.ourviewmodel, " Inserting: " + MainMeter.financeviewmodel.onetimepasscode);
                        //await SmartRoutinesV2018.Check_Trace(MainMeter.ourviewmodel, MainMeter.financeviewmodel.finance_token, 0, 0, "Inserting: " + MainMeter.financeviewmodel.onetimepasscode);
#endif
                        if (string.IsNullOrEmpty(MainMeter.financeviewmodel.onetimepasscode))
                        {
                            // Hardly likely to happen
                            //set_error_message("Notification content is empty");
                            return; // false
                        }
                        //NationwideV2023.set_smsotp(MainMeter.financeviewmodel.onetimepasscode);

                        /* Take this out!
                        //IElementHandle smsotp = await MainMeter.financeviewmodel.nwpage.QuerySelectorAsync("input[name='OneTimePasscode']");
                        //if (smsotp == null)
                        //{
                        //    // Hardly likely to happen, either
                        //    //set_error_message("Couldn't find the SMSOTP input");
                        //    return;// false;
                        //}
                        //await smsotp.FillAsync(MainMeter.financeviewmodel.onetimepasscode);
                        */

                        //MainPage.ShowNotification(smsotp);
                        //Toast.MakeText(context, smsotp, ToastLength.Short).Notify();
                    }
                }
            }

            return;
            //Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
        }

        public long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
                double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
                long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
                return utcAlarmTime; // milliseconds
            
        }
    }
    #endif
}
