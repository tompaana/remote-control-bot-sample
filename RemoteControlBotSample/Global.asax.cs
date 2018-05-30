using Autofac;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace RemoteControlBotSample
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                    // Notifications
                    builder.RegisterModule<GlobalMessageHandlerModule>();

                    // Bot Storage: register state storage for your bot
                    IBotDataStore<BotData> botDataStore = null;

                    // Default store: volatile in-memory store - Only for prototyping!
                    System.Diagnostics.Debug.WriteLine("WARNING!!! Using InMemoryDataStore, which should be only used for prototyping, for the bot state!");
                    botDataStore = new InMemoryDataStore();

                    builder.Register(c => botDataStore)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();
                });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
