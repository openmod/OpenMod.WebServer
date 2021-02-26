using System;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Microsoft.Extensions.Logging;
using OpenMod.WebServer.Authentication;

namespace OpenMod.WebServer.Controllers
{
    public class TokenController : OpenModController
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenCodeService _codeService;

        public TokenController(
            ILogger<TokenController> logger,
            IServiceProvider serviceProvider,
            ITokenCodeService codeService) : base(serviceProvider)
        {
            _logger = logger;
            _codeService = codeService;
        }

        [Route(HttpVerbs.Post, "/")]
        public virtual async Task<string?> Get()
        {
            var input = await HttpContext.GetRequestDataAsync<CreateTokenInput>();
            if (string.IsNullOrEmpty(input.Code))
            {
                _logger.LogDebug("Input code was empty.");
                return null;
            }

            var token = await _codeService.CreateAuthTokenAsync(input.Code!, new TokenCreationParameters
            {
                Audience = input.Audience,
                ExpirationTime = input.ExpirationTime
            });

            if (token != null)
            {
                _codeService.DeleteCode(input.Code!);
            }
            else
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return "Invalid code";
            }

            return token?.Token;
        }
    }
}