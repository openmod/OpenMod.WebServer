using System;

namespace OpenMod.ApiServer.Dtos
{
    public class PlayerDto : UserDto
    {
        public DateTime SessionStartTime { get; set; }
    }
}