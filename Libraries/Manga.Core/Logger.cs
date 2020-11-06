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

        public void Log(Common.Enums.LogLevel logLevel, string message)
        {
            Instance.Log(GetNLogLogTargetEquivalent(logLevel), message);
        }

        public void Log(Common.Enums.LogLevel logLevel, string message, params object[] args)
        {
            Instance.Log(GetNLogLogTargetEquivalent(logLevel), message, args);
        }

        public void Log(Common.Enums.LogLevel logLevel, string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Log(GetNLogLogTargetEquivalent(logLevel), formatProvider, message, args);
        }

        public void Trace(string message)
        {
            Instance.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            Instance.Trace(message, args);
        }

        public void Trace(string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Trace(formatProvider, message, args);
        }

        public void Info(string message)
        {
            Instance.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            Instance.Info( message, args);
        }

        public void Info(string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Info(formatProvider, message, args);
        }

        public void Warning(string message)
        {
            Instance.Warn(message);
        }

        public void Warning(string message, params object[] args)
        {
            Instance.Warn(message, args);
        }

        public void Warning(string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Warn(formatProvider, message, args);
        }

        public void Error(string message)
        {
            Instance.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            Instance.Error(message, args);
        }

        public void Error(string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Error(formatProvider, message, args);
        }

        public void Fatal(string message)
        {
            Instance.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            Instance.Fatal(message, args);
        }

        public void Fatal(string message, IFormatProvider formatProvider, params object[] args)
        {
            Instance.Fatal(formatProvider, message, args);
        }


        private static LogLevel GetNLogLogTargetEquivalent(Common.Enums.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Common.Enums.LogLevel.Trace:
                    return LogLevel.Trace;
                case Common.Enums.LogLevel.Debug:
                    return LogLevel.Debug;
                case Common.Enums.LogLevel.Info:
                    return LogLevel.Info;
                case Common.Enums.LogLevel.Warning:
                    return LogLevel.Warn;
                case Common.Enums.LogLevel.Error:
                    return LogLevel.Error;
                case Common.Enums.LogLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    throw new NotSupportedException();
            }                
        }
    }
}
