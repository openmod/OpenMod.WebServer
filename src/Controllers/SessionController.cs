using System.Collections.Generic;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using OpenMod.ApiServer.Dtos;

namespace OpenMod.ApiServer.Controllers
{
    public class SessionController : WebApiController
    {
        [Route(HttpVerbs.Get, "/session")]
        public virtual Task<SessionDto> GetSession()
        {
            return Task.FromResult(new SessionDto());
        }
    }
}