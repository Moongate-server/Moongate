using Moongate.Core.Interfaces.Services.Base;
using Moongate.Core.Network.Interfaces.Messages;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Core.Spans;

namespace Moongate.Core.Interfaces.Services.System;

public interface INetworkService : IMoongateStartStopService
{
    delegate IUoNetworkPacket? ByteReceivedHandler(string serverId, string sessionId, SpanReader buffer);
    delegate void PacketReceivedHandler(string serverId, string sessionId, IUoNetworkPacket packet);
    delegate void ClientConnectedDelegate(string serverId, string sessionId, NetClient client);
    delegate void ClientDisconnectedDelegate(string serverId, string sessionId);

    event ClientConnectedDelegate ClientConnected;
    event ClientDisconnectedDelegate ClientDisconnected;

    void AddOpCodeHandler(byte opCode, int length, ByteReceivedHandler handler);
    void ChangeOpCodeSize(byte opCode, int length);
    void AddPacketHandler(byte opCode, PacketReceivedHandler handler);
    void Send(string sessionId, ReadOnlyMemory<byte> buffer);
    NetClient? GetClient(string sessionId, bool throwIfNotFound = true);

}
