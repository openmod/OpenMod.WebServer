using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.Core.Ioc;
using OpenMod.Core.Permissions;
using OpenMod.Core.Permissions.Data;
using OpenMod.WebServer.Authentication;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Controllers;

namespace OpenMod.WebServer.Routing
{
    public class OpenModWebApiModule : WebApiModuleBase
    {
        private readonly Dictionary<Type, IOpenModComponent> _openModComponents = new();
        public OpenModWebApiModule(string baseRoute) : base(baseRoute)
        {
        }

        public OpenModWebApiModule(string baseRoute, ResponseSerializerCallback serializer) : base(baseRoute, serializer)
        {
        }
        
        public void RegisterController<TController>(IOpenModComponent component)
            where TController : OpenModController
        {
            RegisterController(component, () => (TController) CreateComponentFactory(typeof(TController), component));
        }

        public void RegisterController<TController>(IOpenModComponent component, Func<TController> factory)
            where TController : OpenModController
        {
            _openModComponents.Add(typeof(TController), component);
            RegisterControllerType(typeof(TController), factory);
        }

        public void RegisterController(Type controllerType, IOpenModComponent component)
        {
            RegisterController(controllerType, component, () => CreateComponentFactory(controllerType, component));
        }

        public void RegisterController(Type controllerType, IOpenModComponent component, Func<WebApiController> factory)
        {
            _openModComponents.Add(controllerType, component);
            RegisterControllerType(controllerType, factory);
        }

        private OpenModController CreateComponentFactory(Type type, IOpenModComponent component)
        {
            AddAuthorizationHandler(type, component);
            var scope = component.LifetimeScope.BeginLifetimeScope("AutofacWebRequest");
            return (OpenModController) ActivatorUtilitiesEx.CreateInstance(scope, type);
        }

        private void AddAuthorizationHandler(Type controllerType, IOpenModComponent openModComponent)
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
                        var scope = (ILifetimeScope) context.Items["scope"];
                        var permissionChecker = scope.Resolve<IPermissionChecker>();
                        var authenticationToken = context.Request.Headers["Authentication"];
                        var authenticationService = scope.Resolve<IAuthenticationService>();

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

                        foreach(var attr in authorizationAttributes)
                        {
                            if (await permissionChecker.CheckPermissionAsync(actor, attr.Permission) != PermissionGrantResult.Grant)
                            {
                                throw new AuthorizationFailedException($"Missing permission: {attr.Permission}.");
                            }
                        }
                    });
                }
            }
        }
    }
}