using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Core.Commands.OpenModCommands;
using OpenMod.Core.Helpers;
using OpenMod.WebServer.Authentication;

namespace OpenMod.WebServer.Commands
{
    [Command("webauth")]
    [CommandParent(typeof(CommandOpenMod))]
    [CommandSyntax("[generatetoken]")]
    [CommandDescription("Gets auth codes")]
    public class CommandOpenModAuth : Command
    {
        private readonly ITokenCodeService _codeService;

        public CommandOpenModAuth(
            IServiceProvider serviceProvider, 
            ITokenCodeService codeService) : base(serviceProvider)
        {
            _codeService = codeService;
        }

        protected override async Task OnExecuteAsync()
        {
            var code = _codeService.GenerateCode(Context.Actor);
            await PrintAsync($"Your authentication code is: {code}", Color.DarkGreen);
            await PrintAsync("Your code will expire in 60 seconds.", Color.Green);
            await PrintAsync("DO NOT SHARE YOUR CODE WITH ANYONE.", Color.Red);

            AsyncHelper.Schedule("Auth code expiration", async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                if (_codeService.DeleteCode(code))
                {
                    // code was not used
                    await PrintAsync("Your code has expired.", Color.Red);
                }
            });
        }
    }
}