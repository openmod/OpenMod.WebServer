using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.WebServer.Logging;

namespace OpenMod.WebServer
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ILoggerFactory, LoggerFactoryProxy>();
            // serviceCollection.AddTransient(typeof(ILogger<>), typeof(LoggerProxy<>));
        }
    }
}