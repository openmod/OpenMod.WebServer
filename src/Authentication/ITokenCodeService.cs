using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;

namespace OpenMod.WebServer.Authentication
{
    [Service]
    public interface ITokenCodeService
    {
        string GenerateCode(IPermissionActor actor);

        Task<AuthToken?> CreateAuthTokenAsync(string code, TokenCreationParameters parameters);

        bool DeleteCode(string inputCode);
    }
}