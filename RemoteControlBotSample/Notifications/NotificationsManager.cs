using Microsoft.Bot.Connector;
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
        public const string NotificationBackchannelId = "notification";

        /// <summary>
        /// Handles sending of the given notification.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns></returns>
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

        /// <summary>
        /// For convenience.
        /// Handles sending of the given notification.
        /// </summary>
        /// <param name="notificationAsJsonString">The notification as a string.</param>
        /// <returns></returns>
        public async static Task SendNotificationAsync(string notificationAsJsonString)
        {
            Notification notification = Notification.FromJsonString(notificationAsJsonString);
            await SendNotificationAsync(notification);
        }

        /// <summary>
        /// Checks the given activity for a notification related backchannel message.
        /// </summary>
        /// <param name="activity">The activity to check.</param>
        /// <param name="notificationData">Where the notification data (as string) is placed.
        /// Will be empty, if no backchannel message is detected.</param>
        /// <returns>True, if a backchannel message was detected. False otherwise.</returns>
        public static bool TryGetNotificationData(Activity activity, out string notificationData)
        {
            if (activity != null
                && !string.IsNullOrEmpty(activity.Text)
                && activity.Text.Equals(NotificationBackchannelId)
                && activity.ChannelData != null)
            {
                // Backchannel message with ID matching notifications action found
                notificationData = activity.ChannelData.ToString();
                return true;
            }

            notificationData = string.Empty;
            return false;
        }
    }
}