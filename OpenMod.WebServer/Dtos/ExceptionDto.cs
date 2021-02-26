namespace OpenMod.WebServer.Dtos
{
    public class ExceptionDto
    {
        public string Type { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string StackTrace { get; set; } = null!;
    }
}