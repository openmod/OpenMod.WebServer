using EmbedIO.WebApi;
using OpenMod.API;
using OpenMod.ApiServer.Helpers;

namespace OpenMod.ApiServer.Extensions
{
    public static class WebApiModuleExtensions
    {
        public static void RegisterOpenModController<T>(this WebApiModule module, IOpenModComponent component) where T : WebApiController
        {
            module.RegisterController(ControllerFactory.CreateFor<T>(component));
        }
    }
}