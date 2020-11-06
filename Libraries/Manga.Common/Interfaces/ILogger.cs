using Manga.Common.Enums;

namespace Manga.Common.Interfaces
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string message);

        void Log(LogLevel logLevel, string message, params object[] args);

        void Log(LogLevel logLevel, string message, System.IFormatProvider formatProvider, params object[] args);

        void Trace(string message);

        void Trace(string message, params object[] args);

        void Trace(string message, System.IFormatProvider formatProvider, params object[] args);

        void Info(string message);

        void Info(string message, params object[] args);

        void Info(string message, System.IFormatProvider formatProvider, params object[] args);

        void Warning(string message);

        void Warning(string message, params object[] args);

        void Warning(string message, System.IFormatProvider formatProvider, params object[] args);

        void Error(string message);

        void Error(string message, params object[] args);

        void Error(string message, System.IFormatProvider formatProvider, params object[] args);

        void Fatal(string message);

        void Fatal(string message, params object[] args);

        void Fatal(string message, System.IFormatProvider formatProvider, params object[] args);
    }
}