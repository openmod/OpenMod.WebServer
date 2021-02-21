using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Utilities;
using JetBrains.Annotations;

namespace OpenMod.WebServer.Modules
{
    public static class OpenModModuleExtensions
    {
        public static TContainer WithOpenModWebApi<TContainer>(
            this TContainer @this,
            string baseRoute,
            IServiceProvider serviceProvider,
            Action<OpenModWebApiModule> configure)
            where TContainer : class, IWebModuleContainer
        {
            configure = Validate.NotNull(nameof(configure), configure);
            var module = new OpenModWebApiModule(baseRoute, serviceProvider);
            return @this.WithModule(null, module, configure);
        }
    }
}