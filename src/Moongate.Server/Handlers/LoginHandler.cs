using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Packets;

namespace Moongate.Server.Handlers;

public class LoginHandler : IPacketListener
{
    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is LoginPacket loginPacket)
        {
        }

        if (packet is SeedPacket seedPacket)
        {

        }
    }
}
