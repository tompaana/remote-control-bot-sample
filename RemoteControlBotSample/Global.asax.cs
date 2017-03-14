using Autofac;
using System.Web;
using System.Web.Http;

namespace RemoteControlBotSample
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<GlobalMessageHandlerModule>();

#pragma warning disable 0618
            containerBuilder.Update(Microsoft.Bot.Builder.Dialogs.Conversation.Container);
#pragma warning restore 0618

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
