using Moongate.Core.Data.Configs.Server.Sections;

namespace Moongate.Core.Data.Configs.Server;

public class MoongateServerConfig
{
    public ShardConfig Shard { get; set; } = new();
    public NetworkConfig Network { get; set; } = new();

    public WebServerConfig WebServer { get; set; } = new();
}
