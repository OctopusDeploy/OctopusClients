using System;
using System.IO;
using System.Xml;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;

namespace OctopusTools.Diagnostics
{
    public static class Logger
    {
        public static string LoggingConfiguration
        {
            get
            {
                using (var reader = new StreamReader(typeof (Logger).Assembly.GetManifestResourceStream(GetLoggingFileName())))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static ILog Default
        {
            get { return Nested.Log; }
        }

        static string GetLoggingFileName()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return "logging-unix.config";
                default:
                    return "logging.config";
            }
        }

        public static void SetLevel(this ILoggerWrapper log, string levelName)
        {
            var logger = (log4net.Repository.Hierarchy.Logger) log.Logger;
            logger.Level = logger.Hierarchy.LevelMap[levelName];
        }

        public static void AddAppender(this ILoggerWrapper log, IAppender appender)
        {
            var logger = (log4net.Repository.Hierarchy.Logger) log.Logger;
            logger.AddAppender(appender);
        }

        #region Nested type: Nested

        static class Nested
        {
            public static readonly ILog Log;

            static Nested()
            {
                var document = new XmlDocument();
                document.LoadXml(LoggingConfiguration);

                Log = LogManager.GetLogger("Octopus");

                XmlConfigurator.Configure(Log.Logger.Repository, (XmlElement) document.GetElementsByTagName("log4net")[0]);
            }
        }

        #endregion
    }
}