using System;

namespace OpenMod.WebServer.Dtos
{
    public class PlayerDto : UserDto
    {
        public DateTime? SessionStartTime { get; set; }
    }
}