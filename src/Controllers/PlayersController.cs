using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.WebServer.Authorization;
using OpenMod.WebServer.Dtos;

namespace OpenMod.WebServer.Controllers
{
    public class PlayersController : OpenModController
    {
        public const string PermissionGetAllPlayers = "apis.players.get.all";
        public const string PermissionGetPlayer = "apis.players.get.all";

        private readonly IUserManager _userManager;
        private readonly IPermissionRoleStore _roleStore;

        public PlayersController(
            IServiceProvider serviceProvider,
            IUserManager userManager,
            IPermissionRoleStore roleStore) : base(serviceProvider)
        {
            _userManager = userManager;
            _roleStore = roleStore;
        }

        [Route(HttpVerbs.Get, "/players")]
        [Authorize(PermissionGetAllPlayers)]
        public virtual async Task<ICollection<PlayerDto>> GetPlayers()
        {
            var list = new List<PlayerDto>();
            var users = await _userManager.GetUsersAsync(KnownActorTypes.Player);
            foreach (var user in users)
            {
                if (!(user is IPlayerUser))
                {
                    continue;
                }

                list.Add(await MapToPlayerDtoAsync(user));
            }

            return list;
        }

        [Route(HttpVerbs.Get, "/players/{id?}")]
        [Authorize(PermissionGetPlayer)]
        public virtual async Task<PlayerDto> GetPlayer(string id)
        {
            IUser? user;
            try
            {
                user = await _userManager.FindUserAsync(KnownActorTypes.Player, id, UserSearchMode.FindById);
            }
            catch
            {
                user = null;
            }

            user = user as IPlayerUser;

            if (user == null)
            {
                throw HttpException.NotFound();
            }

            return await MapToPlayerDtoAsync(user);
        }

        private async Task<PlayerDto> MapToPlayerDtoAsync(IUser user)
        {
            var roles = await _roleStore.GetRolesAsync(user);
            return new PlayerDto
            {
                Id = user.Id,
                SessionStartTime = user.Session?.SessionStartTime,
                Roles = roles.Select(d => d.Id).ToList(),
            };
        }
    }
}