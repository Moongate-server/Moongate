using Moongate.Core.Interfaces.Services.System;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Moongate.Uo.Services.Events.Characters;
using Moongate.Uo.Services.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Handlers;

public class CharacterHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<CharacterHandler>();

    private readonly ISessionManagerService _sessionManagerService;

    private readonly IMapService _mapService;


    public CharacterHandler(IEventBusService eventBusService, ISessionManagerService sessionManagerService, IMapService mapService)
    {
        _sessionManagerService = sessionManagerService;
        _mapService = mapService;

        eventBusService.Subscribe<SendCharacterListEvent>(OnSendCharacterListEvent);
    }

    private async Task OnSendCharacterListEvent(SendCharacterListEvent @event)
    {
        var session = _sessionManagerService.GetSession(@event.SessionId);
    }

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        session.EnableCompression();
    }
}
