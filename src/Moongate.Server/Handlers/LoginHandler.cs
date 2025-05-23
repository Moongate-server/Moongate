using Moongate.Core.Instances;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Packets;
using Moongate.Uo.Network.Types;
using Moongate.Uo.Services.Events.Accounts;
using Moongate.Uo.Services.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Handlers;

public class LoginHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<LoginHandler>();

    private readonly IAccountManagerService _accountManagerService;

    public LoginHandler(IAccountManagerService accountManagerService)
    {
        _accountManagerService = accountManagerService;
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
    }

    private void SetSeedVersion(SessionData session, SeedPacket seedPacket)
    {
        _logger.Debug("Client {Session} connected with version {Version}", session.Id, seedPacket.ToString());


        //TODO: Check if the version is supported by the server

        session.SetClientVersion(
            new ClientVersion(seedPacket.Major, seedPacket.Minor, seedPacket.Revision, seedPacket.Prototype)
        );

        session.SetSeed(seedPacket.Seed);
    }
}
