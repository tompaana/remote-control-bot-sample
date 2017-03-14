using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlBotSample.Notifications
{
    public class NotificationsScorable : ScorableBase<IActivity, string, double>
    {
        private const string NotificationBackchannelId = "notification";
        private readonly IDialogTask _dialogTask;

        public NotificationsScorable(IDialogTask dialogTask)
        {
            SetField.NotNull(out _dialogTask, nameof(dialogTask), dialogTask);
        }

        protected override Task DoneAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity activity, string state)
        {
            return (string.IsNullOrEmpty(state) ? 0d : 1d);
        }

        protected override bool HasScore(IActivity activity, string state)
        {
            return !string.IsNullOrEmpty(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="state">Should contain the notification as JSON string. We store it to the state in PrepareAsync method (see the return value).</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task PostAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            await NotificationsManager.SendNotificationAsync(state);
            await _dialogTask.PollAsync(cancellationToken);
        }

#pragma warning disable 1998
        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken cancellationToken)
        {
            if (activity is IMessageActivity messageActivity
                && !string.IsNullOrEmpty(messageActivity.Text)
                && messageActivity.Text.Equals(NotificationBackchannelId)
                && messageActivity.ChannelData != null)
            {
                // Backchannel message with ID matching notifications action found
                return (activity as IMessageActivity).ChannelData.ToString();
            }

            return string.Empty;
        }
#pragma warning restore 1998
    }
}