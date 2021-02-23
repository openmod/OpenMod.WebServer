using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.Core.Plugins;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;

[assembly: PluginMetadata("OpenMod.Dashboard", DisplayName = "OpenMod Dashboard")]
namespace OpenMod.Dashboard
{
    public class DashboardPlugin : OpenModUniversalPlugin
    {
        private readonly IConfiguration _configuration;

        public DashboardPlugin(
            IConfiguration configuration,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = configuration;
        }

        protected override Task OnLoadAsync()
        {
            if (_configuration.GetSection("updateFiles").Get<bool?>() ?? true)
            {
                AssemblyHelper.CopyAssemblyResources(GetType().Assembly, WorkingDirectory, true);
            }

            return Task.CompletedTask;
        }
    }
}
