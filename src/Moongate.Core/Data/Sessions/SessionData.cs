using Moongate.Core.Network.Servers.Tcp;

namespace Moongate.Core.Data.Sessions;

public class SessionData : IDisposable
{
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

    public void Dispose()
    {
        _sessionData.Clear();
    }
}
