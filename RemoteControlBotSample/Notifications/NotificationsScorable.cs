using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlBotSample.Notifications
{
    /// <summary>
    /// A class that integrates to the Microsoft Bot Framework and looks for notification related
    /// backchannel messages from incoming activities.
    /// </summary>
    public class NotificationsScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask _dialogTask;

        public NotificationsScorable(IDialogTask dialogTask)
        {
            SetField.NotNull(out _dialogTask, nameof(dialogTask), dialogTask);
        }

        protected override Task DoneAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns the score (match of the activity to types this class handles).
        /// 1 means we are going to consume the activity. 0 score means we don't care about
        /// the current activity.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="state">Will contain the notification data, if the activity contained it.
        /// An empty string otherwise.</param>
        /// <returns></returns>
        protected override double GetScore(IActivity activity, string state)
        {
            return (string.IsNullOrEmpty(state) ? 0d : 1d);
        }

        /// <summary>
        /// Tells us if we this scorable has reached a score or not.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="state">Will contain the notification data, if the activity contained it.
        /// An empty string otherwise.</param>
        /// <returns></returns>
        protected override bool HasScore(IActivity activity, string state)
        {
            return !string.IsNullOrEmpty(state);
        }

        /// <summary>
        /// Sends the notification(s).
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="state">Should contain the notification as JSON string.
        /// We store it to the state in PrepareAsync method (see the return value).</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task PostAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            await NotificationsManager.SendNotificationAsync(state);
            await _dialogTask.PollAsync(cancellationToken);
        }

        /// <summary>
        /// Checks the given activity for a backchannel message ordering the bot to send out
        /// notifications.
        /// </summary>
        /// <param name="activity">The received activity.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The notification data as JSON string, if found. An empty string otherwise.</returns>
#pragma warning disable 1998
        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken cancellationToken)
        {
            string notificationData = string.Empty;
            NotificationsManager.TryGetNotificationData((activity as Activity), out notificationData);
            return notificationData;
        }
#pragma warning restore 1998
    }
}