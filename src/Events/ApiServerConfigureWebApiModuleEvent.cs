using EmbedIO;
using EmbedIO.WebApi;
using OpenMod.Core.Eventing;

namespace OpenMod.ApiServer.Events
{
    public sealed class ApiServerConfigureWebApiModuleEvent : Event
    {
        public WebServer Server { get; }
        public WebApiModule WebApiModule { get; }

        public ApiServerConfigureWebApiModuleEvent(WebServer server, WebApiModule webApiModule)
        {
            Server = server;
            WebApiModule = webApiModule;
        }
    }
}