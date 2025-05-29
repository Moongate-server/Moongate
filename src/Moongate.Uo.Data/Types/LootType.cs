namespace Moongate.Uo.Data.Types;

public enum LootType : byte
{
    /// <summary>
    ///     Stealable. Lootable.
    /// </summary>
    Regular = 0,

    /// <summary>
    ///     Unstealable. Unlootable, unless owned by a murderer.
    /// </summary>
    Newbied = 1,

    /// <summary>
    ///     Unstealable. Unlootable, always.
    /// </summary>
    Blessed = 2,

    /// <summary>
    ///     Stealable. Lootable, always.
    /// </summary>
    Cursed = 3
}
