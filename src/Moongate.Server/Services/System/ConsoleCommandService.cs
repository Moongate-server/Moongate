using DryIoc;
using Moongate.Core.Data.Commands;
using Moongate.Core.Instances;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Services.Base;
using Moongate.Core.Services.Hosted;
using Serilog;
using ZLinq;

namespace Moongate.Server.Services.System;

public class ConsoleCommandService : AbstractBaseMoongateService, IConsoleCommandService
{
    private readonly Dictionary<string, CommandDefinitionData> _commands = new();

    public ConsoleCommandService() : base(Log.ForContext<ConsoleCommandService>())
    {
        RegisterCommand("quit|exit", "Exit the application.", QuitCommand);
        RegisterCommand("help|h|?", "Show help information.", HelpCommand);
    }

    private async Task QuitCommand(string[] args)
    {
        await MoongateInstanceHolder.Container.Resolve<MoongateStartupService>()
            .StopAsync(MoongateInstanceHolder.ConsoleCancellationTokenSource.Token);
        await MoongateInstanceHolder.ConsoleCancellationTokenSource.CancelAsync();
    }

    private async Task HelpCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Available commands:");
            foreach (var command in _commands)
            {
                Console.WriteLine($"- {command.Key} : {command.Value.Description}");
            }
        }
        else
        {
            var command = args[0];
            if (_commands.TryGetValue(command, out var commandDefinition))
            {
                Console.WriteLine($"{commandDefinition.Command} : {commandDefinition.Description}");
            }
            else
            {
                Console.WriteLine($"Command '{command}' not found.");
            }
        }
    }

    public void RegisterCommand(string commands, string description, IConsoleCommandService.CommandHandlerDelegate handler)
    {
        foreach (var command in commands.Split('|'))
        {
            if (_commands.ContainsKey(command))
            {
                throw new ArgumentException($"Command '{command}' is already registered.");
            }

            Log.Debug("Registering command '{Command}' ({Description})", command, description);
            _commands[command] = new CommandDefinitionData(command, description, handler);
        }
    }

    public void Autocomplete(string input)
    {
        var matchingCommands = _commands
            .AsValueEnumerable()
            .Where(c => c.Key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Key)
            .ToList();

        if (matchingCommands.Count > 0)
        {
            Console.WriteLine("Matching commands:");
            foreach (var command in matchingCommands)
            {
                Console.WriteLine($"- {command} : {_commands[command].Description}");
            }
        }
        else
        {
            Console.WriteLine("No matching commands found.");
        }
    }

    public Task ProcessCommand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return Task.CompletedTask;
        }

        var command = parts[0];
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(command, out var commandDefinition))
        {
            return Task.Run(() => commandDefinition.Handler(args));
        }
        else
        {
            Console.WriteLine($"Command '{command}' not found.");
            return Task.CompletedTask;
        }
    }
}
