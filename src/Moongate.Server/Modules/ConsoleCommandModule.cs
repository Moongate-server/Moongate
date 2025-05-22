using Moongate.Core.Attributes.Scripts;
using Moongate.Core.Data.Commands;
using Moongate.Core.Instances;
using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Server.Modules;

[ScriptModule("console")]
public class ConsoleCommandModule
{
    private readonly IConsoleCommandService _consoleCommandService;

    public ConsoleCommandModule(IConsoleCommandService consoleCommandService)
    {
        _consoleCommandService = consoleCommandService;
    }

    [ScriptFunction("register", "Register a new console command")]
    public void RegisterCommand(string command, string description, Action<string[]> handler)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentException("Command cannot be null or empty.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler), "Handler cannot be null.");
        }

        _consoleCommandService.RegisterCommand(command, description, async args => handler(args));
    }
}
