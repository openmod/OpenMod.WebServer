using EmbedIO.WebApi;
using OpenMod.API;
using OpenMod.WebServer.Controllers;
using OpenMod.WebServer.Helpers;

namespace OpenMod.WebServer.Extensions
{
    public static class WebApiModuleExtensions
    {
        public static void RegisterOpenModController<T>(this WebApiModule module, IOpenModComponent component) where T : OpenModController
        {
            module.RegisterController(ControllerFactory.CreateFor<T>(component));
        }
    }
}