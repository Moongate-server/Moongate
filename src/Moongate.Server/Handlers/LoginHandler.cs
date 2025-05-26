using System.Net;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Instances;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Network.Data.Entries;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Packets.Connection;
using Moongate.Uo.Network.Types;
using Moongate.Uo.Services.Events.Accounts;
using Moongate.Uo.Services.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Handlers;

public class LoginHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<LoginHandler>();

    private readonly IAccountManagerService _accountManagerService;

    private readonly MoongateServerConfig _moongateServerConfig;

    private readonly List<GameServerEntry> _gameServerEntries = new();

    public LoginHandler(IAccountManagerService accountManagerService, MoongateServerConfig moongateServerConfig)
    {
        _accountManagerService = accountManagerService;
        _moongateServerConfig = moongateServerConfig;

        _gameServerEntries.Add(
            new GameServerEntry()
            {
                IP = IPAddress.Parse("127.0.0.1"),
                LoadPercent = 0x0,
                Name = _moongateServerConfig.Shard.Name,
                TimeZone = (byte)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours
            }
        );
    }

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is LoginPacket loginPacket)
        {
            await Login(session, loginPacket);
        }

        if (packet is SeedPacket seedPacket)
        {
            SetSeedVersion(session, seedPacket);
        }

        if (packet is SelectServerPacket selectServerPacket)
        {
            await OnSelectServer(session, selectServerPacket);
        }

        if (packet is ClientVersionPacket clientVersionPacket)
        {
            // Drop packet,we already handled it in SetSeedVersion
        }
    }


    private async Task OnSelectServer(SessionData session, SelectServerPacket selectServerPacket)
    {
        var gameServer = _gameServerEntries[selectServerPacket.ShardId];
        _logger.Debug("User {Session} selected server {ServerId}", session.Id, gameServer.Name);
        session.AuthId = Random.Shared.Next();
        session.PutInLimbo = true;

        /// (From Prima project) 05/05/2025 --> Fixed connection bug after 4 days of testing, now i can die in peace! :D
        var connectToServer = new ConnectToGameServerPacket()
        {
            GameServerIP = gameServer.IP,
            GameServerPort = (ushort)_moongateServerConfig.Network.GamePort,
            SessionKey = session.AuthId,
        };

        session.SendPacket(connectToServer);
    }

    private async Task Login(SessionData session, LoginPacket loginPacket)
    {
        var account = _accountManagerService.Login(loginPacket.Username, loginPacket.Password);

        if (account == null)
        {
            _logger.Warning("Login failed for {Username}", loginPacket.Username);
            session.SendPacket(new LoginDeniedPacket(LoginDeniedReasonType.IncorrectPassword));
            return;
        }

        if (!account.IsActive)
        {
            _logger.Warning("Account {Username} is blocked", loginPacket.Username);
            session.SendPacket(new LoginDeniedPacket(LoginDeniedReasonType.AccountBlocked));
            return;
        }

        session.AccountId = account.Id;
        await MoongateInstanceHolder.PublishEvent(new AccountLoginEvent(account.Id, account.Username));

        _logger.Information("Login successful for {Username}", loginPacket.Username);

        session.SendPacket(PrepareGameServerList());
    }

    private GameServerListPacket PrepareGameServerList()
    {
        var gameServerList = new GameServerListPacket();

        foreach (var gameServer in _gameServerEntries)
        {
            gameServerList.AddServer(gameServer);
        }

        return gameServerList;
    }


    private void SetSeedVersion(SessionData session, SeedPacket seedPacket)
    {
        _logger.Debug("Client {Session} connected with version {Version}", session.Id, seedPacket.ToString());


        //TODO: Check if the version is supported by the server


        session.SetClientVersion(
            new ClientVersion(seedPacket.Major, seedPacket.Minor, seedPacket.Revision, seedPacket.Prototype)
        );

        if (UoContext.ServerVersion != session.GetClientVersion())
        {
            _logger.Warning(
                "Client {Session} connected with unsupported version {Version}, expected {ExpectedVersion}, disconnecting",
                session.Id,
                session.GetClientVersion(),
                UoContext.ServerVersion
            );
            session.SendPacket(new LoginDeniedPacket(LoginDeniedReasonType.IgrGeneralError));
            session.Disconnect();
            return;
        }

        session.SetSeed(seedPacket.Seed);
    }
}
