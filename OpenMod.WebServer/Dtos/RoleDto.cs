namespace OpenMod.WebServer.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;

        public int Priority { get; set; }

        public string DisplayName { get; set; } = null!;

        public bool IsAutoAssigned { get; set; }
    }
}