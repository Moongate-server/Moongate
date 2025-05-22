using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Moongate.Persistence.Entities;

/// <summary>
///     Registry for mapping entity types to their byte identifiers
/// </summary>
public static class EntityTypeRegistry
{
    private static readonly ConcurrentDictionary<Type, byte> _typeToId = new();
    private static readonly ConcurrentDictionary<byte, Type> _idToType = new();
    private static byte _nextId = 1;

    /// <summary>
    ///     Registers an entity type with the specified ID
    /// </summary>
    /// <typeparam name="T">The entity type to register</typeparam>
    /// <param name="id">The byte identifier for this entity type</param>
    public static void RegisterEntityType<T>(byte id) where T : class
    {
        var type = typeof(T);
        _typeToId[type] = id;
        _idToType[id] = type;
    }

    /// <summary>
    ///     Registers an entity type with an auto-assigned ID
    /// </summary>
    /// <typeparam name="T">The entity type to register</typeparam>
    /// <returns>The assigned ID</returns>
    public static byte RegisterEntityType<T>() where T : class
    {
        var id = _nextId++;
        RegisterEntityType<T>(id);
        return id;
    }

    /// <summary>
    ///     Gets the ID for the specified entity type
    /// </summary>
    /// <param name="type">The entity type</param>
    /// <returns>The byte identifier</returns>
    public static byte GetEntityId(Type type)
    {
        return _typeToId.TryGetValue(type, out var id)
            ? id
            : throw new InvalidOperationException($"Entity type {type.Name} is not registered");
    }

    /// <summary>
    ///     Gets the type for the specified entity ID
    /// </summary>
    /// <param name="id">The byte identifier</param>
    /// <returns>The entity type</returns>
    public static Type GetEntityType(byte id)
    {
        return _idToType.TryGetValue(id, out var type)
            ? type
            : throw new InvalidOperationException($"Entity ID {id} is not registered");
    }

    /// <summary>
    ///     Checks if an entity type is registered
    /// </summary>
    /// <typeparam name="T">The entity type to check</typeparam>
    /// <returns>True if registered, false otherwise</returns>
    public static bool IsRegistered<T>() where T : class
    {
        return _typeToId.ContainsKey(typeof(T));
    }

    public static bool IsRegistered(Type type)
    {
        return _typeToId.ContainsKey(type);
    }

    /// <summary>
    ///     Gets all registered entity types
    /// </summary>
    /// <returns>Dictionary of type to ID mappings</returns>
    public static IReadOnlyDictionary<Type, byte> GetRegisteredTypes()
    {
        return _typeToId.ToImmutableDictionary();
    }
}
