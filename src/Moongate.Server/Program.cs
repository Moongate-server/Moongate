using Moongate.Core.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Services.System;
using Serilog;
using ConsoleAppFramework;
using DryIoc;
using Moongate.Core.Data.Configs.Services;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Loggers;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Types;
using Moongate.Server.Modules;
using Moongate.Server.Services.System;
using Serilog.Formatting.Compact;


await ConsoleApp.RunAsync(
    args,
    async (LogLevelType defaultLogLevel = LogLevelType.Debug, bool logToFile = true) =>
    {
        var cts = new CancellationTokenSource();

        var container = new Container();

        container
            .AddService<IEventBusService, EventBusService>()
            .AddService<IVersionService, VersionService>()
            .AddService<IScriptEngineService, ScriptEngineService>()
            .AddService<ITextTemplateService, TextTemplateService>();

        container.AddService<MoongateStartupService>();

        container.RegisterInstance(container);


        container.RegisterInstance(new DirectoriesConfig(Enum.GetNames<DirectoryType>()));
        container.RegisterInstance(new ScriptEngineConfig());

        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(defaultLogLevel.ToSerilogLogLevel())
            .WriteTo.Console();

        if (logToFile)
        {
            var logFilePath = Path.Combine(
                container.Resolve<DirectoriesConfig>()[DirectoryType.Logs],
                "moongate_.log"
            );

            logConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                logFilePath,
                rollingInterval: RollingInterval.Day
            );
        }

        Log.Logger = logConfiguration.CreateLogger();


        Log.Information("Starting Moongate Server...");
        Log.Information("Root directory: {RootDirectory}", container.Resolve<DirectoriesConfig>().Root);


        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        try
        {
            // Add script here
            container.Resolve<IScriptEngineService>().AddScriptModule(typeof(LoggerModule));

            await container.Resolve<MoongateStartupService>().StartAsync(cts.Token);

            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Handle cancellation
            Log.Information("Cancellation requested. Exiting...");

            await container.Resolve<MoongateStartupService>().StopAsync(cts.Token);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while running the application.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
);
