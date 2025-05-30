namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.Types;

/// <summary>
/// All supported subcommand types for General Information packet
/// </summary>
public enum SubcommandType : ushort
{
    Invalid = 0x00,

    // Fast Walk Prevention
    InitializeFastWalkPrevention = 0x01,
    AddKeyToFastWalkStack = 0x02,

    // Gump Management
    CloseGenericGump = 0x04,

    // Client Information
    ScreenSize = 0x05,

    // Party System
    PartySystem = 0x06,

    // Map and Cursor
    SetCursorHueSetMap = 0x08,

    // Combat
    WrestlingStun = 0x0A,

    // Localization
    ClientLanguage = 0x0B,

    // Status
    ClosedStatusGump = 0x0C,

    // 3D Client
    Client3DAction = 0x0E,

    // Client Type
    ClientType = 0x0F,

    // Mega Cliloc
    MegaClilocUnknown = 0x10,

    // Popup Menus
    RequestPopupMenu = 0x13,
    DisplayPopupMenu = 0x14,
    PopupEntrySelection = 0x15,

    // UI Management
    CloseUserInterfaceWindows = 0x16,

    // Codex
    CodexOfWisdom = 0x17,

    // Map Differences
    EnableMapDiff = 0x18,

    // Extended Stats
    ExtendedStats = 0x19,
    ExtendedStatsChange = 0x1A,

    // Spellbook
    NewSpellbook = 0x1B,
    SpellSelected = 0x1C,

    // Custom Housing
    SendHouseRevisionState = 0x1D,
    RequestHouseRevision = 0x1E,
    CustomHousing = 0x20,

    // Abilities
    AbilityIconConfirm = 0x21,

    // Damage
    Damage = 0x22,

    // Unknown
    Unknown = 0x24,

    // SE Abilities
    SEAbilityChange = 0x25,

    // Mount Speed
    MountSpeed = 0x26,

    // Race Change
    ChangeRace = 0x2A,

    // Targeted Actions
    UseTargetedItem = 0x2C,
    CastTargetedSpell = 0x2D,
    UseTargetedSkill = 0x2E,

    // KR House Menu
    KRHouseMenuGump = 0x2F,

    // Gargoyle Flying
    ToggleGargoyleFlying = 0x32
}
