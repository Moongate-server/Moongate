using Moongate.Core.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Interfaces.Handlers;

public interface IPacketListener
{
    void OnPacketReceived(SessionData session, IUoNetworkPacket packet);

}
