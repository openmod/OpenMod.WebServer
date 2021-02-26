using System.Drawing;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Modules
{
    public class WebConsoleUser : IUser
    {
        private readonly IUser _user;
        private readonly OpenModConsoleModule _module;
        public IWebSocketContext Context { get; }

        public WebConsoleUser(IUser user, IWebSocketContext context, OpenModConsoleModule module)
        {
            _user = user;
            _module = module;
            Context = context;

            Id = _user.Id;
            Type = _user.Type;
            DisplayName = _user.FullActorName;
            FullActorName = _user.DisplayName;
            Provider = _user.Provider;
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName { get; }
        public string FullActorName { get; }

        public Task PrintMessageAsync(string message)
        {
            return _module.OnActorMessage(this, message);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message);
        }

        public Task SavePersistentDataAsync<T>(string key, T? data)
        {
            return _user.SavePersistentDataAsync(key, data);
        }

        public Task<T?> GetPersistentDataAsync<T>(string key)
        {
            return _user.GetPersistentDataAsync<T>(key);
        }

        public IUserSession? Session => _user.Session;
        public IUserProvider? Provider { get; }
    }
}