using Manga.Common.Enums;

namespace Manga.Common.Interfaces
{
    public interface ILogger
    {
        void Log(string message, LogLevel logLevel);
    }
}