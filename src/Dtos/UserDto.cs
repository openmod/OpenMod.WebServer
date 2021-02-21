using System;
using System.Collections.Generic;

namespace OpenMod.WebServer.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime? SessionStartTime { get; set; }

        public ICollection<RoleDto> Roles { get; set; } = null!;
    }
}