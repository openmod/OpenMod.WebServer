using System;
using EmbedIO;
using EmbedIO.Utilities;

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