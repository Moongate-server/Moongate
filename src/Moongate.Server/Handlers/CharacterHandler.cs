using Moongate.Core.Interfaces.Services.System;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Data.Network.Packets.Characters;
using Moongate.Uo.Data.Network.Packets.Data;
using Moongate.Uo.Data.Network.Packets.Flags;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Moongate.Uo.Services.Events.Characters;
using Moongate.Uo.Services.Interfaces.Services;
using Moongate.Uo.Services.Serialization.Entities;
using Serilog;

namespace Moongate.Server.Handlers;

public class CharacterHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<CharacterHandler>();

    private readonly ISessionManagerService _sessionManagerService;

    private readonly IAccountManagerService _accountManagerService;

    private readonly IMapService _mapService;


    public CharacterHandler(
        IEventBusService eventBusService, ISessionManagerService sessionManagerService, IMapService mapService,
        IAccountManagerService accountManagerService
    )
    {
        _sessionManagerService = sessionManagerService;
        _accountManagerService = accountManagerService;
        _mapService = mapService;

        eventBusService.Subscribe<SendCharacterListEvent>(OnSendCharacterListEvent);
    }

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is CharacterCreationPacket characterCreation)
        {
            await ProcessCharacterCreation(session, characterCreation);
        }
    }

    private async Task ProcessCharacterCreation(SessionData session, CharacterCreationPacket packet)
    {
        _logger.Debug("Processing character creation");
        var characterEntity = new CharacterEntity()
        {
            AccountId = session.AccountId,
            MobileId = Random.Shared.Next(),
            Name = packet.Name
        };
        await _accountManagerService.AddCharacterToAccountAsync(session.AccountId, characterEntity);
    }

    private async Task OnSendCharacterListEvent(SendCharacterListEvent @event)
    {
        var session = _sessionManagerService.GetSession(@event.SessionId);

        session.EnableCompression();

        var packet = new CharactersStartingLocationsPacket(session.GetClientVersion().ProtocolChanges);
        packet.Cities.AddRange(_mapService.GetStartingCities());
        packet.FillCharacters();

        var userCharacters = _accountManagerService.GetCharactersByAccountId(session.AccountId);

        for (var i = 0; i < userCharacters.ToList().Count; i++)
        {
            var character = userCharacters.ToList()[i];


            if (character != null)
            {
                packet.Characters[i] = new CharacterEntry(character.Name);
            }
            else
            {
                _logger.Warning("Character {CharacterId} not found", character.MobileId);
            }
        }

        session.SendPacket(packet);
        session.SendPacket(new FeatureFlagsResponse(UoContext.ExpansionInfo.SupportedFeatures));
    }
}
