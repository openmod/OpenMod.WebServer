using System.Collections.Generic;

namespace OpenMod.ApiServer.Dtos
{
    public class RoleDto
    {
        public string Id { get; set; }

        public int Priority { get; set; }

        public string DisplayName { get; set; }

        public bool IsAutoAssigned { get; set; }

        public ICollection<string> GrantedPermissions { get; set; }
        
        public ICollection<string> DeniedPermissions { get; set; }

        public ICollection<string> ParentRoles { get; set; }
    }
}