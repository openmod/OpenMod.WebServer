using System;
using System.Collections.Generic;
using System.Drawing;
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

    public class WebUser : IUser
    {
        private readonly IUser _baseUser;

        public WebUser(IUser baseUser)
        {
            _baseUser = baseUser;
        }

        public string Id => _baseUser.Id;
        public string Type => _baseUser.Type;
        public string DisplayName => _baseUser.DisplayName;

        public Task PrintMessageAsync(string message)
        {
            return _baseUser.PrintMessageAsync(message);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            return _baseUser.PrintMessageAsync(message, color);
        }

        public string FullActorName => _baseUser.FullActorName;

        public Task SavePersistentDataAsync<T>(string key, T? data)
        {
            return _baseUser.SavePersistentDataAsync(key, data);
        }

        public Task<T?> GetPersistentDataAsync<T>(string key)
        {
            return _baseUser.GetPersistentDataAsync<T>(key);
        }

        public IUserSession? Session => _baseUser.Session;

        public IUserProvider? Provider => _baseUser.Provider;
    }
}