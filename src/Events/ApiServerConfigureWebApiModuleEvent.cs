using OpenMod.Core.Eventing;
using OpenMod.WebServer.Routing;

namespace OpenMod.WebServer.Events
{
    public sealed class ApiServerConfigureWebApiModuleEvent : Event
    {
        public EmbedIO.WebServer Server { get; }
        public OpenModWebApiModule WebApiModule { get; }

        public ApiServerConfigureWebApiModuleEvent(EmbedIO.WebServer server, OpenModWebApiModule webApiModule)
        {
            Server = server;
            WebApiModule = webApiModule;
        }
    }
}