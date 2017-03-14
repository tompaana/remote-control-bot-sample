using RemoteControlBotSample.MessageRouting;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteControlBotSample.Notifications
{
    /// <summary>
    /// Simple class for sending notifications to users.
    /// </summary>
    public static partial class NotificationsManager
    {
        public async static Task SendNotificationAsync(Notification notification)
        {
            if (notification != null)
            {
                if (notification.PartiesToNotify == null || notification.PartiesToNotify.Count() == 0)
                {
                    // No receivers given - broadcast the notification to all users the bot is aware of
                    notification.PartiesToNotify.AddRange(MessageRouterManager.Instance.RoutingDataManager.GetUserParties());
                }

                if (string.IsNullOrEmpty(notification.Message))
                {
                    // Notification message missing - let's insert something for the sake of example
                    notification.Message = "Hello! This is a test notification message.";
                }

                foreach (Party partyToNotify in notification.PartiesToNotify)
                {
                    await MessageRouterManager.Instance.SendMessageToPartyByBotAsync(partyToNotify, notification.Message);
                }
            }
        }

        public async static Task SendNotificationAsync(string notificationAsJsonString)
        {
            Notification notification = Notification.FromJsonString(notificationAsJsonString);
            await SendNotificationAsync(notification);
        }
    }
}