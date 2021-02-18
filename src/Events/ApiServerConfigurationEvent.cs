using EmbedIO;
using OpenMod.Core.Eventing;

namespace OpenMod.WebServer.Events
{
    public sealed class ApiServerConfigurationEvent : Event
    {
        public ApiServerConfigurationEvent(EmbedIO.WebServer server)
        {
            Server = server;
        }

        public EmbedIO.WebServer Server { get; }
    }
}