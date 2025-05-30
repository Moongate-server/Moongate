using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;
using Moongate.Core.Services.Base;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Services;
using Serilog;
using ZLinq;

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

    public SessionData? GetSession(string sessionId, bool throwIfNotFound = true, bool waitForNetClient = false)
    {
        if (_sessionData.TryGetValue(sessionId, out var session))
        {
            if (waitForNetClient && session.Client == null)
            {
                /// FIXME: This is a blocking call, consider using async/await pattern
                SpinWait.SpinUntil(() => session.Client != null, TimeSpan.FromSeconds(2));

                if (session.Client == null)
                {
                    throw new InvalidOperationException(
                        $"Session with ID {sessionId} does not have a NetClient associated with it."
                    );
                }
            }

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
        session.Id = sessionId;
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

    public List<SessionData> QuerySessions(Func<SessionData, bool> predicate)
    {
        return _sessionData.Values.AsValueEnumerable()
            .Where(predicate)
            .ToList();
    }

    public void Dispose()
    {
        _sessionData.Clear();
    }
}
