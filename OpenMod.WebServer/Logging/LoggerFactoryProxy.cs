using Microsoft.Extensions.Logging;

namespace OpenMod.WebServer.Logging
{
    public class LoggerFactoryProxy : ILoggerFactory
    {
        private readonly ILoggerFactory _baseLoggerFactory;

        public LoggerFactoryProxy(ILoggerFactory baseLoggerFactory)
        {
            _baseLoggerFactory = baseLoggerFactory;
        }

        public void Dispose()
        {
            _baseLoggerFactory.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            var baseLogger = _baseLoggerFactory.CreateLogger(categoryName);
            return new LoggerProxy(baseLogger);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _baseLoggerFactory.AddProvider(provider);
        }
    }
}