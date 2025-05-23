using MemoryPack;
using Moongate.Persistence.Attributes;
using Moongate.Uo.Services.Types;
using NanoidDotNet;

namespace Moongate.Uo.Services.Serialization.Entities;

[MemoryPackable]
[EntityType(0x01, Description = "Account Entity")]
public partial class AccountEntity
{
    public string Id { get; set; } = Nanoid.Generate();

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public AccountLevelType AccountLevel { get; set; } = AccountLevelType.Player;

    public bool IsActive { get; set; }
}
