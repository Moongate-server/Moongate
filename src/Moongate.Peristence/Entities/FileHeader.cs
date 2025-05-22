using MemoryPack;

namespace Moongate.Persistence.Entities;

/// <summary>
///     Main file header structure
/// </summary>
[MemoryPackable]
public partial struct FileHeader
{
    /// <summary>
    ///     Magic identifier "MOONGATE" (8 bytes)
    /// </summary>
    public ulong MagicNumber { get; set; }

    /// <summary>
    ///     File format version
    /// </summary>
    public uint Version { get; set; }

    /// <summary>
    ///     Total file size in bytes
    /// </summary>
    public ulong FileSize { get; set; }

    /// <summary>
    ///     CRC32 checksum of all data (excluding this field)
    /// </summary>
    public uint DataChecksum { get; set; }

    /// <summary>
    ///     Offset to Table of Contents
    /// </summary>
    public ulong TocOffset { get; set; }

    /// <summary>
    ///     Number of entries in TOC
    /// </summary>
    public uint TocEntryCount { get; set; }

    /// <summary>
    ///     Timestamp when file was created
    /// </summary>
    public long CreatedTimestamp { get; set; }

    /// <summary>
    ///     Timestamp when file was last modified
    /// </summary>
    public long ModifiedTimestamp { get; set; }
}
