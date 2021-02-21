using Autofac;
using EmbedIO;
using EmbedIO.WebApi;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;

namespace OpenMod.WebServer.Controllers
{
    [HarmonyPatch(typeof(WebApiController))]
    [HarmonyPatch(MethodType.Setter)]
    [HarmonyPatch(nameof(WebApiController.HttpContext))]
    public static class WebApiControllerPatch
    {
        [HarmonyPrefix]
        public static void OnSet(object __instance, IHttpContext value)
        {
            if (__instance is not OpenModController controller)
            {
                return;
            }

            value.Items["scope"] = controller.ServiceProvider.GetRequiredService<ILifetimeScope>();
        }
    }
}