using Moongate.Core.Data.Ids;
using Moongate.Core.Interfaces.Services.Base;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Services.Serialization.Entities;
using Moongate.Uo.Services.Types;

namespace Moongate.Uo.Services.Interfaces.Services;

public interface IAccountManagerService : IMoongateStartStopService
{
    Task<bool> CreateAccount(
        string username, string password, bool isActive = true, AccountLevelType level = AccountLevelType.Player
    );

    AccountEntity? Login(string username, string password);
    List<CharacterEntity> GetCharactersByAccountId(string accountId);

    Task AddCharacterToAccountAsync(string accountId, CharacterEntity character);

    Task SaveAsync(CancellationToken cancellationToken);
}
