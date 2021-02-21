using System;
using EmbedIO.WebApi;

namespace OpenMod.WebServer.Controllers
{
    public class OpenModController : WebApiController
    {
        public IServiceProvider ServiceProvider { get; }

        public OpenModController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}