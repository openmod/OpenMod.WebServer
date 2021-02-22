using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.WebServer.Authentication
{
    public class WebUser : IUser
    {
        private readonly IUser? _baseUser;

        public WebUser(IUser baseUser)
        {
            Id = baseUser.Id;
            Type = baseUser.Type;
            _baseUser = baseUser;
        }

        public WebUser(string type, string id)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName => _baseUser?.DisplayName ?? Id;

        public Task PrintMessageAsync(string message)
        {
            if (_baseUser == null)
            {
                return Task.CompletedTask;
            }

            return _baseUser.PrintMessageAsync(message);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            if (_baseUser == null)
            {
                return Task.CompletedTask;
            }

            return _baseUser.PrintMessageAsync(message, color);
        }

        public string FullActorName
        {
            get
            {
                if (_baseUser == null)
                {
                    return $"{Type}/{Id}";
                }

                return _baseUser.FullActorName;
            }
        }

        public Task SavePersistentDataAsync<T>(string key, T? data)
        {
            if (_baseUser == null)
            {
                return Task.CompletedTask;
            }

            return _baseUser.SavePersistentDataAsync(key, data);
        }

        public Task<T?> GetPersistentDataAsync<T>(string key)
        {
            if (_baseUser == null)
            {
                return Task.FromResult<T?>(default);
            }

            return _baseUser.GetPersistentDataAsync<T>(key);
        }

        public IUserSession? Session => _baseUser?.Session;

        public IUserProvider? Provider => _baseUser?.Provider;
    }
}