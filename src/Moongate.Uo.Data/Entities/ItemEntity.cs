using System.Runtime.CompilerServices;
using MemoryPack;
using Moongate.Core.Data.Ids;
using Moongate.Persistence.Attributes;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Interfaces.Entities;
using Moongate.Uo.Data.Mul;
using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data.Entities;

[MemoryPackable]
[EntityType(0x03, Description = "Item Entity")]
public partial class ItemEntity
{
    public Serial Serial { get; set; }

    public int ItemId { get; set; }

    public Direction Direction { get; set; }

    public ItemImplFlag ImplFlag { get; set; }

    public int Hue { get; set; }

    public Layer Layer { get; set; }

    public int Amount { get; set; }

    public Point3D Location { get; set; }

    public LootType LootType { get; set; }

    [MemoryPackAllowSerialize]
    public Map Map { get; set; }

    [MemoryPackAllowSerialize] public ItemData ItemData => TileData.ItemTable[ItemId & TileData.MaxItemValue];

    public ItemEntity(int itemId = 0)
    {
        ItemId = itemId;

        Visible = true;
        Movable = true;
        Amount = 1;
        Map = Map.Internal;

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool GetFlag(ItemImplFlag flag) => (ImplFlag & flag) != 0;

    private void SetFlag(ItemImplFlag flag, bool value)
    {
        if (value)
        {
            ImplFlag |= flag;
        }
        else
        {
            ImplFlag &= ~flag;
        }
    }

    public bool Visible
    {
        get => GetFlag(ItemImplFlag.Visible);
        set
        {
            if (GetFlag(ItemImplFlag.Visible) != value)
            {
                SetFlag(ItemImplFlag.Visible, value);
            }
        }
    }

    public bool Movable
    {
        get => GetFlag(ItemImplFlag.Movable);
        set
        {
            if (GetFlag(ItemImplFlag.Movable) != value)
            {
                SetFlag(ItemImplFlag.Movable, value);
            }
        }
    }

    public virtual TimeSpan DecayTime => DefaultDecayTime;
    public static TimeSpan DefaultDecayTime { get; set; } = TimeSpan.FromHours(1.0);

    public bool Stackable
    {
        get => GetFlag(ItemImplFlag.Stackable);
        set => SetFlag(ItemImplFlag.Stackable, value);
    }

    public virtual int LabelNumber
    {
        get
        {
            if (ItemId < 0x4000)
            {
                return 1020000 + ItemId;
            }

            return 1078872 + ItemId;
        }
    }

    public virtual double DefaultWeight
    {
        get
        {
            if (ItemId < 0 || ItemId > TileData.MaxItemValue)
            {
                return 0;
            }

            var weight = TileData.ItemTable[ItemId].Weight;

            if (weight is 255 or 0)
            {
                weight = 1;
            }

            return weight;
        }
    }
}
