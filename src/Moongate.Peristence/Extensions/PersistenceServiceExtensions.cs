using Moongate.Persistence.Entities;
using Moongate.Persistence.Interfaces.Services;

namespace Moongate.Persistence.Extensions;

/// <summary>
/// Extension methods for the binary persistence service
/// </summary>
public static class PersistenceServiceExtensions
{
    /// <summary>
    /// Saves a collection of entities of the same type
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="service">The persistence service</param>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="entities">Collection of entities to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task SaveEntitiesAsync<T>(this IPersistenceManager service, string filePath, IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
    {
        if (!EntityTypeRegistry.IsRegistered<T>())
            throw new InvalidOperationException($"Entity type {typeof(T).Name} is not registered. Call EntityTypeRegistry.RegisterEntityType<{typeof(T).Name}>() first.");

        var entityDict = new Dictionary<Type, IList<object>>
        {
            [typeof(T)] = entities.Cast<object>().ToList()
        };

        await service.SaveEntitiesAsync(filePath, entityDict, cancellationToken);
    }

    /// <summary>
    /// Saves multiple entity collections to a single file
    /// </summary>
    /// <param name="service">The persistence service</param>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="entityCollections">Collections of entities to save</param>
    public static async Task SaveMultipleEntitiesAsync(this IPersistenceManager service, string filePath, CancellationToken cancellationToken = default, params (Type Type, IEnumerable<object> Entities)[] entityCollections)
    {
        var entityDict = new Dictionary<Type, IList<object>>();

        foreach (var (type, entities) in entityCollections)
        {
            if (!EntityTypeRegistry.GetRegisteredTypes().ContainsKey(type))
                throw new InvalidOperationException($"Entity type {type.Name} is not registered.");

            entityDict[type] = entities.ToList();
        }

        await service.SaveEntitiesAsync(filePath, entityDict, cancellationToken);
    }

    /// <summary>
    /// Creates a backup of an existing persistence file
    /// </summary>
    /// <param name="service">The persistence service</param>
    /// <param name="sourceFilePath">Source file path</param>
    /// <param name="backupFilePath">Backup file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task CreateBackupAsync(this IPersistenceManager service, string sourceFilePath, string backupFilePath, CancellationToken cancellationToken = default)
    {
        // Validate source file first
        if (!await service.ValidateFileAsync(sourceFilePath, cancellationToken))
            throw new InvalidOperationException($"Source file {sourceFilePath} is not valid for backup");

        // Load and re-save to ensure data integrity
        var entities = await service.LoadEntitiesAsync(sourceFilePath, cancellationToken);
        await service.SaveEntitiesAsync(backupFilePath, entities, cancellationToken);
    }

    /// <summary>
    /// Compacts a persistence file by removing fragmentation
    /// </summary>
    /// <param name="service">The persistence service</param>
    /// <param name="filePath">File path to compact</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task CompactFileAsync(this IPersistenceManager service, string filePath, CancellationToken cancellationToken = default)
    {
        var tempFilePath = $"{filePath}.compact.tmp";

        try
        {
            // Load all entities
            var entities = await service.LoadEntitiesAsync(filePath, cancellationToken);

            // Save to temporary file
            await service.SaveEntitiesAsync(tempFilePath, entities, cancellationToken);

            // Replace original file
            File.Move(tempFilePath, filePath, true);
        }
        finally
        {
            // Clean up temporary file if it exists
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }
}
