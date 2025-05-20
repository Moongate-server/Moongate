using System.Diagnostics.CodeAnalysis;
using DryIoc;
using Moongate.Core.Data.Services;

namespace Moongate.Core.Extensions.Services;

public static class AddMoongateServiceExtension
{
    public static IContainer AddService(
        this IContainer container,
        Type serviceType,
        Type implementationType,
        int priority = 0
    )
    {
        container.Register(serviceType, implementationType, Reuse.Singleton);
        container.AddToRegisterTypedList(new ServiceDescriptionData(serviceType, implementationType, priority));
        return container;
    }


    // public static IContainer AddService<TService, TImplementation>(
    //     this IContainer container, int priority = 0
    // ) where TService : class where TImplementation : class, TService
    // {
    //     container.Register<TService, TImplementation>(Reuse.Singleton);
    //     container.AddToRegisterTypedList(new ServiceDescriptionData(typeof(TService), typeof(TImplementation), priority));
    //     return container;
    // }
    //
    // public static IContainer AddService<TService>(
    //     this IContainer container, int priority = 0
    // ) where TService : class
    // {
    //     container.Register<TService>(Reuse.Singleton);
    //     container.AddToRegisterTypedList(new ServiceDescriptionData(typeof(TService), typeof(TService), priority));
    //     return container;
    // }


    public static IContainer AddService(
        this IContainer container, [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors
        )]
        Type serviceType, int priority = 0
    )
    {
        container.Register(serviceType, Reuse.Singleton);
        container.AddToRegisterTypedList(new ServiceDescriptionData(serviceType, serviceType, priority));
        return container;
    }
}
