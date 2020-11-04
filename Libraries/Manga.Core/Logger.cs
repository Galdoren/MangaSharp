using NLog;
using NLog.Config;
using NLog.Targets;
using System;

namespace Manga.Core
{
    public class Logger : Common.Interfaces.ILogger
    {
        private static NLog.Logger Instance => LogManager.GetLogger("*");

        static Logger()
        {
            var configuration = new LoggingConfiguration();
            var logFile = new FileTarget() 
            { 
                ArchiveEvery = FileArchivePeriod.Day,
                FileName = "log.txt" 
            };
            configuration.AddRuleForAllLevels(logFile);
            LogManager.Configuration = configuration;   
        }

        public void Log(string message, Common.Enums.LogLevel logLevel)
        {
            Instance.Log(GetNLogLogTargetEquivalent(logLevel), message);
        }

        private static LogLevel GetNLogLogTargetEquivalent(Common.Enums.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Common.Enums.LogLevel.Debug:
                    return LogLevel.Debug;
                case Common.Enums.LogLevel.Info:
                    return LogLevel.Info;
                case Common.Enums.LogLevel.Warning:
                    return LogLevel.Warn;
                case Common.Enums.LogLevel.Error:
                    return LogLevel.Error;
                default:
                    throw new NotSupportedException();
            }                
        }
    }
}
