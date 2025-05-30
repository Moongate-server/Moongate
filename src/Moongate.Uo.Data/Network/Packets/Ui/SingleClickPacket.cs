using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Ui;

public class SingleClickPacket : IUoNetworkPacket
{
    public byte OpCode => 0x09;
    public int Length => 5;

    public Serial Id { get; set; }

    public bool Read(SpanReader reader)
    {
        Id = new Serial(reader.ReadUInt32());

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }

    public override string ToString()
    {
        return $"SingleClick Id={Id}";
    }
}
