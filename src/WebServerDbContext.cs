using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using OpenMod.WebServer.Authentication;

namespace OpenMod.WebServer
{
    public class WebServerDbContext : OpenModDbContext<WebServerDbContext>
    {
        public DbSet<AuthToken> AuthTokens { get; set; } = null!;

        public WebServerDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public WebServerDbContext(
            DbContextOptions<WebServerDbContext> options, 
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}