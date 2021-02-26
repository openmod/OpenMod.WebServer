using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using EmbedIO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Events;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Controllers;
using OpenMod.WebServer.Events;
using OpenMod.WebServer.Modules;
using Swan.Logging;
using Zio;
using Zio.FileSystems;

namespace OpenMod.WebServer
{
    public class InitializedListener : IEventListener<OpenModInitializedEvent>
    {
        private readonly ILogger<WebServerPlugin> _logger;
        private readonly IRuntime _runtime;
        private readonly IConfiguration _configuration;
        private readonly WebServerPlugin _plugin;
        private readonly IPluginActivator _pluginActivator;
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;

        public InitializedListener(
            ILogger<WebServerPlugin> logger,
            IRuntime runtime,
            IConfiguration configuration,
            WebServerPlugin plugin,
            IPluginActivator pluginActivator,
            IEventBus eventBus)
        {
            _logger = logger;
            _runtime = runtime;
            _configuration = configuration;
            _plugin = plugin;
            _pluginActivator = pluginActivator;
            _eventBus = eventBus;
            _serviceProvider = _plugin.LifetimeScope.Resolve<IServiceProvider>();
        }

        public async Task HandleEventAsync(object? sender, OpenModInitializedEvent _)
        {
            var staticFilesDirectory = Path.Combine(_plugin.WorkingDirectory, "www");
            if (!Directory.Exists(staticFilesDirectory))
            {
                Directory.CreateDirectory(staticFilesDirectory);
            }

            var url = _configuration["bind"];
            _plugin.Server = new EmbedIO.WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO));

            var server = _plugin.Server;
            server.OnHttpException += OnHttpException;
            server.OnUnhandledException = OnUnhandledException;

            var @event = (IEvent)new ApiServerConfiguringEvent(server);
            await _eventBus.EmitAsync(_plugin, this, @event);

            server
                .WithOpenModWebApi("/api/openmod/", _serviceProvider, module =>
                {
                    if (_runtime.HostInformation is IGameHostInformation)
                    {
                        module.RegisterController<PlayersController>(_plugin);
                    }

                    module.RegisterController<PluginsController>(_plugin);
                    module.RegisterController<SessionController>(_plugin);
                })
                .WithOpenModWebApi("/token", _serviceProvider, module =>
                {
                    module.RegisterController<TokenController>(_plugin);
                })
                .WithOpenModConsole("/sock/console", _serviceProvider)
                .WithOpenModFileSystem("/", _serviceProvider, RegisterPluginFiles);

            @event = new ApiServerConfiguredEvent(server);
            await _eventBus.EmitAsync(_plugin, this, @event);

            _plugin.Server.StateChanged += (s, e) => Logger.Info($"WebServer state - {e.NewState}");
            AsyncHelper.Schedule("API Server", () => _plugin.Server.RunAsync());
        }

        private void RegisterPluginFiles(OpenModFileSystemModule module)
        {
            foreach (var plugin in _pluginActivator.ActivatedPlugins)
            {
                var memoryFileSystem = new MemoryFileSystem();
                var workingDirectory = plugin.WorkingDirectory;
                var webDirectory = Path.Combine(workingDirectory, "web");

                if (!Directory.Exists(webDirectory))
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(webDirectory, "*", SearchOption.AllDirectories))
                {
                    var path = Path.GetFullPath(file).Replace(Path.GetFullPath(webDirectory), string.Empty);

                    var directory = Path.GetDirectoryName(path);
                    if (directory != null && !memoryFileSystem.DirectoryExists(directory))
                    {
                        memoryFileSystem.CreateDirectory(directory);
                    }

                    memoryFileSystem.WriteAllBytes(path, File.ReadAllBytes(file));
                }

                module.FileSystem.AddFileSystem(memoryFileSystem);
            }
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
                (int)HttpStatusCode.Forbidden,
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

    }
}