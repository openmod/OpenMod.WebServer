using EmbedIO;
using OpenMod.Core.Eventing;

namespace OpenMod.ApiServer.Events
{
    public sealed class ApiServerConfigurationEvent : Event
    {
        public ApiServerConfigurationEvent(WebServer server)
        {
            Server = server;
        }

        public WebServer Server { get; }
    }
}