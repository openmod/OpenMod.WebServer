using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using EmbedIO;
using EmbedIO.Files;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;
using OpenMod.Extensions.Games.Abstractions;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Controllers;
using OpenMod.WebServer.Events;
using OpenMod.WebServer.Logging;
using OpenMod.WebServer.Modules;
using SwanLogger = Swan.Logging.Logger;

[assembly: PluginMetadata("OpenMod.WebServer", Author = "OpenMod", DisplayName = "OpenMod WebServer", Website = "https://github.com/openmod/OpenMod.WebServer/")]
namespace OpenMod.WebServer
{
    [UsedImplicitly]
    public class WebServerPlugin : OpenModUniversalPlugin
    {
        private readonly IRuntime _runtime;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPermissionRegistry _permissionRegistry;
        private readonly ILogger<WebServerPlugin> _logger;
        private readonly IServiceProvider _serviceProvider;
        public EmbedIO.WebServer? Server { get; internal set; }

        public WebServerPlugin(IServiceProvider serviceProvider,
            IRuntime runtime,
            ILoggerFactory loggerFactory,
            IPermissionRegistry permissionRegistry,
            ILogger<WebServerPlugin> logger) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _runtime = runtime;
            _loggerFactory = loggerFactory;
            _permissionRegistry = permissionRegistry;
            _logger = logger;
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
        }

        protected override Task OnUnloadAsync()
        {
            Server?.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
