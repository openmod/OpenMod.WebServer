using System;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    [Service]
    public interface IAuthenticationService
    {
        Task<AuthToken> CreateAuthTokenAsync(string ownerType, string ownerId, DateTime? expirationTime);

        Task<bool> RevokeAuthTokenAsync(string token);

        Task<IUser?> AuthenticateAsync(string token);
    }
}