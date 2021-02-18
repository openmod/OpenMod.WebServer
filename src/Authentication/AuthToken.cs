using System;
using System.ComponentModel.DataAnnotations;

namespace OpenMod.WebServer.Authentication
{
    public class AuthToken
    {
        [Key]
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string OwnerType { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;

        public DateTime? ExpirationTime { get; set; }

        public DateTime? RevokeTime { get; set; }
    }
}