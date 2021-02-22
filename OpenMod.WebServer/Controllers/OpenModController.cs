using System;
using EmbedIO.WebApi;
using OpenMod.API.Permissions;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Controllers
{
    public class OpenModController : WebApiController
    {
        public IServiceProvider ServiceProvider { get; }

        public IPermissionActor? Actor { get; internal set; }

        public OpenModController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}