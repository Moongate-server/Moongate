using System.Reflection;
using Moongate.Core.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Services.System;
using Serilog;
using ConsoleAppFramework;
using DryIoc;
using Moongate.Core.Data.Configs.Services;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Loggers;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Extensions.Templates;
using Moongate.Core.Instances;
using Moongate.Core.Types;
using Moongate.Core.Utils.Resources;
using Moongate.Server.Modules;
using Moongate.Server.Services.System;
using Serilog.Formatting.Compact;


await ConsoleApp.RunAsync(
    args,
    async (
        LogLevelType defaultLogLevel = LogLevelType.Debug, bool logToFile = true, bool loadFromEnv = false,
        string? rootDirectory = null, bool printHeader = true
    ) =>
    {
        var cts = new CancellationTokenSource();

        if (loadFromEnv)
        {
            CheckEnvFileAndLoad();
        }

        rootDirectory ??= Environment.GetEnvironmentVariable("MOONGATE_ROOT") ??
                          Path.Combine(Directory.GetCurrentDirectory(), "moongate");


        var content = ResourceUtils.ReadEmbeddedResource("Assets.header.txt", typeof(Program).Assembly);

        if (!string.IsNullOrEmpty(content) && printHeader)
        {
            Console.WriteLine(content);
        }

        var container = new Container(rules => rules
            .WithUseInterpretation()
            .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Keep)
            .WithFactorySelector(Rules.SelectLastRegisteredFactory())
        );

        MoongateInstanceHolder.Container = container;

        container
            .AddService(typeof(IEventBusService), typeof(EventBusService))
            .AddService(typeof(ITextTemplateService), typeof(TextTemplateService))
            .AddService(typeof(IVersionService), typeof(VersionService))
            .AddService(typeof(ISchedulerSystemService), typeof(SchedulerSystemService))
            .AddService(typeof(IDiagnosticService), typeof(DiagnosticService))
            .AddService(typeof(ITimerService), typeof(TimerService))
            .AddService(typeof(IEventLoopService), typeof(EventLoopService), -1)
            .AddService(typeof(IProcessQueueService), typeof(ProcessQueueService))
            .AddService(typeof(IEventDispatcherService), typeof(EventDispatcherService))
            .AddService(typeof(IScriptEngineService), typeof(ScriptEngineService));

        container.AddService(typeof(MoongateStartupService));

        container.RegisterInstance(container);


        container.RegisterInstance(new DirectoriesConfig(rootDirectory, Enum.GetNames<DirectoryType>()));
        container.RegisterInstance(new ScriptEngineConfig());
        container.RegisterInstance(new EventLoopConfig());
        container.RegisterInstance(
            new DiagnosticServiceConfig()
            {
                PidFileName = Path.Combine(container.Resolve<DirectoriesConfig>().Root, "moongate.pid")
            }
        );

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
            container
                .AddService(typeof(LoggerModule))
                .AddService(typeof(SchedulerModule))
                .AddService(typeof(IncludeModule))
                .AddService(typeof(TimerScriptModule));


            container.Resolve<IScriptEngineService>().AddScriptModule(typeof(LoggerModule));
            container.Resolve<IScriptEngineService>().AddScriptModule(typeof(SchedulerModule));
            container.Resolve<IScriptEngineService>().AddScriptModule(typeof(IncludeModule));
            container.Resolve<IScriptEngineService>().AddScriptModule(typeof(TimerScriptModule));


            await container.Resolve<MoongateStartupService>().StartAsync(cts.Token);

            Log.Information("CPU: {{cpu_count}} running version: {{version}}".ReplaceTemplate());

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

void CheckEnvFileAndLoad()
{
    var currentDirectory = Directory.GetCurrentDirectory();

    var envFilePath = Path.Combine(currentDirectory, ".env");

    if (File.Exists(envFilePath))
    {
        var envFileContent = File.ReadAllText(envFilePath);
        var envVariables = envFileContent.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var envVariable in envVariables)
        {
            var keyValue = envVariable.Split('=');
            if (keyValue.Length == 2)
            {
                Environment.SetEnvironmentVariable(keyValue[0].Trim(), keyValue[1].Trim());
            }
        }
    }
}
