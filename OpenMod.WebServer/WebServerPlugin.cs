using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.WebServer.Controllers;
using OpenMod.WebServer.Logging;
using OpenMod.WebServer.Modules;
using Serilog;
using SwanLogger = Swan.Logging.Logger;

[assembly: PluginMetadata("OpenMod.WebServer", Author = "OpenMod", DisplayName = "OpenMod WebServer", Website = "https://github.com/openmod/OpenMod.WebServer/")]
namespace OpenMod.WebServer
{
    [UsedImplicitly]
    public class WebServerPlugin : OpenModUniversalPlugin
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPermissionRegistry _permissionRegistry;
        public EmbedIO.WebServer? Server { get; internal set; }

        public WebServerPlugin(IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IPermissionRegistry permissionRegistry) : base(serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _permissionRegistry = permissionRegistry;
        }

        protected override async Task OnLoadAsync()
        {
            await base.OnLoadAsync();

            RegisterPermissions();

            SwanLogger.NoLogging();
            SwanLogger.RegisterLogger(new SerilogLogger(_loggerFactory.CreateLogger("OpenMod.ApiServer.WebServer")));
        }

        public void RegisterPermissions()
        {
            _permissionRegistry.RegisterPermission(this, PlayersController.PermissionGetPlayer, "Allows to get information about a player");
            _permissionRegistry.RegisterPermission(this, PlayersController.PermissionGetAllPlayers, "Allows to get all connected players");
            _permissionRegistry.RegisterPermission(this, PluginsController.PermissionGetAllPlugins, "Allows to get all installed plugins");
            _permissionRegistry.RegisterPermission(this, OpenModConsoleModule.PermissionAccessConsole, "Allows to execute commands remotely");
            _permissionRegistry.RegisterPermission(this, OpenModConsoleModule.PermissionAccessLogs, "Allows to read all console logs");
        }

        protected override Task OnUnloadAsync()
        {
            Server?.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
