using OpenMod.Core.Eventing;

namespace OpenMod.WebServer.Events
{
    public sealed class ApiServerConfiguredEvent : Event
    {
        public ApiServerConfiguredEvent(EmbedIO.WebServer server)
        {
            Server = server;
        }

        public EmbedIO.WebServer Server { get; }
    }
}