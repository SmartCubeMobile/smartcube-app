//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SmartCubeMobileV2023
//{
//    class SmartSMSV2022
//    {
//    }
//}

//Class in Xamarin Forms Project (Android)

using Android.App;
using Android.Content;
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Android.Widget;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//#if __ANDROID
//using App.Models;
//using App.Services;
//#endif
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(SmartCubeMobileV2023.SMSHelper))]
namespace SmartCubeMobileV2023
{
    [BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SMSHelper : BroadcastReceiver, ISMSHelper
    {
        private const string Tag = "SMSBroadcastReceiver";
        private const string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

//#if __ANDROID
//        SMS sms = new SMS();
//#endif
        public SMSHelper()
        {
            /* https://blog.xamarin.com/cross-platform-messaging-for-ios-android-and-windows https://forums.xamarin.com/discussion/13682/broadcastreceiver-for-sms*/
        }

        public void Create(string sender, string receiver, string message)
        {
//#if __ANDROID
//            sms.Sender = sender;
//            sms.Receiver = receiver;
//            sms.Message = message;
//#endif
        }

        public void Send()
        {
//#if __ANDROID
//            if ((!sms.Receiver.Equals("")) && (!sms.Message.Equals("")))
//            {
//                var smsMessenger = CrossMessaging.Current.SmsMessenger;

//                if (smsMessenger.CanSendSmsInBackground)
//                {
//                    smsMessenger.SendSmsInBackground(sms.Receiver, sms.Message);
//                }
//            }
//#endif
        }

//#if __ANDROID
//        public SMS Get()
//        {
//            return sms;
//        }
//#endif

//#if __ANDROID
//        public List<SMS> ReadAll()
//        {
//            throw new System.NotImplementedException();
//        }
//#endif
    public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(Tag, "Intent received: " + intent.Action);


            if (intent.Action != IntentAction) return;

            SmsMessage[] messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);

            for (var i = 0; i < messages.Length; i++)
            {
//#if __ANDROID
//                sms.Sender = messages[i].OriginatingAddress;
//                sms.Message = messages[i].MessageBody;
//                sms.Timestamp = DateTime.Now;

//                Toast.MakeText(context, sms.Message, ToastLength.Long).Show();
//#endif
                Debug.WriteLine("SMS ER");
            }

            Toast.MakeText(context, "sdfghhj", ToastLength.Long).Show();
        }

        public void CheckForIncommingSMS()
        {
            Debug.WriteLine("HALLO");
//#if __ANDROID
//            Forms.Context.RegisterReceiver(this, new IntentFilter(IntentAction));
//#endif
        }

    }
}