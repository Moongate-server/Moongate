using System.Diagnostics;
using Jab;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Server.Services.System;
using Orion.Core.Server.Interfaces.Services.System;

namespace Moongate.Server.Provider;

[ServiceProvider]
[Singleton<IEventBusService, EventBusService>]
[Singleton<IVersionService, VersionService>]
[Singleton<ITextTemplateService, TextTemplateService>]
[Singleton<IEventDispatcherService, EventDispatcherService>]
public partial class MoongateServiceProvider
{

    public static MoongateServiceProvider Instance { get; } = new();
}
