using Moongate.Persistence.Entities;

namespace Moongate.Persistence.Interfaces.Services;

public interface IPersistenceManager
{
    /// <summary>
    /// Saves entities to a binary file with compression and integrity checks
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="entities">Dictionary of entity lists grouped by type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveEntitiesAsync(
        string filePath, List<object> entities, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Loads all entities from a binary file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of entity lists grouped by type</returns>
    Task<IDictionary<Type, IList<object>>> LoadEntitiesAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads entities of a specific type from a binary file
    /// </summary>
    /// <typeparam name="T">The entity type to load</typeparam>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of entities of the specified type</returns>
    Task<IList<T>> LoadEntitiesAsync<T>(string filePath, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Validates the integrity of a binary persistence file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file is valid, false otherwise</returns>
    Task<bool> ValidateFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file information without loading the entire file
    /// </summary>
    /// <param name="filePath">Path to the binary file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File information</returns>
    Task<EntityFileInfo> GetFileInfoAsync(string filePath, CancellationToken cancellationToken = default);
}
