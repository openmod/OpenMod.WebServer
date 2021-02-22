using System;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    [Service]
    public interface IAuthenticationService
    {
        Task<AuthToken> CreateAuthTokenAsync(string userType, string userId, TokenCreationParameters parameters);

        Task<IUser?> AuthenticateAsync(string token);
    }
}