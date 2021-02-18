using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using OpenMod.WebServer.Dtos;

namespace OpenMod.WebServer.Controllers
{
    public class SessionController : OpenModController
    {
        public SessionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Route(HttpVerbs.Get, "/session")]
        public virtual Task<SessionDto> GetSession()
        {
            return Task.FromResult(new SessionDto());
        }
    }
}