using ConsoleAppFramework;
using DryIoc;
using Moongate.Core.Data.Configs.Services;
using Moongate.Core.Data.Options;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Types;
using Moongate.Server;
using Moongate.Server.Modules;
using Moongate.Server.Services.System;
using Moongate.Uo.Network.Interfaces.Services;
using Orion.Core.Server.Interfaces.Services.System;

await ConsoleApp.RunAsync(
    args,
    async (
        LogLevelType defaultLogLevel = LogLevelType.Debug, bool logToFile = true, bool loadFromEnv = false,
        string? rootDirectory = null, bool printHeader = true, string configName = "moongate.json"
    ) =>
    {
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        var moongateStartupServer = new MoongateStartupServer(
            cts,
            new MoongateServerArgs()
            {
                RootDirectory = rootDirectory,
                LogToFile = logToFile,
                LoadFromEnv = loadFromEnv,
                PrintHeader = printHeader,
                ConfigName = configName,
                DefaultLogLevel = defaultLogLevel
            }
        );

        moongateStartupServer.RegisterServices += container =>
        {
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
                .AddService(typeof(IScriptEngineService), typeof(ScriptEngineService))
                .AddService(typeof(ISessionManagerService), typeof(SessionManagerService), 99)
                .AddService(typeof(INetworkService), typeof(NetworkService), 100)
                ;


            container.RegisterInstance(new ScriptEngineConfig());

            container.RegisterInstance(new EventLoopConfig());
            container.RegisterInstance(
                new DiagnosticServiceConfig()
                {
                    PidFileName = Path.Combine(container.Resolve<DirectoriesConfig>().Root, "moongate.pid")
                }
            );

        };


        moongateStartupServer.RegisterScriptModules += scriptEngine =>
        {
            scriptEngine.AddScriptModule(typeof(LoggerModule));
            scriptEngine.AddScriptModule(typeof(SchedulerModule));
            scriptEngine.AddScriptModule(typeof(IncludeModule));
            scriptEngine.AddScriptModule(typeof(VariableScriptModule));
            scriptEngine.AddScriptModule(typeof(TimerScriptModule));
        };


        moongateStartupServer.Init();

        await moongateStartupServer.StartAsync();
    }
);
