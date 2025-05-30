using Moongate.Core.Interfaces.Services.System;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Moongate.Uo.Network.Packets;
using Moongate.Uo.Network.Packets.Connection;
using Moongate.Uo.Services.Events.Characters;
using Serilog;

namespace Moongate.Server.Handlers;

public class GameLoginHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<GameLoginHandler>();


    private readonly INetworkService _networkService;
    private readonly ISessionManagerService _sessionManagerService;

    private readonly IEventBusService _eventBusService;

    public GameLoginHandler(
        INetworkService networkService, ISessionManagerService sessionManagerService, IEventBusService eventBusService
    )
    {
        _networkService = networkService;
        _sessionManagerService = sessionManagerService;
        _eventBusService = eventBusService;
    }

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is GameServerLoginPacket gameServerLoginPacket)
        {
            /// NOTE: Server is too fast, and we need to check if the session is in limbo or is session is already connected

            var limboSession = _networkService.GetInLimboSession(gameServerLoginPacket.AuthId) ?? _sessionManagerService
                .QuerySessions(data => data.AuthId == gameServerLoginPacket.AuthId)
                .FirstOrDefault();


            session.AuthId = gameServerLoginPacket.AuthId;
            session.AccountId = limboSession?.AccountId;
            session.CloneDataFrom(limboSession);


            _logger.Information("Game server login: {SessionId} - {Username}", session.Id, gameServerLoginPacket.Sid);

            await _eventBusService.PublishAsync(new SendCharacterListEvent(session));


        }
    }
}
