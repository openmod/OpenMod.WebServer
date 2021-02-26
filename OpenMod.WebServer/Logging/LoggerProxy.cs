using System;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenMod.WebServer.Logging
{
    /*
    public class LoggerProxy<T> : LoggerProxy, ILogger<T>
    {
        public LoggerProxy(ILoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<T>())
        {
        }
    }
    */

    public delegate void OnLog(LogLevel logLevel, EventId eventId, object? state, Exception? exception,
        Func<object, Exception?, string>? formatter);

    public class LoggerProxy : ILogger
    {
        public static event OnLog? Logged;

        private readonly ILogger _baseLogger;

        public LoggerProxy(ILogger baseLogger)
        {
            _baseLogger = baseLogger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string>? formatter)
        {
            Logged?.Invoke(logLevel, eventId, state, exception, formatter != null ? (o, e) => formatter((TState) o, e) : null);
            _baseLogger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _baseLogger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _baseLogger.BeginScope(state);
        }
    }
}