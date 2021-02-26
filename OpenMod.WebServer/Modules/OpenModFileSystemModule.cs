using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zio;
using Zio.FileSystems;

namespace OpenMod.WebServer.Modules
{
    public class OpenModFileSystemModule : WebModuleBase, IDisposable
    {
        public AggregateFileSystem FileSystem { get; }
        public List<string> IndexFiles = new() { "index.html", "index.htm" };
        private readonly ILogger<OpenModFileSystemModule> _logger;
        private readonly IConfiguration _configuration;

        public OpenModFileSystemModule(string baseRoute,
            IConfiguration configuration,
            ILogger<OpenModFileSystemModule> logger) : base(baseRoute)
        {
            _configuration = configuration;
            _logger = logger;

            FileSystem = new AggregateFileSystem(owned: true);
        }

        protected override Task OnRequestAsync(IHttpContext context)
        {
            if (context.Request.HttpVerb != HttpVerbs.Get)
            {
                _logger.LogDebug("Request was not GET");
                return Task.FromException(HttpException.MethodNotAllowed());
            }

            var path = context.RequestedPath.Replace('/', Path.DirectorySeparatorChar);

            if (FileSystem.FileExists(path))
            {
                return SendFileAsync(context, path);
            }

            if (FileSystem.DirectoryExists(path))
            {
                foreach (var indexFile in IndexFiles)
                {
                    var indexPath = Path.Combine(path, indexFile);
                    if (FileSystem.FileExists(indexPath))
                    {
                        return SendFileAsync(context, indexPath);
                    }
                }
            }

            _logger.LogDebug($"File not found: {path}");
            return OnNotFoundAsync(context);

        }
        protected virtual async Task SendFileAsync(IHttpContext context, string file)
        {
            var data = FileSystem.ReadAllText(file, Encoding.UTF8);
            var contentType = context.GetMimeType(Path.GetExtension(file));
            _logger.LogDebug($"Sending file: {file}");

            await context.SendStringAsync(data, contentType, Encoding.UTF8);
        }

        protected virtual Task OnNotFoundAsync(IHttpContext context)
        {
            if (FileSystem.FileExists("/404.html"))
            {
                context.Response.StatusCode = 404;
                return SendFileAsync(context, "/404.html");
            }

            if (_configuration.GetSection("spaSupport").Get<bool?>() ?? false)
            {
                // Required for SPAs: send index.html if not found with 200 OK
                // SPA will handle route

                foreach (var indexFile in IndexFiles)
                {
                    if (FileSystem.FileExists($"/{indexFile}"))
                    {
                        return SendFileAsync(context, $"/{indexFile}");
                    }
                }
            }

            return context.SendStandardHtmlAsync((int)HttpStatusCode.NotFound);
        }

        public override bool IsFinalHandler => true;

        public void Dispose()
        {
            FileSystem.Dispose();
        }
    }
}