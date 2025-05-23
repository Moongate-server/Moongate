using MemoryPack;
using Moongate.Persistence.Attributes;
using NanoidDotNet;

namespace Moongate.Uo.Services.Serialization.Entities;

[MemoryPackable]
[EntityType(0x02, Description = "Character Entity")]
public partial class CharacterEntity
{
    public string AccountId { get; set; }

    public string CharaterName { get; set; }
}
