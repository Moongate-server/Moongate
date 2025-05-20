using Moongate.Core.Attributes.Scripts;

namespace Moongate.Server.Modules;


[ScriptModule("logger")]
public class LoggerModule
{

    [ScriptFunction("info")]
    public void LogInfo(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }
}
