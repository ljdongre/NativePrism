// ILogger.cs

namespace NativePrism
{
    public interface ILoggerFacade
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
    }
}