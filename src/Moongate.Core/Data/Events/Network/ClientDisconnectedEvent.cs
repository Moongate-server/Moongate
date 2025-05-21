namespace Moongate.Core.Data.Events.Network;

public record ClientDisconnectedEvent(string ServerId, string SessionId);
