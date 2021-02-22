using System.Security.Principal;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    public class OpenModIdentity : IIdentity
    {
        public IUser User { get; }

        public OpenModIdentity(IUser user)
        {
            User = user;
        }

        public string AuthenticationType { get; } = "OpenMod";
        public bool IsAuthenticated { get; } = true;
        public string Name => User.DisplayName;
    }
}