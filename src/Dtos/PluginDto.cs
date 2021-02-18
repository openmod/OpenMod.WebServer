namespace OpenMod.WebServer.Dtos
{
    public class PluginDto
    {
        public string Id { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string? Author { get; set; }
        public string? Website { get; set; }
    }
}