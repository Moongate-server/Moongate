using MemoryPack;

namespace Moongate.Persistence.Entities;

/// <summary>
/// Table of Contents entry
/// </summary>
[MemoryPackable]
public partial struct TocEntry
{
    /// <summary>
    /// Entity type identifier
    /// </summary>
    public byte EntityType { get; set; }

    /// <summary>
    /// Number of entities of this type
    /// </summary>
    public uint Count { get; set; }

    /// <summary>
    /// Offset to first entity of this type
    /// </summary>
    public ulong Offset { get; set; }

    /// <summary>
    /// Total size of all entities of this type
    /// </summary>
    public ulong TotalSize { get; set; }
}
