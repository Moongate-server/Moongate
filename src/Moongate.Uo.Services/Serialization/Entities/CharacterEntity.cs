using MemoryPack;
using Moongate.Core.Data.Ids;
using Moongate.Persistence.Attributes;
using NanoidDotNet;

namespace Moongate.Uo.Services.Serialization.Entities;

[MemoryPackable]
[EntityType(0x02, Description = "Character Entity")]
public partial class CharacterEntity
{
    public string AccountId { get; set; }

    public int Slot { get; set; }

    public string Name { get; set; }

    public Serial MobileId { get; set; }
}
