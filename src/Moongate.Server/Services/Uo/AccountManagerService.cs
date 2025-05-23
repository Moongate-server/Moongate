using Moongate.Core.Directories;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Services.Base;
using Moongate.Core.Types;
using Moongate.Core.Utils.Hash;
using Moongate.Persistence.Extensions;
using Moongate.Persistence.Interfaces.Services;
using Moongate.Uo.Services.Events.Accounts;
using Moongate.Uo.Services.Interfaces.Services;
using Moongate.Uo.Services.Serialization.Entities;
using Moongate.Uo.Services.Types;
using Serilog;
using ZLinq;

namespace Moongate.Server.Services.Uo;

public class AccountManagerService : AbstractBaseMoongateStartStopService, IAccountManagerService
{
    private readonly IPersistenceManager _persistenceManager;

    private readonly IEventBusService _eventBusService;

    private readonly Dictionary<string, AccountEntity> _accounts = new();
    private readonly Dictionary<string, List<CharacterEntity>> _characters = new();


    private readonly string _accountsFilePath;

    private readonly string _charactersFilePath;

    public AccountManagerService(
        DirectoriesConfig directoriesConfig, IPersistenceManager persistenceManager, IEventBusService eventBusService
    ) : base(
        Log.ForContext<AccountManagerService>()
    )
    {
        _persistenceManager = persistenceManager;
        _eventBusService = eventBusService;

        _accountsFilePath = Path.Combine(directoriesConfig[DirectoryType.Data], "accounts.moongate");
        _charactersFilePath = Path.Combine(directoriesConfig[DirectoryType.Data], "characters.moongate");
    }

    public async Task<bool> CreateAccount(
        string username, string password, bool isActive = true, AccountLevelType level = AccountLevelType.Player
    )
    {
        if (_accounts.ContainsKey(username))
        {
            Logger.Warning("Account with username {Username} already exists.", username);
            return false;
        }

        var account = new AccountEntity
        {
            Username = username,
            PasswordHash = HashUtils.CreatePassword(password),
            IsActive = isActive,
            AccountLevel = level
        };

        _accounts.Add(account.Id, account);

        await _eventBusService.PublishAsync(new AccountCreatedEvent(account.Id, username, level));

        return true;
    }

    public AccountEntity? Login(string username, string password)
    {
        var account = _accounts.Values.AsValueEnumerable()
            .Where(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        if (account != null)
        {
            if (HashUtils.VerifyPassword(password, account.PasswordHash))
            {
                Logger.Information("User {Username} logged in successfully.", username);
                return account;
            }

            Logger.Warning("Invalid password for user {Username}.", username);
        }
        else
        {
            Logger.Warning("User {Username} not found.", username);
        }

        return null;
    }


    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await LoadAccountsAsync(cancellationToken);
        await LoadCharactersAsync(cancellationToken);
    }

    private async Task LoadCharactersAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_charactersFilePath))
        {
            return;
        }

        var entities = await _persistenceManager.LoadEntitiesAsync(_charactersFilePath, cancellationToken);

        if (entities.TryGetValue(typeof(CharacterEntity), out var characterEntities))
        {
            foreach (var entity in characterEntities)
            {
                if (entity is CharacterEntity character)
                {
                    if (!_characters.TryGetValue(character.AccountId, out List<CharacterEntity>? value))
                    {
                        value = new List<CharacterEntity>();
                        _characters[character.AccountId] = value;
                    }

                    value.Add(character);
                }
            }

            Logger.Information("Loaded {Count} characters.", characterEntities.Count);
        }
    }

    private async Task LoadAccountsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_accountsFilePath))
        {
            Logger.Warning("No accounts file found. Creating default one.");
            Logger.Warning("Default username: admin, password: admin");

            CreateAccount("admin", "admin");

            await SaveAccountsAsync(cancellationToken);
            return;
        }

        var entities = await _persistenceManager.LoadEntitiesAsync(_accountsFilePath, cancellationToken);

        if (entities.TryGetValue(typeof(AccountEntity), out var accountEntities))
        {
            foreach (var entity in accountEntities)
            {
                if (entity is AccountEntity account)
                {
                    _accounts.Add(account.Id, account);
                }
            }

            Logger.Information("Loaded {Count} accounts.", accountEntities.Count);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private Task SaveAccountsAsync(CancellationToken cancellationToken)
    {
        return _persistenceManager.SaveEntitiesAsync(_accountsFilePath, _accounts.Values, cancellationToken);
    }

    public void Dispose()
    {
    }
}
