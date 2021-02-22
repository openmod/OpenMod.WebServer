using System.Collections.Generic;

namespace OpenMod.WebServer.Dtos
{
    public class DetailedRoleDto : RoleDto
    {
        public ICollection<string> GrantedPermissions { get; set; } = null!;

        public ICollection<string> DeniedPermissions { get; set; } = null!;

        public ICollection<string> ParentRoles { get; set; } = null!;
    }
}