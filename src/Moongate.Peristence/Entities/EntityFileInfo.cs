namespace Moongate.Persistence.Entities;

/// <summary>
///     Information about a binary persistence file
/// </summary>
public class EntityFileInfo
{
    /// <summary>
    ///     Full path to the file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    ///     File format version
    /// </summary>
    public uint Version { get; set; }

    /// <summary>
    ///     File size in bytes
    /// </summary>
    public ulong FileSize { get; set; }

    /// <summary>
    ///     When the file was created
    /// </summary>
    public DateTimeOffset CreatedTimestamp { get; set; }

    /// <summary>
    ///     When the file was last modified
    /// </summary>
    public DateTimeOffset ModifiedTimestamp { get; set; }

    /// <summary>
    ///     Count of entities by type
    /// </summary>
    public Dictionary<Type, uint> EntityCounts { get; set; } = new();

    /// <summary>
    ///     Total number of entities in the file
    /// </summary>
    public uint TotalEntityCount => (uint)EntityCounts.Values.Sum(x => x);
}
