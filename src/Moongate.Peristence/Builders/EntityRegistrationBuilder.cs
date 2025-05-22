using System.Reflection;
using Moongate.Persistence.Attributes;
using Moongate.Persistence.Entities;
using Moongate.Persistence.Interfaces.Entities;

namespace Moongate.Persistence.Builders;

/// <summary>
///     Implementation of the entity registration builder
/// </summary>
public class EntityRegistrationBuilder : IEntityRegistrationBuilder
{
    public static EntityRegistrationBuilder Instance { get; } = new();

    public IEntityRegistrationBuilder Register<T>(byte id) where T : class
    {
        EntityTypeRegistry.RegisterEntityType<T>(id);
        return this;
    }

    public IEntityRegistrationBuilder Register<T>() where T : class
    {
        var attribute = typeof(T).GetCustomAttribute<EntityTypeAttribute>(true);

        if (attribute == null)
        {
            throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have an EntityType attribute");
        }


        EntityTypeRegistry.RegisterEntityType<T>(attribute.TypeId);
        return this;
    }
}
