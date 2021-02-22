using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using OpenMod.Core.Plugins;
using OpenMod.Core.Users;

namespace OpenMod.WebServer.Authentication
{
    [ServiceImplementation]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IRuntime _runtime;
        private readonly byte[] _secret;

        public AuthenticationService(
            IUserManager userManager,
            ILogger<AuthenticationService> logger,
            IRuntime runtime)
        {
            _userManager = userManager;
            _logger = logger;
            _runtime = runtime;
            _secret = ReadSecret();
        }

        private byte[] ReadSecret()
        {
            var workingDirectory = PluginHelper.GetWorkingDirectory(_runtime, "OpenMod.WebServer");
            var file = Path.Combine(workingDirectory, "secret.dat");

            if (!File.Exists(file))
            {
                var secret = GenerateSecret();
                File.WriteAllText(file, Convert.ToBase64String(secret));
                return secret;
            }

            return Convert.FromBase64String(File.ReadAllText(file));
        }

        private byte[] GenerateSecret()
        {
            var buffer = new byte[64];
            new Random().NextBytes(buffer);
            return buffer;
        }

        public async Task<AuthToken> CreateAuthTokenAsync(string userType, string userId, TokenCreationParameters parameters)
        {
            var user = await _userManager.FindUserAsync(userType, userId, UserSearchMode.FindById);

            var builder = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .AddClaim("sub", userId)
                .AddClaim("ut", userType)
                .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .AddClaim("preferred_username", user?.DisplayName ?? userId);

            if (parameters.Audience != null)
            {
                builder.AddClaim("aud", parameters.Audience);
            }

            if (parameters.ExpirationTime != null)
            {
                builder.AddClaim("exp", parameters.ExpirationTime.Value.ToUnixTimeSeconds());
            }

            var token = builder.Encode();

            return new AuthToken
            {
                Token = token,
                OwnerId = userId,
                OwnerType = userType
            };
        }

        public async Task<IUser?> AuthenticateAsync(string token)
        {
            Dictionary<string, object> decodedToken;

            try
            {
                decodedToken = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(_secret)
                    .MustVerifySignature()
                    .Decode<Dictionary<string, object>>(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token validation has failed", ex);
                return null;
            }

            var offlineUserProvider = (OfflineUserProvider?)_userManager.UserProviders
                .FirstOrDefault(d => d is OfflineUserProvider);

            if (offlineUserProvider == null)
            {
                _logger.LogWarning("Failed to find offline user provider.");
            }


            var userId = (string)decodedToken["sub"];
            var userType = (string)decodedToken["ut"];

            IUser? offlineUser = null;

            if (offlineUserProvider != null)
            {
                offlineUser = await offlineUserProvider.FindUserAsync(userType, userId, UserSearchMode.FindById);
            }

            if (offlineUser == null)
            {
                // User not found
                // Can happen if CreateToken was called for a user that doesn't exists
                return new WebUser(userType, userId);
            }

            return new WebUser(offlineUser);
        }
    }
}