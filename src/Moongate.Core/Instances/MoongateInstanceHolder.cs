using DryIoc;
using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Core.Instances;

public static class MoongateInstanceHolder
{
    public static CancellationTokenSource ConsoleCancellationTokenSource { get; set; } = new();

    public static IContainer Container { get; set; }

    public static ITextTemplateService TemplateServiceService => Container.Resolve<ITextTemplateService>();

    public static Task PublishEvent<T>(T @event) where T : class
    {
        return Container.Resolve<IEventBusService>().PublishAsync(@event);
    }
}
