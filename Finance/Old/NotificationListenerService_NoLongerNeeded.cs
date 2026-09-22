using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Service.Notification;
using Android.Service.Notification;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;


namespace SmartCubeMobile
{
    
    [Service]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class SmartNotificationListenerService : NotificationListenerService
    {
        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            base.OnNotificationPosted(sbn);

            // Check if the notification is from the SMS app
            string notificationText = sbn.Notification.Extras.GetString("android.text");

            if (!string.IsNullOrEmpty(notificationText) && notificationText.Contains("Your One-Time Code"))
            {
                // Process the SMS OTP here
                string otp = ExtractOtpFromMessage(notificationText);
                HandleOtpReceived(otp);
            }
        }

        public override void OnNotificationRemoved(StatusBarNotification sbn)
        {
            base.OnNotificationRemoved(sbn);
            // Handle notification removal if necessary
        }

        private string ExtractOtpFromMessage(string message)
        {
            // Example: extract the OTP from a typical message
            string otp = message.Substring(message.IndexOf("code is ") + 8, 6);
            return otp;
        }

        private void HandleOtpReceived(string otp)
        {
            // For example, update the UI or pass the OTP to other parts of your app
            // You might want to use a local broadcast or another form of communication to update the UI
            Intent intent = new Intent("com.smartcube.otp.RECEIVED");
            intent.PutExtra("otp", otp);
            SendBroadcast(intent);
        }
    }
}