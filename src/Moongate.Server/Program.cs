using Moongate.Core.Interfaces.Services.System;
using Moongate.Server.Provider;
using Orion.Core.Server.Interfaces.Services.System;
using Serilog;
using ConsoleAppFramework;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Loggers;
using Moongate.Core.Types;
using Serilog.Formatting.Compact;


await ConsoleApp.RunAsync(
    args,
    async (LogLevelType defaultLogLevel = LogLevelType.Information, bool logToFile = true) =>
    {
        MoongateServiceProvider.Instance.DirectoriesConfig = new DirectoriesConfig(Enum.GetNames<DirectoryType>());

        var cts = new CancellationTokenSource();

        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(defaultLogLevel.ToSerilogLogLevel())
            .WriteTo.Console();

        if (logToFile)
        {
            var logFilePath = Path.Combine(
                MoongateServiceProvider.Instance.DirectoriesConfig[DirectoryType.Logs],
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
        Log.Information("Root directory: {RootDirectory}", MoongateServiceProvider.Instance.DirectoriesConfig.Root);


        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        try
        {
            MoongateServiceProvider.Instance.GetService<IEventBusService>();
            MoongateServiceProvider.Instance.GetService<IVersionService>();
            MoongateServiceProvider.Instance.GetService<ITextTemplateService>();

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
