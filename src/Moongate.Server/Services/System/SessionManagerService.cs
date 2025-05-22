using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;
using Moongate.Core.Data.Sessions;
using Moongate.Core.Services.Base;
using Moongate.Uo.Network.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Services.System;

public class SessionManagerService : AbstractBaseMoongateService, ISessionManagerService
{
    private readonly ObjectPool<SessionData> _sessionPool = ObjectPool.Create(
        new DefaultPooledObjectPolicy<SessionData>()
    );

    private readonly ConcurrentDictionary<string, SessionData> _sessionData = new();

    public SessionManagerService() : base(Log.ForContext<SessionManagerService>())
    {
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

    public SessionData CreateSession(string sessionId)
    {
        if (_sessionData.ContainsKey(sessionId))
        {
            throw new InvalidOperationException($"Session with ID {sessionId} already exists.");
        }

        var session = _sessionPool.Get();
        _sessionData[sessionId] = session;

        Logger.Information("Created session: {SessionId}", sessionId);

        return session;
    }

    public void DeleteSession(string sessionId)
    {
        if (_sessionData.TryRemove(sessionId, out var session))
        {
            session.Dispose();
            _sessionPool.Return(session);
            Logger.Information("Deleted session: {SessionId}", sessionId);
        }
        else
        {
            Logger.Warning("Session not found for deletion: {SessionId}", sessionId);
        }
    }

    public void Dispose()
    {
        _sessionData.Clear();
    }
}
