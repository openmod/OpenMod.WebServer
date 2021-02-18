using System;
using EmbedIO;
using EmbedIO.Utilities;

namespace OpenMod.WebServer.Routing
{
    public static class OpenModWebApiModuleExtensions
    {
        public static TContainer WithOpenModWebApi<TContainer>(
            this TContainer @this,
            string baseRoute,
            Action<OpenModWebApiModule> configure)
            where TContainer : class, IWebModuleContainer
        {
            configure = Validate.NotNull(nameof(configure), configure);
            var module = new OpenModWebApiModule(baseRoute);
            return @this.WithModule(null, module, configure);
        }
    }
}