using System.Security.Cryptography;
using MemoryPack;
using Moongate.Persistence.Entities;
using Moongate.Persistence.Interfaces.Services;
using Moongate.Persistence.Utils;
using Serilog;
using static System.GC;

namespace Moongate.Persistence.Services;

public class MemoryPackPersistenceManager : IPersistenceManager
{
    private readonly ILogger _logger = Log.ForContext<MemoryPackPersistenceManager>();

    /// <summary>
    /// Magic number representing "MOONGATE" in binary
    /// </summary>
    private static readonly ulong MagicNumber = BitConverter.ToUInt64("MOONGATE"u8);

    private const uint CurrentFileVersion = 1;

    /// <summary>
    /// Saves entities to a binary file with the specified format
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="entities">Dictionary of entity lists grouped by type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException">Thrown when filePath or entities is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when entity types are not registered</exception>
    public async Task SaveEntitiesAsync(
        string filePath, IDictionary<Type, IList<object>> entities, CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(entities);

        _logger.Information("Saving {EntityCount} entity types to {FilePath}", entities.Count, filePath);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 64 * 1024
        );

        // Write placeholder header (will be updated later)
        var header = new FileHeader
        {
            MagicNumber = MagicNumber,
            Version = CurrentFileVersion,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ModifiedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var headerBytes = MemoryPackSerializer.Serialize(header);
        await fileStream.WriteAsync(headerBytes, cancellationToken);

        var dataStartOffset = fileStream.Position;
        var tocEntries = new List<TocEntry>();
        var dataChecksum = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        try
        {
            // Write entity data and build TOC
            foreach (var (entityType, entityList) in entities)
            {
                if (entityList.Count == 0) continue;

                var entityId = EntityTypeRegistry.GetEntityId(entityType);
                var tocEntry = new TocEntry
                {
                    EntityType = entityId,
                    Count = (uint)entityList.Count,
                    Offset = (ulong)fileStream.Position
                };

                var totalEntitySize = 0UL;

                // Write entities of this type
                foreach (var entity in entityList)
                {
                    var entityBytes = MemoryPackSerializer.Serialize(entityType, entity);
                    var entityChecksum = CRC32.Compute(entityBytes);

                    var entityHeader = new EntityHeader
                    {
                        EntityType = entityId,
                        DataSize = (uint)entityBytes.Length,
                        Checksum = entityChecksum
                    };

                    var entityHeaderBytes = MemoryPackSerializer.Serialize(entityHeader);

                    await fileStream.WriteAsync(entityHeaderBytes, cancellationToken);
                    await fileStream.WriteAsync(entityBytes, cancellationToken);

                    dataChecksum.AppendData(entityHeaderBytes);
                    dataChecksum.AppendData(entityBytes);

                    totalEntitySize += (ulong)(entityHeaderBytes.Length + entityBytes.Length);
                }

                tocEntry.TotalSize = totalEntitySize;
                tocEntries.Add(tocEntry);

                _logger.Debug(
                    "Written {Count} entities of type {EntityType} ({Size} bytes)",
                    entityList.Count,
                    entityType.Name,
                    totalEntitySize
                );
            }

            // Write TOC
            var tocOffset = fileStream.Position;
            foreach (var tocEntry in tocEntries)
            {
                var tocEntryBytes = MemoryPackSerializer.Serialize(tocEntry);
                await fileStream.WriteAsync(tocEntryBytes, cancellationToken);
                dataChecksum.AppendData(tocEntryBytes);
            }

            // Calculate final checksum
            var finalChecksum = dataChecksum.GetHashAndReset();
            var checksumValue = BitConverter.ToUInt32(finalChecksum, 0);

            // Update header with final information
            header.FileSize = (ulong)fileStream.Length;
            header.DataChecksum = checksumValue;
            header.TocOffset = (ulong)tocOffset;
            header.TocEntryCount = (uint)tocEntries.Count;

            // Rewrite header
            fileStream.Seek(0, SeekOrigin.Begin);
            var finalHeaderBytes = MemoryPackSerializer.Serialize(header);
            await fileStream.WriteAsync(finalHeaderBytes, cancellationToken);

            _logger.Information(
                "Successfully saved {TotalEntities} entities across {TypeCount} types to {FilePath} ({FileSize} bytes)",
                entities.Sum(kvp => kvp.Value.Count),
                entities.Count,
                filePath,
                fileStream.Length
            );
        }
        finally
        {
            dataChecksum.Dispose();
        }
    }

    /// <summary>
    /// Loads entities from a binary file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of entity lists grouped by type</returns>
    /// <exception cref="ArgumentNullException">Thrown when filePath is null</exception>
    /// <exception cref="FileNotFoundException">Thrown when file doesn't exist</exception>
    /// <exception cref="InvalidDataException">Thrown when file format is invalid</exception>
    public async Task<IDictionary<Type, IList<object>>> LoadEntitiesAsync(
        string filePath, CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Binary persistence file not found: {filePath}");

        _logger.Information("Loading entities from {FilePath}", filePath);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024
        );

        // Read and validate header
        var headerSize = MemoryPackSerializer.Serialize(new FileHeader()).Length;
        var headerBytes = new byte[headerSize];
        await fileStream.ReadExactlyAsync(headerBytes, cancellationToken);

        var header = MemoryPackSerializer.Deserialize<FileHeader>(headerBytes);

        if (header.MagicNumber != MagicNumber)
            throw new InvalidDataException("Invalid file format: magic number mismatch");

        if (header.Version > CurrentFileVersion)
            throw new InvalidDataException($"Unsupported file version: {header.Version}");

        _logger.Debug(
            "File header: Version={Version}, Entities={EntryCount}, Size={FileSize}",
            header.Version,
            header.TocEntryCount,
            header.FileSize
        );

