using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Moongate.Core.Data.Services;
using sMoongate.Core.Extensions.Services;

namespace Moongate.Core.Extensions.Services;

public static class AddMoongateServiceExtension
{
    public static IServiceCollection AddService(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors
        )]
        Type serviceType,
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors
        )]
        Type implementationType, int priority = 0
    )
    {
        services.AddSingleton(serviceType, implementationType);
        services.AddToRegisterTypedList(new ServiceDescriptionData(serviceType, implementationType, priority));

        return services;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
    public static IServiceCollection AddService<TService, TImplementation>(
        this IServiceCollection services, int priority = 0
    ) where TService : class where TImplementation : class, TService
    {
        services.AddSingleton<TService, TImplementation>();
        services.AddToRegisterTypedList(new ServiceDescriptionData(typeof(TService), typeof(TImplementation), priority));

        return services;
    }

    public static IServiceCollection AddService<TService>(
        this IServiceCollection services, int priority = 0
    ) where TService : class
    {
        services.AddSingleton<TService>();
        services.AddToRegisterTypedList(new ServiceDescriptionData(typeof(TService), typeof(TService), priority));

        return services;
    }

    public static IServiceCollection AddService(
        this IServiceCollection services, Type serviceType, int priority = 0
    )
    {
        services.AddSingleton(serviceType);
        services.AddToRegisterTypedList(new ServiceDescriptionData(serviceType, serviceType, priority));

        return services;
    }
}
