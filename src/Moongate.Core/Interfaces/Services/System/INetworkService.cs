using Moongate.Core.Interfaces.Services.Base;
using Moongate.Core.Network.Interfaces.Messages;

namespace Moongate.Core.Interfaces.Services.System;

public interface INetworkService : IMoongateStartStopService
{
    public delegate IUoNetworkPacket? ByteReceivedHandler(string serverId, string sessionId, ReadOnlyMemory<byte> buffer);
    public delegate void PacketReceivedHandler(string serverId, string sessionId, IUoNetworkPacket packet);

    void AddOpCodeHandler(byte opCode, int length, ByteReceivedHandler handler);
    void ChangeOpCodeSize(byte opCode, int length);
    void AddPacketHandler(byte opCode, PacketReceivedHandler handler);


}
