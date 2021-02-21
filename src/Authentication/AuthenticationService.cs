using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using OpenMod.Core.Users;

namespace OpenMod.WebServer.Authentication
{
    [ServiceImplementation]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly WebServerDbContext _dbContext;
        private readonly IUserManager _userManager;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            WebServerDbContext dbContext,
            IUserManager userManager,
            ILogger<AuthenticationService> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AuthToken> CreateAuthTokenAsync(string ownerType, string ownerId, DateTime? expirationTime = null)
        {
            var token = new AuthToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpirationTime = expirationTime,
                OwnerId = ownerId,
                OwnerType = ownerType
            };

            await _dbContext.AuthTokens.AddAsync(token);

            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(token).State = EntityState.Detached;
            return token;
        }

        public async Task<bool> RevokeAuthTokenAsync(string token)
        {
            var matchedToken = await _dbContext.AuthTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Token == token);
            if (matchedToken == null)
            {
                // Unknown token
                return false;
            }

            matchedToken.ExpirationTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyCollection<AuthToken>> GetAuthTokensAsync(string userType, string userId)
        {
            return await _dbContext.AuthTokens
                .Where(d => d.OwnerId == userId && d.OwnerType == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IUser?> AuthenticateAsync(string token)
        {
            var matchedToken = await _dbContext.AuthTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Token == token);

            if (matchedToken == null || (matchedToken.ExpirationTime != null && matchedToken.ExpirationTime < DateTime.UtcNow))
            {
                return null;
            }

            var offlineUserProvider = (OfflineUserProvider?)_userManager.UserProviders
                .FirstOrDefault(d => d is OfflineUserProvider);

            if (offlineUserProvider == null)
            {
                _logger.LogError("Failed to find offline user provider, authentication will fail.");
                return null;
            }

            var offlineUser = await offlineUserProvider.FindUserAsync(matchedToken.OwnerType, matchedToken.OwnerId, UserSearchMode.FindById);
            if (offlineUser == null)
            {
                // User not found
                // Can happen if CreateToken was called for a user that doesn't exists
                return null;
            }

            return new WebUser(offlineUser);
        }
    }
}