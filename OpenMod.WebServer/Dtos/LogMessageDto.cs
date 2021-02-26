using Microsoft.Extensions.Logging;

namespace OpenMod.WebServer.Dtos
{
    public class LogMessageDto
    {
        public LogLevel LogLevel { get; set; }

        public string Message { get; set; } = null!;

        public ExceptionDto? Exception { get; set; } = null!;
    }
}