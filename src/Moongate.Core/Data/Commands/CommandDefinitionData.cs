using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Core.Data.Commands;

public record CommandDefinitionData(string Command, string Description, IConsoleCommandService.CommandHandlerDelegate Handler);
