using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private static readonly MethodInfo CompileHandlerMethod;
        private readonly IServiceProvider _serviceProvider;

        public OpenModWebApiModule(string baseRoute, IServiceProvider serviceProvider) : base(baseRoute)
        {
            _serviceProvider = serviceProvider;
        }

        static OpenModWebApiModule()
        {
            CompileHandlerMethod = typeof(WebApiModuleBase)
                    .GetMethod("CompileHandler", BindingFlags.Instance | BindingFlags.NonPublic)!;
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
            RegisterControllerType<TController>(component);
        }

        private OpenModController CreateComponentFactory(Type type, IOpenModComponent component)
        {
            var scope = component.LifetimeScope.BeginLifetimeScope("AutofacWebRequest");
            return (OpenModController)ActivatorUtilitiesEx.CreateInstance(scope, type);
        }

        private void RegisterControllerType<T>(IOpenModComponent component) where T: OpenModController
        {
            var controllerType = typeof(T);
            Func<T> factory = () => (T) CreateComponentFactory(controllerType, component);
            var expression = Expression.Call(
                factory.Target == null ? null : Expression.Constant(factory.Target),
                factory.Method);

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

                foreach (var attribute in routeAttributes)
                {
                    var callbacks = new List<RouteHandlerCallback>();

                    async Task AggregateHandler(IHttpContext context, RouteMatch route)
                    {
                        foreach (var callback in callbacks)
                        {
                            await callback.Invoke(context, route);
                        }
                    }

                    callbacks.Add(async (context, _) =>
                    {
                        var scope = component.LifetimeScope;
                        var permissionChecker = scope.Resolve<IPermissionChecker>();
                        var authenticationToken = context.Request.Headers["Authorization"]?.Replace("Bearer ", string.Empty);
                        var authenticationService = _serviceProvider.GetRequiredService<IAuthenticationService>();

                        IPermissionActor? actor = null;
                        if (!string.IsNullOrEmpty(authenticationToken))
                        {
                            actor = await authenticationService.AuthenticateAsync(authenticationToken!);
                        }

                        if (actor == null)
                        {
                            var roleStore = scope.Resolve<IPermissionRoleStore>();
                            actor = await roleStore.GetRoleAsync("AnonymousWebUsers")
                                       ?? new PermissionRole(new PermissionRoleData
                                       {
                                           Id = "AnonymousWebUsers",
                                           DisplayName = "Anonymous Web Users",
                                           Data = new Dictionary<string, object?>(),
                                           IsAutoAssigned = false,
                                           Parents = new HashSet<string>(),
                                           Permissions = new HashSet<string>(),
                                           Priority = 0
                                       });
                        }

                        context.Items["actor"] = actor;

                        foreach (var attr in authorizationAttributes)
                        {
                            if (await permissionChecker.CheckPermissionAsync(actor, attr.Permission) != PermissionGrantResult.Grant)
                            {
                                throw new AuthorizationFailedException(component, attr.Permission);
                            }
                        }
                    });

                    callbacks.Add((RouteHandlerCallback)CompileHandlerMethod.Invoke(this, new object[] { expression, method, attribute.Matcher }));
                    AddHandler(attribute.Verb, attribute.Matcher, AggregateHandler);
                }
            }
        }
    }
}