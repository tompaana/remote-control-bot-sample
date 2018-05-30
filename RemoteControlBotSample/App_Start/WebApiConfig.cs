using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using Underscore.Bot.MessageRouting;
using Underscore.Bot.MessageRouting.DataStore.Local;

namespace RemoteControlBotSample
{
    public static class WebApiConfig
    {
        public static MessageRouterManager MessageRouterManager
        {
            get;
            private set;
        }

        public static void Register(HttpConfiguration config)
        {
            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            MessageRouterManager = new MessageRouterManager(new LocalRoutingDataManager());
        }
    }
}
