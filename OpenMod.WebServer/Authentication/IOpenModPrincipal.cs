using System.Security.Principal;
using OpenMod.API.Permissions;

namespace OpenMod.WebServer.Authentication
{
    public interface IOpenModPrincipal : IPrincipal
    {
        new OpenModIdentity Identity { get; }
    }
}