using Moongate.Core.Data.Sessions;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Uo.Network.Interfaces.Services;

public interface ISessionManagerService : IMoongateService
{
    SessionData? GetSession(string sessionId, bool throwIfNotFound = true);
    SessionData CreateSession(string sessionId);

    void DeleteSession(string sessionId);

}
