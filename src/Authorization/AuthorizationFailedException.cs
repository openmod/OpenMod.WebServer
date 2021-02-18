using System;

namespace OpenMod.WebServer.Authorization
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string message) : base(message)
        {
            
        }
    }
}