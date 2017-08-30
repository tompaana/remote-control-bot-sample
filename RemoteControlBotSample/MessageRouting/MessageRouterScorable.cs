using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlBotSample.MessageRouting
{
    public class MessageRouterScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask _dialogTask;

        public MessageRouterScorable(IDialogTask dialogTask)
        {
            SetField.NotNull(out _dialogTask, nameof(dialogTask), dialogTask);
        }

        protected override Task DoneAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity activity, string state)
        {
            return 0d;
        }

        protected override bool HasScore(IActivity activity, string state)
        {
            return false;
        }

        protected override async Task PostAsync(IActivity activity, string state, CancellationToken cancellationToken)
        {
            await _dialogTask.PollAsync(cancellationToken);
        }

#pragma warning disable 1998
        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken cancellationToken)
        {
            WebApiConfig.MessageRouterManager.MakeSurePartiesAreTracked(activity);
            return string.Empty;
        }
#pragma warning restore 1998
    }
}