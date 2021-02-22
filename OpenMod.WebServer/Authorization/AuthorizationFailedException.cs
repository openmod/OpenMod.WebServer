using System;
using OpenMod.API;

namespace OpenMod.WebServer.Authorization
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(IOpenModComponent component, string permission) : base($"Missing permission: {component.OpenModComponentId}:{permission}")
        {
            
        }
    }
}