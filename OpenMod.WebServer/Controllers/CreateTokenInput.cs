using System;

namespace OpenMod.WebServer.Controllers
{
    [Serializable]
    public class CreateTokenInput
    {
        public string? Code { get; set; }

        public string? Audience { get; set; }

        public DateTime? ExpirationTime { get; set; }
    }
}