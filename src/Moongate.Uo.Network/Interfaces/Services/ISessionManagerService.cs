using Moongate.Core.Interfaces.Services.Base;
using Moongate.Uo.Network.Data.Sessions;

namespace Moongate.Uo.Network.Interfaces.Services;

public interface ISessionManagerService : IMoongateService
{
    SessionData? GetSession(string sessionId, bool throwIfNotFound = true);
    SessionData CreateSession(string sessionId);
    void DeleteSession(string sessionId);

    List<SessionData> QuerySessions(Func<SessionData, bool> predicate);
}
