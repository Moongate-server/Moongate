using Moongate.Core.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Services.System;
using Serilog;
using ConsoleAppFramework;
using DryIoc;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Loggers;
using Moongate.Core.Types;
using Serilog.Formatting.Compact;


await ConsoleApp.RunAsync(
    args,
    async (LogLevelType defaultLogLevel = LogLevelType.Information, bool logToFile = true) =>
    {
        var cts = new CancellationTokenSource();

        var container = new Container();
        container.RegisterInstance(container);
        container.RegisterInstance(new DirectoriesConfig(Enum.GetNames<DirectoryType>()));


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
            container.Resolve<IEventBusService>();
            container.Resolve<IVersionService>();
            container.Resolve<ITextTemplateService>();

            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Handle cancellation
            Log.Information("Cancellation requested. Exiting...");
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
