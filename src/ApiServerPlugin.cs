using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Files;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.ApiServer.Controllers;
using OpenMod.ApiServer.Events;
using OpenMod.ApiServer.Extensions;
using OpenMod.ApiServer.Logging;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;
using OpenMod.Extensions.Games.Abstractions;
using SwanLogger = Swan.Logging.Logger;

[assembly: PluginMetadata("OpenMod.ApiServer", Author = "OpenMod", DisplayName = "OpenMod ApiServer", Website = "https://github.com/openmodplugins/OpenMod.ApiServer/")]
namespace OpenMod.ApiServer
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public class ApiServerPlugin : OpenModUniversalPlugin
    {
        private readonly IRuntime _runtime;
        private readonly ILoggerFactory _loggerFactory;
        private WebServer _server;

        public ApiServerPlugin(IServiceProvider serviceProvider,
            IRuntime runtime,
            ILoggerFactory loggerFactory) : base(serviceProvider)
        {
            _runtime = runtime;
            _loggerFactory = loggerFactory;
        }

        protected override async Task OnLoadAsync()
        {
            await base.OnLoadAsync();
            SwanLogger.NoLogging();
            SwanLogger.RegisterLogger(new SerilogLogger(_loggerFactory.CreateLogger("OpenMod.ApiServer.WebServer")));

            var staticFilesDirectory = Path.Combine(WorkingDirectory, "www");
            if (!Directory.Exists(staticFilesDirectory))
            {
                Directory.CreateDirectory(staticFilesDirectory);
            }

            var url = $"http://{Configuration["bind"]}:{Configuration["port"]}";
            _server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO));

            var @apiServerConfigurationEvent = new ApiServerConfigurationEvent(_server);
            await EventBus.EmitAsync(this, this, @apiServerConfigurationEvent);

            _server.WithWebApi("/api", module =>
                {
                    if (_runtime.HostInformation is IGameHostInformation)
                    {
                        module.RegisterOpenModController<PlayersController>(this);
                    }

                    module.RegisterOpenModController<SessionController>(this);

                    var @event = new ApiServerConfigureWebApiModuleEvent(_server, module);
                    AsyncHelper.RunSync(() => EventBus.EmitAsync(this, this, @event));
                })
                .WithStaticFolder("/", staticFilesDirectory, true, m =>
                {
                    m.WithContentCaching(true);
                });

            _server.StateChanged += (s, e) => SwanLogger.Info($"WebServer state - {e.NewState}");

            AsyncHelper.Schedule("API Server", () => _server.RunAsync());
        }

        protected override Task OnUnloadAsync()
        {
            _server.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
