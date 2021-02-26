using System;
using Autofac;
using EmbedIO;
using EmbedIO.Utilities;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Core.Ioc;

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

            var lifetime = serviceProvider.GetRequiredService<ILifetimeScope>();
            var module = ActivatorUtilitiesEx.CreateInstance<OpenModWebApiModule>(lifetime, baseRoute);
            return @this.WithModule(module, configure);
        }

        public static TContainer WithOpenModFileSystem<TContainer>(
            this TContainer @this,
            string baseRoute,
            IServiceProvider serviceProvider,
            Action<OpenModFileSystemModule> configure)
            where TContainer : class, IWebModuleContainer
        {
            configure = Validate.NotNull(nameof(configure), configure);

            var lifetime = serviceProvider.GetRequiredService<ILifetimeScope>();
            var module = ActivatorUtilitiesEx.CreateInstance<OpenModFileSystemModule>(lifetime, baseRoute);
            return @this.WithModule(module, configure);
        }

        public static TContainer WithOpenModConsole<TContainer>(
            this TContainer @this,
            string baseRoute,
            IServiceProvider serviceProvider)
            where TContainer : class, IWebModuleContainer
        {
            var lifetime = serviceProvider.GetRequiredService<ILifetimeScope>();
            var module = ActivatorUtilitiesEx.CreateInstance<OpenModConsoleModule>(lifetime, baseRoute);
            return @this.WithModule(module);
        }
    }
}