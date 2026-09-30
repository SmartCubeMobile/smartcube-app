using System;
using System.Threading.Tasks;
using Windows.UI.Notifications.Management;
//using Windows.UI.Notifications.Management;

namespace SmartCubeMobile
{
    public class NotificationListener
    {
        internal static UserNotificationListener notification_listener;
        internal static UserNotificationListenerAccessStatus accessStatus;
        
        internal static UserNotificationListener GetCurrent()
        {
            return UserNotificationListener.Current;
        }

        internal static async Task<UserNotificationListenerAccessStatus> RequestAccess(UserNotificationListener listener)
        {
            return await listener.RequestAccessAsync();
        }
    }
}
