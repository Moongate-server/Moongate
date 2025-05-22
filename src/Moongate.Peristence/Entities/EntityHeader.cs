using MemoryPack;

namespace Moongate.Persistence.Entities;

/// <summary>
/// Header structure for each entity in the binary file
/// </summary>
[MemoryPackable]
public partial struct EntityHeader
{
    /// <summary>
    /// Type identifier for the entity (0x1, 0x2, etc.)
    /// </summary>
    public byte EntityType { get; set; }

    /// <summary>
    /// Size of the entity data in bytes
    /// </summary>
    public uint DataSize { get; set; }

    /// <summary>
    /// CRC32 checksum of the entity data
    /// </summary>
    public uint Checksum { get; set; }

    /// <summary>
    /// Reserved bytes for future use
    /// </summary>
    public uint Reserved { get; set; }
}
