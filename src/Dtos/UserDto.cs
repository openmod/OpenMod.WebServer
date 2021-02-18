using System.Collections.Generic;

namespace OpenMod.WebServer.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = null!;

        public ICollection<string> Roles { get; set; } = null!;
    }
}