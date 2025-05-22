using Moongate.Core.Directories;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Services.Base;
using Moongate.Core.Types;
using Moongate.Core.Utils.Hash;
using Moongate.Persistence.Extensions;
using Moongate.Persistence.Interfaces.Services;
using Moongate.Uo.Services.Interfaces.Services;
using Moongate.Uo.Services.Serialization.Entities;
using Serilog;

namespace Moongate.Server.Services.Uo;

public class AccountManagerService : AbstractBaseMoongateStartStopService, IAccountManagerService
{
    private readonly IPersistenceManager _persistenceManager;

    private readonly IEventBusService _eventBusService;

    private readonly Dictionary<string, AccountEntity> _accounts = new();

    private readonly string _accountsFilePath;

    public AccountManagerService(
        DirectoriesConfig directoriesConfig, IPersistenceManager persistenceManager, IEventBusService eventBusService
    ) : base(
        Log.ForContext<AccountManagerService>()
    )
    {
        _persistenceManager = persistenceManager;
        _eventBusService = eventBusService;

        _accountsFilePath = Path.Combine(directoriesConfig[DirectoryType.Data], "accounts.moongate");
    }

    public async Task<bool> CreateAccount(string username, string password)
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
        };

        _accounts.Add(account.Id, account);

        await _eventBusService.PublishAsync(new AccountCreatedEvent(account.Id, username));

        return true;
    }

    public AccountEntity? Login(string username, string password)
    {
        if (_accounts.TryGetValue(username, out var account))
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
