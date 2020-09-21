using System.Collections.Generic;

namespace OpenMod.ApiServer.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}