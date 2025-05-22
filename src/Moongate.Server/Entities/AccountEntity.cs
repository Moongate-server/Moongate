using MemoryPack;
using Moongate.Persistence.Attributes;

namespace Moongate.Server.Entities;


[MemoryPackable]
[EntityType(0x01, Description = "Account Entity")]
public partial class AccountEntity
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

}
