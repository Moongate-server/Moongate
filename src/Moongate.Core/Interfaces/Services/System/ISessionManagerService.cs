using Moongate.Core.Data.Sessions;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface ISessionManagerService : IMoongateService
{
    SessionData? GetSession(string sessionId, bool throwIfNotFound = true);

}
