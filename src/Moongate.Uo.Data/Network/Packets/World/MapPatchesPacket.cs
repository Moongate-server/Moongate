using DryIoc.ImTools;
using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.World;

public class MapPatchesPacket : IUoNetworkPacket
{
    public byte OpCode => 0xBF;
    public int Length => 41;

    public Map[] Maps { get; set; }

    public MapPatchesPacket(Map[] maps = null)
    {
        Maps = maps;
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((ushort)Length);
        writer.Write((byte)0x18);
        writer.Write(4);

        for (int i = 0; i < 4; i++)
        {
            var map = Map.Maps[i];

            writer.Write(map?.Tiles.Patch.StaticBlocks ?? 0);
            writer.Write(map?.Tiles.Patch.LandBlocks ?? 0);
        }


        return writer.ToArray();

    }
}
