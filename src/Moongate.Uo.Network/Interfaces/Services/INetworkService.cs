using Moongate.Core.Interfaces.Services.Base;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Interfaces.Services;

public interface INetworkService : IMoongateStartStopService
{
    delegate void PacketReceivedDelegate(SessionData session, IUoNetworkPacket packet);
    delegate void ClientConnectedDelegate(string serverId, string sessionId, NetClient client);
    delegate void ClientDisconnectedDelegate(string serverId, string sessionId);

    event ClientConnectedDelegate ClientConnected;
    event ClientDisconnectedDelegate ClientDisconnected;

    void ChangeOpCodeSize(byte opCode, int length);

    void RegisterPacket<TPacket>() where TPacket : IUoNetworkPacket, new();

    void RegisterPacketHandler<TPacket>(PacketReceivedDelegate handler) where TPacket : IUoNetworkPacket, new();
    void RegisterPacketHandler<TPacket, THandler>() where TPacket : IUoNetworkPacket, new() where THandler : IPacketListener;

    void Send(string sessionId, ReadOnlyMemory<byte> buffer);

    NetClient? GetClient(string sessionId, bool throwIfNotFound = true);

}
