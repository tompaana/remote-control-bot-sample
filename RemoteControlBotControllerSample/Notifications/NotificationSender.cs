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
        private string _watermark;
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
        /// Gets the latest reply (Activity) from the bot.
        /// </summary>
        /// <returns>The latest Activity instance or null, if none available.</returns>
        public async Task<Activity> GetLatestReplyAsync()
        {
            ActivitySet activitySet = await GetActivitySetAsync();
            Activity activity = null;

            if (activitySet != null
                && activitySet.Activities != null
                && activitySet.Activities.Count > 0)
            {
#if DEBUG
                for (int i = 0; i < activitySet.Activities.Count; ++i)
                {
                    Activity a = activitySet.Activities[i];

                    if (a != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"GetLatestReplyAsync: Activity {(i + 1)}: Text: \"{a.Text}\" ChannelData: \"{a.ChannelData}\"");
                    }
                }
#endif
                activity = activitySet.Activities[activitySet.Activities.Count - 1];
            }

            return activity;
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

        /// <summary>
        /// Gets the latest activity set of the current conversation.
        /// </summary>
        /// <returns>The latest activity set or null, if no conversation or no activities since the last time we checked.</returns>
        private async Task<ActivitySet> GetActivitySetAsync()
        {
            ActivitySet activitySet = null;

            if (_conversation != null)
            {
                using (DirectLineClient directLineClient = new DirectLineClient(_botSecret))
                {
                    directLineClient.Conversations.ReconnectToConversation(_conversation.ConversationId);

                    if (string.IsNullOrEmpty(_watermark))
                    {
                        activitySet = await directLineClient.Conversations.GetActivitiesAsync(_conversation.ConversationId);
                    }
                    else
                    {
                        activitySet = await directLineClient.Conversations.GetActivitiesAsync(_conversation.ConversationId, _watermark);
                    }
                }

                if (activitySet != null)
                {
                    _watermark = activitySet.Watermark;
                }
            }

            return activitySet;
        }
    }
}
