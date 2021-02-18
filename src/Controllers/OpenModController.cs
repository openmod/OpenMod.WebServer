using System;
using Autofac;
using EmbedIO.WebApi;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.WebServer.Controllers
{
    public class OpenModController : WebApiController
    {
        public IServiceProvider ServiceProvider { get; }

        public OpenModController(IServiceProvider serviceProvider)
        {
            HttpContext.Items["scope"] = serviceProvider.GetRequiredService<ILifetimeScope>();
            HttpContext.Items["serviceProvider"] = serviceProvider;
            ServiceProvider = serviceProvider;
        }
    }
}