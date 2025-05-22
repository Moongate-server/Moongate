using Swan.Logging;

namespace Moongate.Core.Web.Logging;

public class SwanToSerilogLogger : TextLogger, ILogger, IDisposable
{
    private readonly Type _sourceType;

    public SwanToSerilogLogger(Type sourceType)
    {
        _sourceType = sourceType;
    }

    public void Dispose()
    {
    }

    public void Log(LogMessageReceivedEventArgs logEvent)
    {
        var logger = Serilog.Log.ForContext(_sourceType);

        var message = logEvent.Message;

        var logLevel = logEvent.MessageType;

        switch (logLevel)
        {
            case LogLevel.Debug:
                logger.Debug(message);
                break;
            case LogLevel.Error:
                logger.Error(message);
                break;
            case LogLevel.Fatal:
                logger.Fatal(message);
                break;
            case LogLevel.Info:
                logger.Information(message);
                break;
            case LogLevel.Trace:
                logger.Verbose(message);
                break;
            case LogLevel.Warning:
                logger.Warning(message);
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public LogLevel LogLevel { get; }
}
