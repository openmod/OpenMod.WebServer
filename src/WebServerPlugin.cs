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
        private EmbedIO.WebServer? _server;

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

            var staticFilesDirectory = Path.Combine(WorkingDirectory, "www");
            if (!Directory.Exists(staticFilesDirectory))
            {
                Directory.CreateDirectory(staticFilesDirectory);
            }

            var url = Configuration["bind"];
            _server = new EmbedIO.WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO));

            _server.OnHttpException += OnHttpException;
            _server.OnUnhandledException = OnUnhandledException;

            var @event = (IEvent) new ApiServerConfiguringEvent(_server);
            await EventBus.EmitAsync(this, this, @event);

            _server
                .WithOpenModWebApi("/api/openmod/", _serviceProvider, module =>
                {
                    if (_runtime.HostInformation is IGameHostInformation)
                    {
                        module.RegisterController<PlayersController>(this);
                    }

                    module.RegisterController<PluginsController>(this);
                    module.RegisterController<SessionController>(this);
                })
                .WithOpenModWebApi("/token", _serviceProvider, module =>
                {
                    module.RegisterController<TokenController>(this);
                })
                .WithStaticFolder("/static", staticFilesDirectory, true, m =>
                {
                    m.WithContentCaching();
                });

            @event = new ApiServerConfigurationEvent(_server);
            await EventBus.EmitAsync(this, this, @event);

            _server.StateChanged += (s, e) => SwanLogger.Info($"WebServer state - {e.NewState}");
            AsyncHelper.Schedule("API Server", () => _server.RunAsync());
        }

        private Task OnUnhandledException(IHttpContext context, Exception exception)
        {
            if (exception is AuthorizationFailedException authorizationFailedException)
            {
                return HandleAuthorizationException(context, authorizationFailedException);
            }

            if (exception is not HttpException and not HttpException)
            {
                _logger.LogError(exception, "Exception occured:");
            }
            else
            {
                _logger.LogDebug(exception, "Exception occured:");
            }

            return Task.CompletedTask;
        }

        private Task HandleAuthorizationException(IHttpContext context, AuthorizationFailedException exception)
        {
            return context.SendStandardHtmlAsync(
                (int) HttpStatusCode.Forbidden,
                text =>
                {
                    text.Write("<p>Access to this page has been denied.</p>");
                    text.Write(
                        "<p><strong>Exception type:</strong> {0}<p><strong>Message:</strong> {1}",
                        HttpUtility.HtmlEncode(exception.GetType().FullName ?? "<unknown>"),
                        HttpUtility.HtmlEncode(exception.Message));
                });
        }

        private Task OnHttpException(IHttpContext context, IHttpException httpException)
        {
            if (httpException is Exception ex)
            {
                return OnUnhandledException(context, ex);
            }

            _logger.LogDebug("HTTP Exception occured: {0}", httpException);
            return Task.CompletedTask;
        }

        public void RegisterPermissions()
        {
            _permissionRegistry.RegisterPermission(this, PlayersController.PermissionGetPlayer, "Allows to get information about a player");
            _permissionRegistry.RegisterPermission(this, PlayersController.PermissionGetAllPlayers, "Allows to get all connected players");
            _permissionRegistry.RegisterPermission(this, PluginsController.PermissionGetAllPlugins, "Allows to get all installed plugins");
        }

        protected override Task OnUnloadAsync()
        {
            _server?.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
