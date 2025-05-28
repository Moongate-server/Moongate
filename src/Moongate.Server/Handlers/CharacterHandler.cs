using Moongate.Core.Data.Ids;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Data.Network.Packets.Characters;
using Moongate.Uo.Data.Network.Packets.Data;
using Moongate.Uo.Data.Network.Packets.Flags;
using Moongate.Uo.Data.Network.Packets.Login;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Moongate.Uo.Network.Packets.Connection;
using Moongate.Uo.Network.Types;
using Moongate.Uo.Services.Events.Characters;
using Moongate.Uo.Services.Interfaces.Services;
using Moongate.Uo.Services.Serialization.Entities;
using Serilog;

namespace Moongate.Server.Handlers;

public class CharacterHandler : IPacketListener
{
    private readonly ILogger _logger = Log.ForContext<CharacterHandler>();

    private readonly ISessionManagerService _sessionManagerService;

    private readonly IMobileService _mobileService;
    private readonly IAccountManagerService _accountManagerService;

    private readonly IMapService _mapService;


    public CharacterHandler(
        IEventBusService eventBusService, ISessionManagerService sessionManagerService, IMapService mapService,
        IAccountManagerService accountManagerService, IMobileService mobileService
    )
    {
        _sessionManagerService = sessionManagerService;
        this._mapService = mapService;
        _accountManagerService = accountManagerService;
        _mobileService = mobileService;
        _mapService = mapService;

        eventBusService.Subscribe<SendCharacterListEvent>(OnSendCharacterListEvent);
    }

    public async Task OnPacketReceivedAsync(SessionData session, IUoNetworkPacket packet)
    {
        if (packet is CharacterCreationPacket characterCreation)
        {
            await ProcessCharacterCreation(session, characterCreation);
        }

        if (packet is CharacterSelectPacket characterSelect)
        {
            await ProcessCharacterSelect(session, characterSelect);
        }
    }

    private async Task ProcessCharacterSelect(SessionData session, CharacterSelectPacket packet)
    {

        _logger.Debug("Processing character select for {CharacterName} slot n: {Slot}", packet.Name, packet.Slot);

        var character = _accountManagerService.GetCharactersByAccountId(session.AccountId)
            .FirstOrDefault(c => c.Slot == packet.Slot);

        // NOTE: This is guard
        if (character == null)
        {
            _logger.Warning("Character with slot {Slot} not found for account {AccountId}", packet.Slot, session.AccountId);
            session.SendPacket(new LoginDeniedPacket(LoginDeniedReasonType.AccountBlocked));
            session.Disconnect();
            return;
        }

        var mobile = _mobileService.GetMobileBySerial(character.MobileId);

        session.SetMobile(mobile);

        session.SendPacket(new ClientVersionPacket());
        session.SendPacket(new LoginConfirmPacket());
    }

    private async Task ProcessCharacterCreation(SessionData session, CharacterCreationPacket packet)
    {
        _logger.Debug("Processing character creation");

        var mobile = _mobileService.CreateMobile();

        var startingLocation = _mapService.GetStartingCities()[packet.StartingLocation];

        mobile.Name = packet.Name;
        mobile.Dexterity = packet.Dex;
        mobile.Strength = packet.Str;
        mobile.Intelligence = packet.Int;
        mobile.Race = packet.Race;
        mobile.Profession = packet.Profession;
        mobile.Hue = packet.Hue;
        mobile.Map = startingLocation.Map;
        mobile.Location = startingLocation.Location;

        var characterEntity = new CharacterEntity()
        {
            Slot = packet.Slot,
            AccountId = session.AccountId,
            MobileId = mobile.Serial,
            Name = packet.Name
        };
        await _accountManagerService.AddCharacterToAccountAsync(session.AccountId, characterEntity);

        // In production we need to schedule the save to the database
        await _mobileService.SaveAsync(CancellationToken.None);
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
