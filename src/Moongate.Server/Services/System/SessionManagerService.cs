using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;
using Moongate.Core.Data.Sessions;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Core.Services.Base;
using Serilog;

namespace Moongate.Server.Services.System;

public class SessionManagerService : AbstractBaseMoongateService, ISessionManagerService
{
    private readonly INetworkService _networkService;

    private readonly ObjectPool<SessionData> _sessionPool = ObjectPool.Create(
        new DefaultPooledObjectPolicy<SessionData>()
    );

    private readonly ConcurrentDictionary<string, SessionData> _sessionData = new();


    public SessionManagerService(INetworkService networkService) : base(Log.ForContext<SessionManagerService>())
    {
        _networkService = networkService;
        _networkService.ClientConnected += OnClientConnected;
        _networkService.ClientDisconnected += OnClientDisconnected;
    }

    private void OnClientDisconnected(string serverId, string sessionId)
    {
        if (_sessionData.TryRemove(sessionId, out var session))
        {
            session.Dispose();
            _sessionPool.Return(session);
            Logger.Information("Removed session: {ServerId} {SessionId}", serverId, sessionId);
        }
        else
        {
            Logger.Warning("Session not found for disconnection: {SessionId}", sessionId);
        }
    }

    private void OnClientConnected(string serverId, string sessionId, NetClient client)
    {
        var session = _sessionPool.Get();

        _sessionData[sessionId] = session;

        Logger.Information("Added session: {ServerId} {SessionId}", serverId, sessionId);
    }


    public SessionData? GetSession(string sessionId, bool throwIfNotFound = true)
    {
        if (_sessionData.TryGetValue(sessionId, out var session))
        {
            return session;
        }

        if (throwIfNotFound)
        {
            throw new KeyNotFoundException($"Session with ID {sessionId} not found.");
        }

        return null;
    }

    public void Dispose()
    {
    }
}
