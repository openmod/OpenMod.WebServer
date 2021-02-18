using System;

namespace OpenMod.WebServer.Authorization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        public string Permission { get; }

        public AuthorizeAttribute(string permission)
        {
            Permission = permission;
        }    
    }
}