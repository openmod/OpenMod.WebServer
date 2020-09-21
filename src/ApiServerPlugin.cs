using System;
using System.IO;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Files;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.ApiServer.Controllers;
using OpenMod.ApiServer.Events;
using OpenMod.ApiServer.Extensions;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;
using OpenMod.Extensions.Games.Abstractions;
using Swan.Logging;

[assembly: PluginMetadata("OpenMod.ApiServer", Author = "OpenMod", DisplayName = "OpenMod ApiServer", Website = "https://github.com/openmodplugins/OpenMod.ApiServer/")]
namespace OpenMod.ApiServer
{
    public class ApiServerPlugin : OpenModUniversalPlugin
    {
        private readonly IRuntime m_Runtime;
        private WebServer m_Server;

        public ApiServerPlugin(IServiceProvider serviceProvider, IRuntime runtime) : base(serviceProvider)
        {
            m_Runtime = runtime;
        }

        protected override async Task OnLoadAsync()
        {
            await base.OnLoadAsync();

            var staticFilesDirectory = Path.Combine(WorkingDirectory, "www");
            if (!Directory.Exists(staticFilesDirectory))
            {
                Directory.CreateDirectory(staticFilesDirectory);
            }

            var url = $"http://{Configuration["bind"]}:{Configuration["port"]}";
            m_Server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO));

            var @apiServerConfigurationEvent = new ApiServerConfigurationEvent(m_Server);
            await EventBus.EmitAsync(this, this, @apiServerConfigurationEvent);

            m_Server.WithWebApi("/api", module =>
                {
                    if (m_Runtime.HostInformation is IGameHostInformation)
                    {
                        module.RegisterOpenModController<PlayersController>(this);
                    }

                    var @event = new ApiServerConfigureWebApiModuleEvent(m_Server, module);
                    AsyncHelper.RunSync(() => EventBus.EmitAsync(this, this, @event));
                })
                .WithStaticFolder("/", staticFilesDirectory, true, m =>
                {
                    m.WithContentCaching(true);
                });

            m_Server.StateChanged += (s, e) => $"WebServer state - {e.NewState}".Info();

            AsyncHelper.Schedule("API Server", () => m_Server.RunAsync());
        }

        protected override Task OnUnloadAsync()
        {
            m_Server.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
