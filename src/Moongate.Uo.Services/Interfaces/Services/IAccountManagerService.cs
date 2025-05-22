using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Uo.Services.Interfaces.Services;

public interface IAccountManagerService : IMoongateStartStopService
{
    Task<bool> CreateAccount(string username, string password);



}
