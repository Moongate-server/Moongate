using ConsoleAppFramework;
using DryIoc;
using Moongate.Core.Data.Configs.Services;
using Moongate.Core.Data.Options;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Services;
using Moongate.Core.Instances;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Types;
using Moongate.Core.Web.Interfaces.Services;
using Moongate.Core.Web.Services;
using Moongate.Persistence.Builders;
using Moongate.Persistence.Interfaces.Services;
using Moongate.Persistence.Services;
using Moongate.Server;
using Moongate.Server.DataLoaders;
using Moongate.Server.Handlers;
using Moongate.Server.Modules;
using Moongate.Server.Services.System;
using Moongate.Server.Services.Uo;
using Moongate.Uo.Network.Interfaces.Services;
using Moongate.Uo.Network.Packets;
using Moongate.Uo.Network.Packets.Connection;
using Moongate.Uo.Services.Interfaces.Services;
using Moongate.Uo.Services.Serialization.Entities;


await ConsoleApp.RunAsync(
    args,
    async (
        LogLevelType defaultLogLevel = LogLevelType.Debug, bool logToFile = true, bool loadFromEnv = false,
        string? rootDirectory = null, bool printHeader = true, string configName = "moongate.json",
        string ultimaOnlineDirectory = ""
    ) =>
    {
        var cts = new CancellationTokenSource();

        MoongateInstanceHolder.ConsoleCancellationTokenSource = cts;

        var moongateStartupServer = new MoongateStartupServer(
            cts,
            new MoongateServerArgs()
            {
                RootDirectory = rootDirectory,
                LogToFile = logToFile,
                LoadFromEnv = loadFromEnv,
                PrintHeader = printHeader,
                ConfigName = configName,
                UltimaOnlineDirectory = ultimaOnlineDirectory,
                DefaultLogLevel = defaultLogLevel
            }
        );

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;

            Console.WriteLine();
            Console.WriteLine("Shutdown signal received. Stopping server gracefully...");

            moongateStartupServer.StopAsync().Wait();

            if (!cts.Token.IsCancellationRequested)
            {
                cts.Cancel();
            }
        };


        // Register the entity types
        EntityRegistrationBuilder.Instance
            .Register<AccountEntity>()
            .Register<CharacterEntity>();

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
                .AddService(typeof(IWebServerService), typeof(WebServerService))
                .AddService(typeof(IDataFileLoaderService), typeof(DataFileLoaderService), -1)
                .AddService(typeof(ISessionManagerService), typeof(SessionManagerService), 99)
                .AddService(typeof(INetworkService), typeof(NetworkService), 100)
                ;


            container
                .AddService(typeof(IPersistenceManager), typeof(MemoryPackPersistenceManager));


            container
                .AddService(typeof(IAccountManagerService), typeof(AccountManagerService));

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
            scriptEngine.AddScriptModule(typeof(ConsoleCommandModule));
        };


        moongateStartupServer.RegisterPacketsAndHandlers += networkService =>
        {
            networkService.RegisterPacket<SeedPacket>();
            networkService.RegisterPacket<LoginPacket>();
            networkService.RegisterPacket<SelectServerPacket>();
            networkService.RegisterPacket<GameServerLoginPacket>();


            networkService.RegisterPacketHandler<SeedPacket, LoginHandler>();
            networkService.RegisterPacketHandler<LoginPacket, LoginHandler>();
            networkService.RegisterPacketHandler<SelectServerPacket, LoginHandler>();
            networkService.RegisterPacketHandler<GameServerLoginPacket, GameLoginHandler>();
        };

        moongateStartupServer.BeforeStart += container =>
        {
            var dataLoaderService = container.Resolve<IDataFileLoaderService>();

            dataLoaderService.AddDataLoaderType(typeof(ServerClientVersionLoader), 0);
        };


        moongateStartupServer.Init();

        await moongateStartupServer.StartAsync();
    }
);
