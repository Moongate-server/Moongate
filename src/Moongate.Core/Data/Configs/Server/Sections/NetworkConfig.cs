namespace Moongate.Core.Data.Configs.Server.Sections;

public class NetworkConfig
{
    public int LoginPort { get; set; } = 2593;
    public int GamePort { get; set; } = 2594;
    public bool IsPingServerEnabled { get; set; } = true;

    public bool LogPackets { get; set; } = false;
}
