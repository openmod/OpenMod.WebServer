using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;

namespace OpenMod.WebServer.Authentication
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class TokenCodeService : ITokenCodeService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly Dictionary<string, IPermissionActor> _codes = new();
        private readonly Random _random = new();

        public TokenCodeService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public string GenerateCode(IPermissionActor actor)
        {
            foreach(var kv in _codes)
            {
                if (kv.Value.Id == actor.Id && string.Equals(kv.Value.Type, actor.Type, StringComparison.OrdinalIgnoreCase))
                {
                    return kv.Key;
                }
            }

            string code;
            if (_codes.Count >= 890000)
            {
                // very unlikely to happen because auth codes only exist for 60 seconds
                _codes.Clear();
            }

            // ensure code doesn't exist already
            do { code = _random.Next(100000, 1000000).ToString(); }
            while (_codes.ContainsKey(code));

            _codes.Add(code, actor);
            return code;
        }

        public Task<AuthToken?> CreateAuthTokenAsync(string code, TokenCreationParameters parameters)
        {
            if (!_codes.ContainsKey(code))
            {
                return Task.FromResult<AuthToken?>(null);
            }

            var actor = _codes[code];
            return _authenticationService.CreateAuthTokenAsync(actor.Type, actor.Id, parameters)!;
        }

        public bool DeleteCode(string inputCode)
        {
            return _codes.Remove(inputCode);
        }
    }
}