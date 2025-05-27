namespace Moongate.Uo.Data.Types;

public enum SubcommandClientFlag : uint
{
    /// <summary>No special features</summary>
    None = 0x00000000,

    /// <summary>Third Dawn client (3D client support)</summary>
    ThirdDawn = 0x00000001,

    /// <summary>Post-AOS features (Age of Shadows)</summary>
    PostAOS = 0x00000002,

    /// <summary>Samurai Empire features</summary>
    SamuraiEmpire = 0x00000004,

    /// <summary>Mondain's Legacy features</summary>
    MondainsLegacy = 0x00000008,

    /// <summary>Ultima Online: ML client features</summary>
    UOMLClient = 0x00000010,

    /// <summary>Kingdom Reborn client</summary>
    KingdomReborn = 0x00000020,

    /// <summary>Stygian Abyss features</summary>
    StygianAbyss = 0x00000040,

    /// <summary>High Seas features</summary>
    HighSeas = 0x00000080,

    /// <summary>Gothic theme features</summary>
    Gothic = 0x00000100,

    /// <summary>Rustic theme features</summary>
    Rustic = 0x00000200,

    /// <summary>Jungle theme features</summary>
    Jungle = 0x00000400,

    /// <summary>Shadowguard theme features</summary>
    Shadowguard = 0x00000800,

    /// <summary>TOL (Time of Legends) features</summary>
    TOL = 0x00001000,

    /// <summary>EJ (Endless Journey) features</summary>
    EndlessJourney = 0x00002000,

    /// <summary>New Movement System</summary>
    NewMovementSystem = 0x00004000,

    /// <summary>Enhanced Client features</summary>
    EnhancedClient = 0x00008000,

    /// <summary>64-bit client support</summary>
    Client64Bit = 0x00010000,

    /// <summary>NewMovement extended features</summary>
    NewMovementExtended = 0x00020000,

    /// <summary>Veteran Rewards system</summary>
    VeteranRewards = 0x00040000,

    /// <summary>Custom housing features</summary>
    CustomHousing = 0x00080000
}
