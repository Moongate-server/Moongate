using DryIoc;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Options;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Loggers;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Instances;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Json;
using Moongate.Core.Services.Hosted;
using Moongate.Core.Types;
using Moongate.Core.Utils.Resources;
using Moongate.Server.Json;
using Moongate.Server.Services.System;
using Moongate.Uo.Network.Interfaces.Services;
using Serilog;
using Serilog.Formatting.Compact;

namespace Moongate.Server;

public class MoongateStartupServer
{
    public delegate void RegisterServicesDelegate(IContainer container);

    public event RegisterServicesDelegate RegisterServices;

    public delegate void RegisterScriptModulesDelegate(IScriptEngineService container);

    public event RegisterScriptModulesDelegate RegisterScriptModules;

    public delegate void RegisterPacketsAndHandlersDelegate(INetworkService networkService);

    public event RegisterPacketsAndHandlersDelegate RegisterPacketsAndHandlers;

    public delegate void BeforeStartDelegate(IContainer container);

    public event BeforeStartDelegate BeforeStart;


    private IContainer _container;

    private readonly MoongateServerArgs _moongateServerArgs;

    private readonly CancellationTokenSource _cancellationTokenSource;

    public MoongateStartupServer(CancellationTokenSource cancellationTokenSource, MoongateServerArgs serverArgs)
    {
        _cancellationTokenSource = cancellationTokenSource;
        _moongateServerArgs = serverArgs;

        SetupRootDirectory();
        CheckEnvFileAndLoad();
        PrintHeader();
        BuildContainer();
        RegisterInstances();
        SetupLogging();
    }

    public void Init()
    {
        var config = CheckAndLoadConfig(_container.Resolve<DirectoriesConfig>(), _moongateServerArgs.ConfigName);

        _container.RegisterInstance(config);

        if (!CheckUltimaOnlineDirectory(config))
        {
            Log.Logger.Error("Ultima Online directory not found. Please check your configuration.");
            Environment.Exit(1);
            return;
        }

        _container.AddService(typeof(MoongateStartupService));

        RegisterServices?.Invoke(_container);

        _container.AddService(typeof(IConsoleCommandService), typeof(ConsoleCommandService));
    }

    public Task StopAsync()
    {
        var startupService = _container.Resolve<MoongateStartupService>();

        return startupService.StopAsync(_cancellationTokenSource.Token);
    }

    public async Task StartAsync()
    {
        Log.Information(
            "Starting Moongate Server v{Version}...",
            _container.Resolve<IVersionService>().GetVersionInfo().Version
        );
        Log.Information("Root directory: {RootDirectory}", _container.Resolve<DirectoriesConfig>().Root);

        var scriptEngine = _container.Resolve<IScriptEngineService>();
        var networkService = _container.Resolve<INetworkService>();
        var startupService = _container.Resolve<MoongateStartupService>();

        RegisterScriptModules?.Invoke(scriptEngine);

        RegisterPacketsAndHandlers?.Invoke(networkService);

        BeforeStart?.Invoke(_container);
        try
        {
            await startupService.StartAsync(_cancellationTokenSource.Token);

            await RunConsoleInputLoop();
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

    private async Task RunConsoleInputLoop()
    {
        Console.WriteLine("Server started successfully. Type 'help' for commands or 'quit' to exit.");
        Console.WriteLine("Press Ctrl+C to stop the server.");

        var consoleTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        Console.Write("> ");

                        var input = await ReadLineWithCancellation(_cancellationTokenSource.Token);

                        if (input == null)
                        {
                            break;
                        }

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            continue;
                        }

                        await _container.Resolve<IConsoleCommandService>().ProcessCommand(input.Trim().ToLowerInvariant());
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error processing console command.");
                    }
                }
            }
        );


        try
        {
            await Task.WhenAny(consoleTask, Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token));
        }
        catch (OperationCanceledException)
        {
            Log.Information("Cancellation detected in console loop.");
        }
    }

    private async Task<string> ReadLineWithCancellation(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<string>();


        await using var registration = cancellationToken.Register(() => { tcs.TrySetCanceled(); });


        Task.Run(
            () =>
            {
                try
                {
                    var result = Console.ReadLine();
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            },
            cancellationToken
        );

        try
        {
            return await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }


    private void BuildContainer()
    {
        _container = new Container(rules => rules
            .WithUseInterpretation()
            .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Keep)
            .WithFactorySelector(Rules.SelectLastRegisteredFactory())
        );

        MoongateInstanceHolder.Container = _container;
        _container.RegisterInstance(_container);
    }

    private void RegisterInstances()
    {
        _container.RegisterInstance(
            new DirectoriesConfig(_moongateServerArgs.RootDirectory, Enum.GetNames<DirectoryType>())
        );
    }

    private void SetupRootDirectory()
    {
        _moongateServerArgs.RootDirectory = Environment.GetEnvironmentVariable("MOONGATE_ROOT") ??
                                            Path.Combine(Directory.GetCurrentDirectory(), "moongate");
    }

    private void PrintHeader()
    {
        if (_moongateServerArgs.PrintHeader)
        {
            var content = ResourceUtils.ReadEmbeddedResource("Assets.header.txt", typeof(Program).Assembly);

            if (!string.IsNullOrEmpty(content))
            {
                Console.WriteLine(content);
            }
        }
    }

    private void SetupLogging()
    {
        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(_moongateServerArgs.DefaultLogLevel.ToSerilogLogLevel())
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithThreadId()
            .WriteTo.Console();

        if (_moongateServerArgs.LogToFile)
        {
            var logFilePath = Path.Combine(
                _container.Resolve<DirectoriesConfig>()[DirectoryType.Logs],
                "moongate_.log"
            );

            logConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                logFilePath,
                rollingInterval: RollingInterval.Day
            );
        }

        Log.Logger = logConfiguration.CreateLogger();
    }

    private void CheckEnvFileAndLoad()
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

    private MoongateServerConfig CheckAndLoadConfig(
        DirectoriesConfig directoriesConfig, string configName
    )
    {
        Log.Logger.Information("Loading config: {ConfigName}", configName);
        var config = new MoongateServerConfig();

        var configPath = Path.Combine(directoriesConfig.Root, configName);

        if (!File.Exists(configPath))
        {
            JsonUtils.SerializeToFile(config, configPath, MoongateJsonContext.Default);
        }

        config = JsonUtils.DeserializeFromFile<MoongateServerConfig>(
            configPath,
            MoongateJsonContext.Default
        );

        JsonUtils.SerializeToFile(config, configPath, MoongateJsonContext.Default);

        config.Shard.UoDirectory ??= _moongateServerArgs.UltimaOnlineDirectory;

        return config;
    }

    public static bool CheckUltimaOnlineDirectory(MoongateServerConfig config)
    {
        if (string.IsNullOrEmpty(config.Shard.UoDirectory))
        {
            Log.Logger.Error("Ultima Online directory is not set in the configuration.");
            return false;
        }

        if (!Directory.Exists(config.Shard.UoDirectory))
        {
            Log.Logger.Error("Ultima Online directory not found: {UltimaOnlineDirectory}", config.Shard.UoDirectory);
            return false;
        }

        return true;
    }
}
