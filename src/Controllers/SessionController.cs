using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.WebServer.Dtos;

namespace OpenMod.WebServer.Controllers
{
    public class SessionController : OpenModController
    {
        private readonly IUserManager _userManager;
        private readonly IPermissionRoleStore _roleStore;

        public SessionController(
            IServiceProvider serviceProvider,
            IUserManager userManager,
            IPermissionRoleStore roleStore) : base(serviceProvider)
        {
            _userManager = userManager;
            _roleStore = roleStore;
        }

        [Route(HttpVerbs.Get, "/session")]
        public virtual async Task<UserDto?> GetSession()
        {
            if (Actor == null)
            {
                return null;
            }

            var user = await _userManager.FindUserAsync(Actor.Type, Actor.Id, UserSearchMode.FindById);
            if (user == null)
            {
                return new UserDto
                {
                    Id = Actor.Id,
                    Type = Actor.Type,
                    Roles = new List<RoleDto>()
                };
            }

            var roles = await _roleStore.GetRolesAsync(user);
            return new UserDto
            {
                Id = Actor.Id,
                Type = Actor.Type,
                SessionStartTime = user.Session?.SessionStartTime,
                Roles = roles.Select(d => new RoleDto
                {
                    DisplayName = d.DisplayName,
                    Id = d.Id,
                    IsAutoAssigned = d.IsAutoAssigned,
                    Priority = d.Priority
                }).ToList()
            };
        }
    }
}