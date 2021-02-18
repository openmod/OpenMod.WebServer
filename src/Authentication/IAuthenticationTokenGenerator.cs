using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.WebServer.Authentication
{
    [Service]
    public interface IAuthenticationTokenGenerator
    {
        Task<string> GenerateTokenAsync(string userType, string userId);
    }
}