        // Read TOC
        fileStream.Seek((long)header.TocOffset, SeekOrigin.Begin);
        var tocEntries = new List<TocEntry>();

        for (var i = 0; i < header.TocEntryCount; i++)
        {
            var tocEntrySize = MemoryPackSerializer.Serialize(new TocEntry()).Length;
            var tocEntryBytes = new byte[tocEntrySize];
            await fileStream.ReadExactlyAsync(tocEntryBytes, cancellationToken);

            var tocEntry = MemoryPackSerializer.Deserialize<TocEntry>(tocEntryBytes);
            tocEntries.Add(tocEntry);
        }

        // Load entities by type
        var result = new Dictionary<Type, IList<object>>();

        foreach (var tocEntry in tocEntries)
        {
            var entityType = EntityTypeRegistry.GetEntityType(tocEntry.EntityType);
            var entityList = new List<object>();

            fileStream.Seek((long)tocEntry.Offset, SeekOrigin.Begin);

            for (var i = 0; i < tocEntry.Count; i++)
            {
                // Read entity header
                var entityHeaderSize = MemoryPackSerializer.Serialize(new EntityHeader()).Length;
                var entityHeaderBytes = new byte[entityHeaderSize];
                await fileStream.ReadExactlyAsync(entityHeaderBytes, cancellationToken);

                var entityHeader = MemoryPackSerializer.Deserialize<EntityHeader>(entityHeaderBytes);

                // Read entity data
                var entityDataBytes = new byte[entityHeader.DataSize];
                await fileStream.ReadExactlyAsync(entityDataBytes, cancellationToken);

                // Validate checksum
                var calculatedChecksum = CRC32.Compute(entityDataBytes);
                if (calculatedChecksum != entityHeader.Checksum)
                {
                    _logger.Warning("Checksum mismatch for entity {Index} of type {EntityType}", i, entityType.Name);
                    continue;
                }

                // Deserialize entity
                var entity = MemoryPackSerializer.Deserialize(entityType, entityDataBytes);
                entityList.Add(entity);
            }

            result[entityType] = entityList;

            _logger.Debug("Loaded {Count} entities of type {EntityType}", entityList.Count, entityType.Name);
        }

        _logger.Information(
            "Successfully loaded {TotalEntities} entities across {TypeCount} types from {FilePath}",
            result.Sum(kvp => kvp.Value.Count),
            result.Count,
            filePath
        );

        return result;
    }

    /// <summary>
    /// Loads entities of a specific type from a binary file
    /// </summary>
    /// <typeparam name="T">The entity type to load</typeparam>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of entities of the specified type</returns>
    public async Task<IList<T>> LoadEntitiesAsync<T>(string filePath, CancellationToken cancellationToken = default)
        where T : class
    {
        var allEntities = await LoadEntitiesAsync(filePath, cancellationToken);

        if (allEntities.TryGetValue(typeof(T), out var entities))
        {
            return entities.Cast<T>().ToList();
        }

        return new List<T>();
    }

    /// <summary>
    /// Validates the integrity of a binary persistence file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file is valid, false otherwise</returns>
    public async Task<bool> ValidateFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filePath);

            if (!File.Exists(filePath))
                return false;

            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Read header
            var headerSize = MemoryPackSerializer.Serialize(new FileHeader()).Length;
            var headerBytes = new byte[headerSize];
            await fileStream.ReadExactlyAsync(headerBytes, cancellationToken);

            var header = MemoryPackSerializer.Deserialize<FileHeader>(headerBytes);

            // Basic validation
            if (header.MagicNumber != MagicNumber)
                return false;

            if (header.Version > CurrentFileVersion)
                return false;

            if (header.FileSize != (ulong)fileStream.Length)
                return false;

            _logger.Debug("File {FilePath} passed basic validation", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "File validation failed for {FilePath}", filePath);
            return false;
        }
    }

    /// <summary>
    /// Gets file information without loading the entire file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File information</returns>
    public async Task<EntityFileInfo> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Binary persistence file not found: {filePath}");

        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        // Read header
        var headerSize = MemoryPackSerializer.Serialize(new FileHeader()).Length;
        var headerBytes = new byte[headerSize];
        await fileStream.ReadExactlyAsync(headerBytes, cancellationToken);

        var header = MemoryPackSerializer.Deserialize<FileHeader>(headerBytes);

        // Read TOC to get entity counts by type
        fileStream.Seek((long)header.TocOffset, SeekOrigin.Begin);
        var entityCounts = new Dictionary<Type, uint>();

        for (var i = 0; i < header.TocEntryCount; i++)
        {
            var tocEntrySize = MemoryPackSerializer.Serialize(new TocEntry()).Length;
            var tocEntryBytes = new byte[tocEntrySize];
            await fileStream.ReadExactlyAsync(tocEntryBytes, cancellationToken);

            var tocEntry = MemoryPackSerializer.Deserialize<TocEntry>(tocEntryBytes);
            var entityType = EntityTypeRegistry.GetEntityType(tocEntry.EntityType);
            entityCounts[entityType] = tocEntry.Count;
        }

        return new EntityFileInfo
        {
            FilePath = filePath,
            Version = header.Version,
            FileSize = header.FileSize,
            CreatedTimestamp = DateTimeOffset.FromUnixTimeSeconds(header.CreatedTimestamp),
            ModifiedTimestamp = DateTimeOffset.FromUnixTimeSeconds(header.ModifiedTimestamp),
            EntityCounts = entityCounts
        };
    }

    /// <summary>
    /// Disposes resources used by the service
    /// </summary>
    public void Dispose() =>
        // No resources to dispose currently
        SuppressFinalize(this);
}
