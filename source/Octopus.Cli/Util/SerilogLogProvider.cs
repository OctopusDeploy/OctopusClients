using System;
using Octopus.Client.Logging;
using Serilog;
using Serilog.Context;
using LogLevel = Octopus.Client.Logging.LogLevel;

namespace Octopus.Cli.Util
{
    public class CliSerilogLogProvider : ILogProvider
    {
        public ILogger Logger { get; set; }

        public CliSerilogLogProvider(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static bool PrintMessages { get; set; }

        public Logger GetLogger(string name) 
            => new SerilogLogger(Logger.ForContext("SourceContext", name, destructureObjects: false)).Log;

        public IDisposable OpenNestedContext(string message) 
            => LogContext.PushProperty("Context", message);

        public IDisposable OpenMappedContext(string key, string value) 
            => LogContext.PushProperty(key, value, false);

        internal class SerilogLogger
        {
            private ILogger logger;

            public SerilogLogger(ILogger logger)
            {
                this.logger = logger;
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception, params object[] formatParameters)
            {
                if (!PrintMessages)
                    return false;

                var translatedLevel = TranslateLevel(logLevel);
                if (messageFunc == null)
                {
                    return logger.IsEnabled(translatedLevel);
                }

                if (!logger.IsEnabled(translatedLevel))
                {
                    return false;
                }

                if (exception != null)
                {
                    LogException(translatedLevel, messageFunc, exception, formatParameters);
                }
                else
                {
                    LogMessage(translatedLevel, messageFunc, formatParameters);
                }

                return true;
            }

            private void LogMessage(Serilog.Events.LogEventLevel logLevel, Func<string> messageFunc, object[] formatParameters)
            {
                logger.Write(logLevel, messageFunc(), formatParameters);
            }

            private void LogException(Serilog.Events.LogEventLevel logLevel, Func<string> messageFunc, Exception exception, object[] formatParams)
            {
                logger.Write(logLevel, exception, messageFunc(), formatParams);
            }

            private static Serilog.Events.LogEventLevel TranslateLevel(LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Fatal:
                        return Serilog.Events.LogEventLevel.Fatal;
                    case LogLevel.Error:
                        return Serilog.Events.LogEventLevel.Error;
                    case LogLevel.Warn:
                        return Serilog.Events.LogEventLevel.Warning;
                    case LogLevel.Info:
                        return Serilog.Events.LogEventLevel.Information;
                    case LogLevel.Trace:
                        return Serilog.Events.LogEventLevel.Verbose;
                    default:
                        return Serilog.Events.LogEventLevel.Debug;
                }
            }
        }
    }
}