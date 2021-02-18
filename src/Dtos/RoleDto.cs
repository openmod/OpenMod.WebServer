using System.Collections.Generic;

namespace OpenMod.WebServer.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;

        public int Priority { get; set; }

        public string DisplayName { get; set; } = null!;

        public bool IsAutoAssigned { get; set; }

        public ICollection<string> GrantedPermissions { get; set; } = null!;

        public ICollection<string> DeniedPermissions { get; set; } = null!;

        public ICollection<string> ParentRoles { get; set; } = null!;
    }
}