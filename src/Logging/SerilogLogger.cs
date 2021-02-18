using System;
using Microsoft.Extensions.Logging;
using Swan.Logging;
using ILogger = Swan.Logging.ILogger;
using LogLevel = Swan.Logging.LogLevel;
using MicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace OpenMod.WebServer.Logging
{
    public class SerilogLogger : ILogger
    {
        private readonly MicrosoftLogger _baseLogger;

        public SerilogLogger(MicrosoftLogger baseLogger)
        {
            _baseLogger = baseLogger;
        }

        public void Log(LogMessageReceivedEventArgs logEvent)
        {
            _baseLogger.Log(TranslateLogLevel(logEvent.MessageType), logEvent.Message);
        }

        protected MicrosoftLogLevel TranslateLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                    return MicrosoftLogLevel.None;
                case LogLevel.Trace:
                    return MicrosoftLogLevel.Trace;
                case LogLevel.Debug:
                    return MicrosoftLogLevel.Debug;
                case LogLevel.Info:
                    return MicrosoftLogLevel.Information;
                case LogLevel.Warning:
                    return MicrosoftLogLevel.Warning;
                case LogLevel.Error:
                    return MicrosoftLogLevel.Error;
                case LogLevel.Fatal:
                    return MicrosoftLogLevel.Critical;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public LogLevel LogLevel { get; } = LogLevel.Debug;

        public void Dispose()
        {
            // do nothing
        }
    }
}