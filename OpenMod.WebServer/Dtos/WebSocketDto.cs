namespace OpenMod.WebServer.Dtos
{
    public class WebSocketDto
    {
        public byte MessageId { get; set; }
        public byte[] Payload { get; set; } = null!;
    }
}