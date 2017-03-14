using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using RemoteControlBotSample.MessageRouting;
using RemoteControlBotSample.Notifications;

namespace RemoteControlBotSample
{
    public class GlobalMessageHandlerModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            base.Load(containerBuilder);

            containerBuilder
                .Register(componentContext => new MessageRouterScorable(componentContext.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            containerBuilder
                .Register(componentContext => new NotificationsScorable(componentContext.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}