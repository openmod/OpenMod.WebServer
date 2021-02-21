using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Permissions;
using OpenMod.Core.Permissions.Data;
using OpenMod.WebServer.Authentication;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Controllers;

namespace OpenMod.WebServer.Modules
{
    public class OpenModWebApiModule : WebApiModuleBase
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenModWebApiModule(string baseRoute, IServiceProvider serviceProvider) : base(baseRoute)
        {
            _serviceProvider = serviceProvider;
        }


        protected override async Task OnRequestAsync(IHttpContext context)
        {
            try
            {
                await base.OnRequestAsync(context);
            }
            finally
            {
                if (context.Items.ContainsKey("scope"))
                {
                    await context.Items["scope"].DisposeSyncOrAsync();
                }
            }
        }

        public void RegisterController<TController>(IOpenModComponent component)
            where TController : OpenModController
        {
            AddAuthorizationHandler(typeof(TController), component);
            RegisterController(component, () => (TController)CreateComponentFactory(typeof(TController), component));
        }

        public void RegisterController<TController>(IOpenModComponent component, Func<TController> factory)
            where TController : OpenModController
        {
            RegisterControllerType(typeof(TController), factory);
        }

        public void RegisterController(Type controllerType, IOpenModComponent component)
        {
            RegisterController(controllerType, component, () => CreateComponentFactory(controllerType, component));
        }

        public void RegisterController(Type controllerType, IOpenModComponent component, Func<WebApiController> factory)
        {
            AddAuthorizationHandler(controllerType, component);
            RegisterControllerType(controllerType, factory);
        }

        private OpenModController CreateComponentFactory(Type type, IOpenModComponent component)
        {
            var scope = component.LifetimeScope.BeginLifetimeScope("AutofacWebRequest");
            return (OpenModController)ActivatorUtilitiesEx.CreateInstance(scope, type);
        }

        private void AddAuthorizationHandler(Type controllerType, IOpenModComponent component)
        {
            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => !m.ContainsGenericParameters);
            
            foreach (var method in methods)
            {
                var routeAttributes = method.GetCustomAttributes<RouteAttribute>()
                    .ToArray();

                if (routeAttributes.Length < 1)
                {
                    continue;
                }

                var authorizationAttributes = method.GetCustomAttributes<AuthorizeAttribute>()
                    .ToArray();

                if (authorizationAttributes.Length < 1)
                {
                    continue;
                }

                foreach (var attribute in routeAttributes)
                {
                    AddHandler(attribute.Verb, attribute.Matcher, async (context, route) =>
                    {
                        var scope = component.LifetimeScope;
                        var permissionChecker = scope.Resolve<IPermissionChecker>();
                        var authenticationToken = context.Request.Headers["Authentication"];
                        var authenticationService = _serviceProvider.GetRequiredService<IAuthenticationService>();

                        IPermissionActor? actor = null;
                        if (!string.IsNullOrEmpty(authenticationToken))
                        {
                            actor = await authenticationService.AuthenticateAsync(authenticationToken);
                        }

                        if (actor == null)
                        {
                            var roleStore = scope.Resolve<IPermissionRoleStore>();
                            actor = await roleStore.GetRoleAsync("anonymous")
                                       ?? new PermissionRole(new PermissionRoleData
                                       {
                                           Id = "anonymous",
                                           DisplayName = "Anonymous",
                                           Data = new Dictionary<string, object?>(),
                                           IsAutoAssigned = false,
                                           Parents = new HashSet<string>(),
                                           Permissions = new HashSet<string>(),
                                           Priority = 0
                                       });
                        }

                        foreach (var attr in authorizationAttributes)
                        {
                            if (await permissionChecker.CheckPermissionAsync(actor, attr.Permission) != PermissionGrantResult.Grant)
                            {
                                throw new AuthorizationFailedException(component, attr.Permission);
                            }
                        }
                    });
                }
            }
        }
    }
}