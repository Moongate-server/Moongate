namespace Moongate.Core.Data.Events.Network;

public record ClientConnectedEvent(string ServerId, string SessionId);
