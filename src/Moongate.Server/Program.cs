using ConsoleAppFramework;
using DryIoc;
using MemoryPack;
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
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Network.Packets.Characters;
using Moongate.Uo.Data.Network.Packets.GeneralInformation;
using Moongate.Uo.Data.Network.Packets.Housing;
using Moongate.Uo.Data.Network.Packets.Ui;
using Moongate.Uo.Data.Serializers;
using Moongate.Uo.Network.Interfaces.Services;
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
            .Register<CharacterEntity>()
            .Register<ItemEntity>()
            .Register<MobileEntity>()
            ;

        moongateStartupServer.RegisterCustomSerializers += () =>
        {
            MemoryPackFormatterProvider.Register(new RaceSerializer());
            MemoryPackFormatterProvider.Register(new ProfessionSerializer());
            MemoryPackFormatterProvider.Register(new Point3dSerializer());
            MemoryPackFormatterProvider.Register(new Point2dSerializer());
            MemoryPackFormatterProvider.Register(new MapSerializer());
            MemoryPackFormatterProvider.Register(new BodySerializer());
            MemoryPackFormatterProvider.Register(new ItemDataSerializer());
        };

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
                .AddService(typeof(ILocalizedTextService), typeof(LocalizedTextService))
                .AddService(typeof(IDataFileLoaderService), typeof(DataFileLoaderService), -1)
                .AddService(typeof(ISessionManagerService), typeof(SessionManagerService), 99)
                .AddService(typeof(INetworkService), typeof(NetworkService), 100)
                ;


            container
                .AddService(typeof(IPersistenceManager), typeof(MemoryPackPersistenceManager));

            container
                .AddService(typeof(IAccountManagerService), typeof(AccountManagerService))
                .AddService(typeof(IMapService), typeof(MapService))
                .AddService(typeof(IMobileService), typeof(MobileService))
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
            scriptEngine.AddScriptModule(typeof(ConsoleCommandModule));
        };


        moongateStartupServer.RegisterPacketsAndHandlers += networkService =>
        {
            networkService.RegisterPacket<SeedPacket>();
            networkService.RegisterPacket<LoginPacket>();
            networkService.RegisterPacket<SelectServerPacket>();
            networkService.RegisterPacket<GameServerLoginPacket>();
            networkService.RegisterPacket<CharacterCreationPacket>();
            networkService.RegisterPacket<CharacterSelectPacket>();
            networkService.RegisterPacket<ClientVersionPacket>();
            networkService.RegisterPacket<GeneralInformationPacket>();
            networkService.RegisterPacket<SingleClickPacket>();
            networkService.RegisterPacket<GetPlayerStatusPacket>();
            networkService.RegisterPacket<UpdateViewPublicHouseContentPacket>();

            networkService.RegisterPacketHandler<SeedPacket, LoginHandler>();
            networkService.RegisterPacketHandler<LoginPacket, LoginHandler>();
            networkService.RegisterPacketHandler<ClientVersionPacket, LoginHandler>();
            networkService.RegisterPacketHandler<SelectServerPacket, LoginHandler>();
            networkService.RegisterPacketHandler<GameServerLoginPacket, GameLoginHandler>();
            networkService.RegisterPacketHandler<CharacterCreationPacket, CharacterHandler>();
            networkService.RegisterPacketHandler<CharacterSelectPacket, CharacterHandler>();
        };

        moongateStartupServer.BeforeStart += container =>
        {
            var dataLoaderService = container.Resolve<IDataFileLoaderService>();

            dataLoaderService.AddDataLoaderType(typeof(ServerClientVersionLoader));
            dataLoaderService.AddDataLoaderType(typeof(ExpansionLoader));
            dataLoaderService.AddDataLoaderType(typeof(SkillInfoLoader));
            dataLoaderService.AddDataLoaderType(typeof(ProfessionsLoader));
            dataLoaderService.AddDataLoaderType(typeof(TileDataLoader));
            dataLoaderService.AddDataLoaderType(typeof(RaceLoader));
            dataLoaderService.AddDataLoaderType(typeof(MultiDataLoader));
            dataLoaderService.AddDataLoaderType(typeof(BodyDataLoader));

            dataLoaderService.AddDataLoaderType(typeof(MapLoader));
        };


        moongateStartupServer.Init();

        await moongateStartupServer.StartAsync();
    }
);
