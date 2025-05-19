namespace Moongate.Core.Data.Services;

/// <summary>
///   ServiceDescriptionData is a record that describes a service and its implementation type.
/// </summary>
/// <param name="ServiceType"></param>
/// <param name="ImplementationType"></param>
/// <param name="Priority"></param>
public record ServiceDescriptionData(Type ServiceType, Type ImplementationType, int Priority = 0);
