using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using JetBrains.Annotations;
using OpenMod.API.Plugins;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Dtos;

namespace OpenMod.WebServer.Controllers
{
    public class PluginsController : OpenModController
    {
        private readonly IPluginActivator _pluginActivator;
        public const string PermissionGetAllPlugins = "api.plugins.get.all";

        public PluginsController(
            IServiceProvider serviceProvider,
            IPluginActivator pluginActivator) : base(serviceProvider)
        {
            _pluginActivator = pluginActivator;
        }

        [Route(HttpVerbs.Get, "/plugins")]
        [Authorize(PermissionGetAllPlugins)]
        public virtual Task<ICollection<PluginDto>> GetPlugins()
        {
            var plugins = new List<PluginDto>();
            foreach(var plugin in _pluginActivator.ActivatedPlugins)
            {
                plugins.Add(new PluginDto
                {
                    Id = plugin.OpenModComponentId,
                    Author = plugin.Author,
                    DisplayName = plugin.DisplayName,
                    Version = plugin.Version.ToString(),
                    Website = plugin.Website
                });
            }

            return Task.FromResult<ICollection<PluginDto>>(plugins);
        }
    }
}