using System;
using Autofac;
using EmbedIO.WebApi;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;

namespace OpenMod.WebServer.Helpers
{
    internal static class ControllerFactory
    {
        public static Func<T> CreateFor<T>(IOpenModComponent component) where T : WebApiController
        {
            var serviceProvider = component.LifetimeScope.Resolve<IServiceProvider>();
            return () => ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}