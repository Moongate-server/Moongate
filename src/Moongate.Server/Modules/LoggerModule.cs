using Moongate.Core.Attributes.Scripts;
using Serilog;

namespace Moongate.Server.Modules;

[ScriptModule("logger")]
public class LoggerModule
{
    private readonly ILogger _logger = Log.ForContext<LoggerModule>();

    [ScriptFunction("info")]
    public void LogInfo(string message, params object[] args)
    {
        _logger.Information("[LUA] " + message, args);
    }

    [ScriptFunction("warn")]
    public void LogWarning(string message, params object[] args )
    {
        _logger.Warning("[LUA] " + message, args);
    }

    [ScriptFunction("error")]
    public void LogError(string message, params object[] args)
    {
        _logger.Error("[LUA] " + message, args);
    }

    [ScriptFunction("debug")]
    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug("[LUA] " + message, args);
    }
}
