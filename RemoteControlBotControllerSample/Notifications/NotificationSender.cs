using Microsoft.Bot.Connector.DirectLine;
using RemoteControlBotSample.MessageRouting;
using RemoteControlBotSample.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteControlBotControllerSample.Notifications
{
    public class NotificationSender
    {
        private const string SenderId = "RemoteControlBotControllerSample";
        private const string NotificationsBackchannelId = "notification";
        private const string ActivityTypeMessage = "message";
        private Conversation _conversation;
        private string _botSecret;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="botSecret">The bot secret (Direct Line).</param>
        public NotificationSender(string botSecret)
        {
            _botSecret = botSecret;
        }

        /// <summary>
        /// Sends a notification to the given users with the given message.
        /// </summary>
        /// <param name="partiesToNotify">The list of users to notify.</param>
        /// <param name="message">The notification message.</param>
        /// <returns></returns>
        public async Task<ResourceResponse> NotifyAsync(IList<Party> partiesToNotify, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException($"The message ({nameof(message)}) is null or empty");
            }

            Notification notification = new Notification()
            {
                Message = message
            };

            if (partiesToNotify != null && partiesToNotify.Count > 0)
            {
                notification.PartiesToNotify.AddRange(partiesToNotify);
            }

            return await PostActivityAsync(CreateNotificationActivity(notification, NotificationsBackchannelId));
        }

        /// <summary>
        /// Creates an activity containing the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="backchannelId">The backchannel ID.</param>
        /// <returns>The newly created activity.</returns>
        private Activity CreateNotificationActivity(Notification notification, string backchannelId)
        {
            return new Activity()
            {
                Type = ActivityTypeMessage,
                From = new ChannelAccount(SenderId),
                Text = backchannelId,
                ChannelData = notification.ToJsonString()
            };
        }

        /// <summary>
        /// Posts the given activity to the bot using Direct Line client.
        /// </summary>
        /// <param name="activity">The activity to send.</param>
        /// <returns>The resoure response.</returns>
        private async Task<ResourceResponse> PostActivityAsync(Activity activity)
        {
            ResourceResponse resourceResponse = null;

            using (DirectLineClient directLineClient = new DirectLineClient(_botSecret))
            {
                if (_conversation == null)
                {
                    _conversation = directLineClient.Conversations.StartConversation();
                }
                else
                {
                    directLineClient.Conversations.ReconnectToConversation(_conversation.ConversationId);
                }

                resourceResponse = await directLineClient.Conversations.PostActivityAsync(_conversation.ConversationId, activity);
            }

            return resourceResponse;
        }
    }
}
