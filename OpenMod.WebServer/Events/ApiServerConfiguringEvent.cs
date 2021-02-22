using OpenMod.Core.Eventing;

namespace OpenMod.WebServer.Events
{
    public sealed class ApiServerConfiguringEvent : Event
    {
        public ApiServerConfiguringEvent(EmbedIO.WebServer server)
        {
            Server = server;
        }

        public EmbedIO.WebServer Server { get; }
    }
}