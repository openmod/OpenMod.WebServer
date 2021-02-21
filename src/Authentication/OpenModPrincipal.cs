using System.Security.Principal;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    public class OpenModPrincipal : IOpenModPrincipal
    {
        public OpenModPrincipal(IUser user)
        {
            Identity = new OpenModIdentity(user);
        }

        public bool IsInRole(string role)
        {
            throw new System.NotImplementedException();
        }

        IIdentity IPrincipal.Identity => Identity;

        public OpenModIdentity Identity { get; }
    }
}