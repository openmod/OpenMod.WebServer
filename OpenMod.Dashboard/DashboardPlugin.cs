using System;
using OpenMod.Core.Plugins;
using OpenMod.API.Plugins;

[assembly: PluginMetadata("OpenMod.Dashboard", DisplayName = "OpenMod Dashboard")]
namespace OpenMod.Dashboard
{
    public class DashboardPlugin : OpenModUniversalPlugin
    {
        public DashboardPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}
