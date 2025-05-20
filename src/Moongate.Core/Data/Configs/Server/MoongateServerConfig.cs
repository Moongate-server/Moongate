using Moongate.Core.Data.Configs.Server.Sections;

namespace Moongate.Core.Data.Configs.Server;

public class MoongateServerConfig
{
    public ShardConfig Shard { get; set; } = new();
}
