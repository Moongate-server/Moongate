using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.System;

public class PingMessagePacket : IUoNetworkPacket
{
    public byte OpCode => 0x73;
    public int Length => 2;

    public byte Sequence { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        Sequence = reader.ReadByte();

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(Sequence);

        return writer.ToArray();
    }

    public override string ToString()
    {
        return $"PingMessagePacket: Sequence={Sequence}";
    }
}
