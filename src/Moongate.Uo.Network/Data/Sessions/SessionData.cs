using Moongate.Core.Network.Servers.Tcp;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Data.Sessions;

public class SessionData : IDisposable
{
    public delegate void PacketSendDelegate(NetClient client, IUoNetworkPacket packet);

    public event PacketSendDelegate OnSendPacket;

    private readonly Dictionary<string, object> _sessionData = new();

    public string Id { get; set; }

    public NetClient Client { get; set; }


    public void AddData<TEntity>(TEntity entity, string name = "default")
    {
        _sessionData[name] = entity;
    }

    public TEntity GetData<TEntity>(string name = "default")
    {
        if (_sessionData.TryGetValue(name, out var value))
        {
            return (TEntity)value;
        }

        return default;
    }

    public void SendPacket(IUoNetworkPacket packet)
    {
        OnSendPacket?.Invoke(Client, packet);
    }

    public void Dispose()
    {
        _sessionData.Clear();
    }
}
