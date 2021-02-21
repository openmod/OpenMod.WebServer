namespace OpenMod.WebServer.Authentication
{
    public class AuthToken
    {
        public string Token { get; set; } = null!;

        public string OwnerType { get; set; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}