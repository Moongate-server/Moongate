using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Server.Services;
using Moongate.Server.Services.System;
using Orion.Core.Server.Interfaces.Services.System;
using Serilog;

namespace Moongate.Server;

class Program
{
    public static CancellationTokenRegistration _quitTokenRegistration = new();

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);


        builder.UseServiceProviderFactory(new DryIocServiceProviderFactory());

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        builder.ConfigureLogging(loggingBuilder => { loggingBuilder.ClearProviders().AddSerilog(); });

        builder.ConfigureServices(services =>
            {
                services
                    .AddService<IEventBusService, EventBusService>()
                    .AddService<ITextTemplateService, TextTemplateService>(-1)
                    .AddService<IVersionService, VersionService>()
                    .AddService<IEventDispatcherService, EventDispatcherService>()
                    .AddService<IProcessQueueService, ProcessQueueService>()
                    ;


                services.AddHostedService<MoongateStartupService>();
            }
        );


        var app = builder.Build();

        await app.RunAsync(_quitTokenRegistration.Token);
    }
}
