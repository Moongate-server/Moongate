using Moongate.Persistence.Entities;
using Moongate.Persistence.Interfaces.Entities;

namespace Moongate.Persistence.Builders;

/// <summary>
/// Implementation of the entity registration builder
/// </summary>
public class EntityRegistrationBuilder : IEntityRegistrationBuilder
{
    public IEntityRegistrationBuilder Register<T>(byte id) where T : class
    {
        EntityTypeRegistry.RegisterEntityType<T>(id);
        return this;
    }

    public IEntityRegistrationBuilder Register<T>() where T : class
    {
        EntityTypeRegistry.RegisterEntityType<T>();
        return this;
    }
}
