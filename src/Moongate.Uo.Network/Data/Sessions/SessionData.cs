using Moongate.Core.Network.Servers.Tcp;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Middlewares;

namespace Moongate.Uo.Network.Data.Sessions;

public class SessionData : IDisposable
{
    public delegate void PacketSendDelegate(NetClient client, IUoNetworkPacket packet);

    private readonly OutgoingCompressionMiddleware _outgoingCompressionMiddleware = new();

    /// <summary>
    ///  Indicates if the session when disconnected should be put in limbo.
    ///  (This is used for the login server to keep the session and information until the client connect to game server).
    /// </summary>
    public bool PutInLimbo { get; set; }

    public event PacketSendDelegate OnSendPacket;


    private readonly Dictionary<string, object> _sessionData = new();

    public string Id { get; set; }

    public int AuthId { get; set; } = -1;
    public string AccountId { get; set; }
    public NetClient? Client { get; set; }


    public void EnableCompression()
    {
        if (!Client.ContainsMiddleware(typeof(OutgoingCompressionMiddleware)))
        {
            Client.AddMiddleware(_outgoingCompressionMiddleware);
            Client.HaveCompression = true;
        }
    }

    public void DisableCompression()
    {
        Client.RemoveMiddleware(_outgoingCompressionMiddleware);
        Client.HaveCompression = false;
    }

    public void SetData<TEntity>(TEntity entity, string? name = null)
    {
        var uniqueName = name ?? $"default_{typeof(TEntity).Name}";
        _sessionData[uniqueName] = entity;
    }

    public void Disconnect()
    {
        Client.Disconnect();
    }

    public TEntity? GetData<TEntity>(string? name = null)
    {
        var uniqueName = name ?? $"default_{typeof(TEntity).Name}";
        if (_sessionData.TryGetValue(uniqueName, out var value))
        {
            return (TEntity)value;
        }

        return default;
    }

    public void SendPacket(IUoNetworkPacket packet)
    {
        OnSendPacket?.Invoke(Client, packet);
    }

    public void CloneDataFrom(SessionData source)
    {
        ArgumentNullException.ThrowIfNull(source);


        _sessionData.Clear();
        foreach (var kvp in source._sessionData)
        {
            _sessionData[kvp.Key] = kvp.Value;
        }
    }

    public void Clear()
    {
        if (!PutInLimbo)
        {
            Id = string.Empty;
            _sessionData.Clear();
            AuthId = -1;
            AccountId = string.Empty;
            Client = null;
            PutInLimbo = false;
        }
    }

    public override string ToString()
    {
        return $"SessionData: {Id} - AccountId: {AccountId} - AuthId: {AuthId}";
    }

    public void Dispose()
    {
        Clear();
    }
}
