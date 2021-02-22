using System;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.WebServer.Authentication;

namespace OpenMod.WebServer.Commands
{
    [Command("generatetoken")]
    [CommandParent(typeof(CommandOpenModAuth))]
    [CommandAlias("gt")]
    [CommandSyntax("[user type] [user id] [audience]")]
    [CommandDescription("Generates an auth token")]
    public class CommandOpenModAuthGenerateTokenCommand : Command
    {
        private readonly ICommandPermissionBuilder _permissionBuilder;
        private readonly IAuthenticationService _authenticationService;

        public CommandOpenModAuthGenerateTokenCommand(
            ICommandPermissionBuilder permissionBuilder,
            IServiceProvider serviceProvider,
            IAuthenticationService authenticationService) : base(serviceProvider)
        {
            _permissionBuilder = permissionBuilder;
            _authenticationService = authenticationService;
        }

        protected override async Task OnExecuteAsync()
        {
            var userType = Context.Actor.Type;
            var userId = Context.Actor.Id;
            string? audience = null;

            if (Context.Parameters.Length > 0)
            {
                if (Context.Parameters.Length == 1)
                {
                    throw new CommandWrongUsageException(Context);
                }

                if (await CheckPermissionAsync("others") != PermissionGrantResult.Grant)
                {
                    var permission = _permissionBuilder.GetPermission(Context.CommandRegistration!);
                    throw new NotEnoughPermissionException(Context, $"{permission}.others");
                }

                userType = Context.Parameters[0];
                userId = Context.Parameters[1];

                if (Context.Parameters.Length > 2)
                {
                    audience = Context.Parameters[2];
                }
            }

            var token = await _authenticationService.CreateAuthTokenAsync(userType, userId, new TokenCreationParameters
            {
                Audience = audience
            });

            await PrintAsync($"Token: {token.Token}");
        }
    }
}