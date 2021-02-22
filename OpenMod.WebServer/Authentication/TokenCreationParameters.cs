using System;

namespace OpenMod.WebServer.Authentication
{
    public struct TokenCreationParameters
    {
        public string? Audience { get; set; }

        public DateTimeOffset? ExpirationTime { get; set; }
    }
}