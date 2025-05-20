using DryIoc;
using Orion.Core.Server.Interfaces.Services.System;

namespace Moongate.Core.Instances;

public static class MoongateInstanceHolder
{
    public static IContainer Container { get; set; }

    public static ITextTemplateService TemplateServiceService => Container.Resolve<ITextTemplateService>();
}
