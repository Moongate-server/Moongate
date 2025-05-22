using Moongate.Core.Interfaces.Services.Base;
using Moongate.Uo.Services.Serialization.Entities;

namespace Moongate.Uo.Services.Interfaces.Services;

public interface IAccountManagerService : IMoongateStartStopService
{
    Task<bool> CreateAccount(string username, string password);
    AccountEntity? Login(string username, string password);
}
