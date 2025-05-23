using Moongate.Uo.Services.Types;

namespace Moongate.Uo.Services.Events.Accounts;

public record AccountCreatedEvent(string Id, string Username, AccountLevelType Level);
