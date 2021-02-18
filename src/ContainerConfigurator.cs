using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;

namespace OpenMod.WebServer
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<WebServerDbContext>();
        }
    }
}