using Moongate.Core.Data.Ids;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Data.Network.Packets.Characters;
using Moongate.Uo.Data.Network.Packets.Chat;
using Moongate.Uo.Data.Network.Packets.Data;
using Moongate.Uo.Data.Network.Packets.Features;
using Moongate.Uo.Data.Network.Packets.Flags;
using Moongate.Uo.Data.Network.Packets.Login;
using Moongate.Uo.Data.Network.Packets.Players;
using Moongate.Uo.Data.Network.Packets.Seasons;
using Moongate.Uo.Data.Network.Packets.World;
using Moongate.Uo.Data.Types;
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

    private readonly ITimerService _timerService;


    private readonly IMapService _mapService;


    public CharacterHandler(
        IEventBusService eventBusService, ISessionManagerService sessionManagerService, IMapService mapService,
        IAccountManagerService accountManagerService, IMobileService mobileService, ITimerService timerService
    )
    {
        _sessionManagerService = sessionManagerService;
        _accountManagerService = accountManagerService;
        _mobileService = mobileService;
        _timerService = timerService;
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
        _logger.Debug(
            "Processing character select for {CharacterName} slot n: {Slot} (Serial: {Serial})",
            packet.Name,
            packet.Slot,
            mobile.Serial
        );


        session.SetMobile(mobile);


        session.SendPacket(new ClientVersionPacket());

        await Task.Delay(64);
        session.SendPacket(new LoginConfirmPacket(mobile));

        //session.SendPacket(new MobileIncomingPacket(mobile));
        //session.SendPacket(new SupportedFeaturesPacket(session));
        session.SendPacket(new CharacterWarModePacket());


        session.SendPacket(new LoginCompletePacket());
        session.SendPacket(new MapChangePacket(mobile.Map));
        session.SendPacket(new OverallLightLevelPacket(LightLevelType.Day));
        session.SendPacket(new PersonalLightLevelPacket(mobile, LightLevelType.Day));
        session.SendPacket(new SeasonPacket(mobile.Map.Season, true));


        _timerService.RegisterTimer(
            "test_broadcast",
            1000,
            () =>
            {
                var broadcastPacket = new UnicodeSpeechPacket()
                {
                    Serial = Serial.MinusOne,
                    Graphic = -1,
                    Type = SpeechType.Command,
                    Hue = SpeechHues.System,
                    Language = "ENU",
                    Text = "Welcome to Moongate!",
                    IsUnicode = true,
                    Name = "System",
                    Font = 3
                };

                session.SendPacket(broadcastPacket);
            },
            1000,
            true
        );


        // session.SendPacket(new MobileUpdatePacket(mobile));
        // session.SendPacket(new LoginCompletePacket());
        // session.SendPacket(new MapChangePacket(mobile.Map));
        //
        // session.SendPacket(new MobileIncomingPacket(mobile));
        // session.SendPacket(new SupportedFeaturesPacket(session));
        // session.SendPacket(new CharacterWarModePacket());
        //
        //
        // session.SendPacket(new OverallLightLevelPacket(LightLevelType.Day));
        // session.SendPacket(new PersonalLightLevelPacket(mobile, LightLevelType.Day));
        //
        // session.SendPacket(new MapChangePacket(mobile.Map));
        //  session.SendPacket(new LoginConfirmPacket(mobile));
        //  session.SendPacket(new MapPatchesPacket(Map.Maps));
        //  session.SendPacket(new MapChangePacket(mobile.Map));
        //  session.SendPacket(new MobileIncomingPacket(mobile));

        // session.SendPacket(new MobileUpdatePacket(mobile));
        //
        // session.SendPacket(new UpdateStatusBarPacket(mobile));
        //
        //
        // session.SendPacket(new SeasonPacket(mobile.Map.Season, true));
        //
        // session.SendPacket(new SupportedFeaturesPacket(session));
        //
        // session.SendPacket(new SetMusicPacket(MusicName.BTCastle));
        // session.SendPacket(new MobileUpdatePacket(mobile));
        //
        // session.SendPacket(new OverallLightLevelPacket(LightLevelType.Day));
        // session.SendPacket(new PersonalLightLevelPacket(mobile, LightLevelType.Day));
        //
        // session.SendPacket(new MobileUpdatePacket(mobile));
        //
        // // TODO: Refactor SendMobileIncoming
        // session.SendPacket(new MobileIncomingPacket(mobile));
        //
        // //TODO: Missing  CreateMobileStatus
        //
        // session.SendPacket(new MobileUpdatePacket(mobile));
        //
        // session.SendPacket(new CharacterWarModePacket());
        // session.SendPacket(new SupportedFeaturesPacket(session));
        //
        // session.SendPacket(new MobileUpdatePacket(mobile));
        // session.SendPacket(new CharacterWarModePacket());
        // session.SendPacket(new MobileIncomingPacket(mobile));
        //
        // session.SendPacket(new LoginCompletePacket());
        //
        // session.SendPacket(new CurrentTimePacket());
        // session.SendPacket(new SeasonPacket(Season.Spring, true));
        //
        // session.SendPacket(new MapChangePacket(mobile.Map));
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
        mobile.Alive = true;
        mobile.Female = packet.IsFemale;
        mobile.ClientFlags = packet.ClientFlags;

        session.SetClientFlags(packet.ClientFlags);


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
        var session = @event.Session;

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
