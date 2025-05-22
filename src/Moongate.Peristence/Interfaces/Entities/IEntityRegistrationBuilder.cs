namespace Moongate.Persistence.Interfaces.Entities;

/// <summary>
/// Builder for registering entity types
/// </summary>
public interface IEntityRegistrationBuilder
{
    /// <summary>
    /// Registers an entity type with a specific ID
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="id">The byte identifier</param>
    /// <returns>The builder for method chaining</returns>
    IEntityRegistrationBuilder Register<T>(byte id) where T : class;

    /// <summary>
    /// Registers an entity type with an auto-assigned ID
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>The builder for method chaining</returns>
    IEntityRegistrationBuilder Register<T>() where T : class;
}
