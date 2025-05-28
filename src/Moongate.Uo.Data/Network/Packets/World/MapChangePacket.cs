using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.World;

public class MapChangePacket : IUoNetworkPacket
{
    public Map Map { get; set; }
    public byte OpCode => 0xBF;
    public int Length => 6;
    public bool Read(SpanReader reader)
    {
        return false;
    }

    public MapChangePacket(Map map = null)
    {
        Map = map;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((byte)0);
        writer.Write((byte)0x06);
        writer.Write((byte)0);
        writer.Write((byte)0x08);
        writer.Write((byte)Map.MapID);

        return writer.ToArray();
    }
}
