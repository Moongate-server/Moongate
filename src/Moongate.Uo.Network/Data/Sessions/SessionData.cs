using Moongate.Core.Network.Servers.Tcp;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Middlewares;

namespace Moongate.Uo.Network.Data.Sessions;

public class SessionData : IDisposable
{
    public delegate void PacketSendDelegate(NetClient client, IUoNetworkPacket packet);

    private readonly OutgoingCompressionMiddleware _outgoingCompressionMiddleware = new();

    public event PacketSendDelegate OnSendPacket;

    private readonly Dictionary<string, object> _sessionData = new();

    public string Id { get; set; }

    public NetClient Client { get; set; }


    public void EnableCompression()
    {
        Client.AddMiddleware(_outgoingCompressionMiddleware);
    }

    public void DisableCompression()
    {
        Client.RemoveMiddleware(_outgoingCompressionMiddleware);
    }

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
