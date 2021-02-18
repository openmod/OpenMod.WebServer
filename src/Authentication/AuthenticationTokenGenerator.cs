using System;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.WebServer.Authentication
{
    [ServiceImplementation]
    public class AuthenticationTokenGenerator : IAuthenticationTokenGenerator
    {
        public Task<string> GenerateTokenAsync(string userType, string userId)
        {
            // todo: use JWT
            return Task.FromResult("usertype-" + Guid.NewGuid());
        }
    }
}