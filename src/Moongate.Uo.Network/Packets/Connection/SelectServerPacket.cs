using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets.Connection;

public class SelectServerPacket : IUoNetworkPacket
{
    public byte OpCode => 0xA0;
    public int Length => 3;

    public int ShardId { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        ShardId = reader.ReadUInt16();

        return true;
    }


    public override string ToString()
    {
        return $"SelectServerPacket: ShardId: {ShardId}";
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }
}
