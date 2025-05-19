using System.Diagnostics;
using Jab;
using Moongate.Core.Directories;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Server.Services.System;
using Orion.Core.Server.Interfaces.Services.System;

namespace Moongate.Server.Provider;

[ServiceProvider]
[Singleton<IEventBusService, EventBusService>]
[Singleton<IVersionService, VersionService>]
[Singleton<ITextTemplateService, TextTemplateService>]
[Singleton<IEventDispatcherService, EventDispatcherService>]

[Singleton(typeof(DirectoriesConfig), Instance = nameof(DirectoriesConfig))]
public partial class MoongateServiceProvider
{
    public static MoongateServiceProvider Instance { get; } = new();

    public DirectoriesConfig DirectoriesConfig { get; set; }
}
