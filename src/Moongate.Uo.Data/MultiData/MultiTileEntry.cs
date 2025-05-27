using Moongate.Uo.Data.Mul;

namespace Moongate.Uo.Data.MultiData;

public struct MultiTileEntry
{
    public ushort ItemId { get; set; }
    public short OffsetX { get; set; }
    public short OffsetY { get; set; }
    public short OffsetZ { get; set; }
    public TileFlag Flags { get; set; }

    public MultiTileEntry(ushort itemID, short xOffset, short yOffset, short zOffset, TileFlag flags)
    {
        ItemId = itemID;
        OffsetX = xOffset;
        OffsetY = yOffset;
        OffsetZ = zOffset;
        Flags = flags;
    }
}
