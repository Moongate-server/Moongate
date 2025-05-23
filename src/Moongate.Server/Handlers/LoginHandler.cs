using Moongate.Uo.Data;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Packets;
using Moongate.Uo.Network.Types;
using Serilog;

namespace Moongate.Server.Handlers;

public class LoginHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<LoginHandler>();

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is LoginPacket loginPacket)
        {
            Login(session, loginPacket);
        }

        if (packet is SeedPacket seedPacket)
        {
            SetSeedVersion(session, seedPacket);
        }
    }

    private void Login(SessionData session, LoginPacket loginPacket)
    {
        session.SendPacket(new LoginDeniedPacket(LoginDeniedReasonType.AccountBlocked));
    }

    private void SetSeedVersion(SessionData session, SeedPacket seedPacket)
    {
        _logger.Debug("Client {Session} connected with version {Version}", session.Id, seedPacket.ToString());
        //TODO: Check if the version is supported by the server

        session.SetClientVersion(
            new ClientVersion(seedPacket.Major, seedPacket.Minor, seedPacket.Revision, seedPacket.Prototype)
        );
    }
}
