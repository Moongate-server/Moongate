namespace Moongate.Core.Network.Data;

public class MoonTcpServerOptions
{
    public int BufferSize { get; set; } = 8192;

    public int Backlog { get; set; } = 100;
}
