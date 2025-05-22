using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface IConsoleCommandService : IMoongateService
{
    delegate Task CommandHandlerDelegate(string[] args);

    void RegisterCommand(string command, string description, CommandHandlerDelegate handler);

    void Autocomplete(string input);

    Task ProcessCommand(string input);
}
