using Moongate.Uo.Data.Network.Packets.System;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Serilog;

namespace Moongate.Server.Handlers;

public class PingHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<PingHandler>();

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is PingMessagePacket pingPacket)
        {
            _logger.Verbose(
                "Received ping packet with sequence {Sequence} from session {SessionId}",
                pingPacket.Sequence,
                session.Id
            );

            session.SendPacket(pingPacket);
        }
    }
}
