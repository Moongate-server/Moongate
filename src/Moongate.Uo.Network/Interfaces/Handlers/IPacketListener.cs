using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Interfaces.Handlers;

public interface IPacketListener
{
    Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet);

}
