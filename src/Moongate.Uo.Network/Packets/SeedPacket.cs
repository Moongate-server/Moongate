using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets;

public class SeedPacket : IUoNetworkPacket
{
    public byte OpCode => 0xEF;
    public int Length => 21;

    public int Major { get; set; }

    public int Minor { get; set; }

    public int Revision { get; set; }

    public int Prototype { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadBytes(5);
        Major = reader.ReadInt32();
        Minor = reader.ReadInt32();
        Revision = reader.ReadInt32();
        Prototype = reader.ReadInt32();

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }

    public override string ToString()
    {
        return $"SeedPacket: Major={Major}, Minor={Minor}, Revision={Revision}, Prototype={Prototype}";
    }
}
