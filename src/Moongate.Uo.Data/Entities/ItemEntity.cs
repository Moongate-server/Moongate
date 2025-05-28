using MemoryPack;
using Moongate.Core.Data.Ids;
using Moongate.Persistence.Attributes;
using Moongate.Uo.Data.Interfaces.Entities;

namespace Moongate.Uo.Data.Entities;

[MemoryPackable]
[EntityType(0x03, Description = "Item Entity")]
public partial class ItemEntity
{
    public Serial Serial { get; set; }

    public int ModelId { get; set; }

    public int Hue { get; set; }
}
