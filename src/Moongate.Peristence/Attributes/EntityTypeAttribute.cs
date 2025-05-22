namespace Moongate.Persistence.Attributes;

/// <summary>
///     Attribute to mark classes as persistable entities with a specific type ID
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EntityTypeAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the EntityTypeAttribute
    /// </summary>
    /// <param name="typeId">The byte identifier for this entity type</param>
    public EntityTypeAttribute(byte typeId) => TypeId = typeId;

    /// <summary>
    ///     The byte identifier for this entity type
    /// </summary>
    public byte TypeId { get; }

    /// <summary>
    ///     Description of the entity type
    /// </summary>
    public string? Description { get; set; }
}
