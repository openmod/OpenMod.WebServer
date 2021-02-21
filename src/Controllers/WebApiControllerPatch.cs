using Autofac;
using EmbedIO;
using EmbedIO.WebApi;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Permissions;

namespace OpenMod.WebServer.Controllers
{
    [HarmonyPatch(typeof(WebApiController))]
    [HarmonyPatch(MethodType.Setter)]
    [HarmonyPatch(nameof(WebApiController.HttpContext))]
    public static class WebApiControllerPatch
    {
        // This patch is needed to set the scope of the current request
        // The current scope is stored in the controller instance
        // EmbedIO does not provide APIs to do this properly without
        // writing a custom routing module, hence a patch is needed

        [HarmonyPrefix]
        public static void OnSet(object __instance, IHttpContext value)
        {
            if (__instance is not OpenModController controller)
            {
                return;
            }

            value.Items["scope"] = controller.ServiceProvider.GetRequiredService<ILifetimeScope>();

            if (value.Items.ContainsKey("actor"))
            {
                controller.Actor = value.Items["actor"] as IPermissionActor;
            }
        }
    }
}