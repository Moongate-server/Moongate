using Microsoft.Extensions.DependencyInjection;

namespace sMoongate.Core.Extensions.Services;

/// <summary>
///     Extension methods for registering typed lists in the dependency injection container.
/// </summary>
public static class AddTypedListMethodEx
{
    /// <summary>
    ///     Adds an entity to a typed list in the dependency injection container.
    ///     If the list doesn't exist, it creates a new one.
    /// </summary>
    /// <typeparam name="TListEntity">The type of entities in the list.</typeparam>
    /// <param name="services">The IServiceCollection to add the entity to.</param>
    /// <param name="entity">The entity to add to the list.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    ///     This method is useful for scenarios where you need to register multiple implementations
    ///     of the same type that can later be retrieved as a collection. For example, registering
    ///     multiple handlers, validators, or processors that will be enumerated and used together.
    ///     Example usage:
    ///     <code>
    /// services.AddToRegisterTypedList(new DataStruct());
    /// services.AddToRegisterTypedList(new DataObject());
    ///
    /// // Later, inject List&lt;IHandler&gt; to get all registered handlers
    /// </code>
    /// </remarks>
    public static IServiceCollection AddToRegisterTypedList<TListEntity>(
        this IServiceCollection services, TListEntity entity
    )
    {
        // Check for null parameters
        ArgumentNullException.ThrowIfNull(services);

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        // Check if a list of this type already exists in the service collection
        var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(List<TListEntity>));

        if (serviceDescriptor != null)
        {
            // If the list exists, get it and add the new entity
            var typedList = (List<TListEntity>)serviceDescriptor.ImplementationInstance!;
            typedList.Add(entity);
        }
        else
        {
            // If the list doesn't exist, create a new one, add the entity, and register it
            var typedList = new List<TListEntity> { entity };
            services.AddSingleton(typedList);
        }

        return services;
    }
}
