using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    [Service]
    public interface IAuthenticationService
    {
        Task<AuthToken> CreateAuthTokenAsync(string ownerType, string ownerId, DateTime? expirationTime);

        Task<bool> RevokeAuthTokenAsync(string token);

        Task<IReadOnlyCollection<AuthToken>> GetAuthTokensAsync(string userType, string userId);

        Task<IUser?> AuthenticateAsync(string token);
    }
}