# Change Log


<a name="0.6.0"></a>
## [0.6.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.6.0) (2025-05-30)

### Features

* **CircularBuffer.cs:** implement CircularBuffer class for managing circular buffers to improve data handling efficiency ([a097fe2](https://www.github.com/Moongate-server/Moongate/commit/a097fe283197a6eb2fa5a6453616047f03535ab6))
* **Moongate.Server.csproj:** add KeraLua package reference to enable Lua scripting support ([370fef6](https://www.github.com/Moongate-server/Moongate/commit/370fef6f0cef13a5d2901d53ebb58b424e235548))

<a name="0.5.0"></a>
## [0.5.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.5.0) (2025-05-29)

### Features

* add new serializers for ItemData and HealthBarColorType to improve data handling ([8392622](https://www.github.com/Moongate-server/Moongate/commit/8392622c4691858ff37c1c7775082025b830b863))
* **CharacterHandler.cs:** add support for new MobileIncomingPacket to handle ([affe949](https://www.github.com/Moongate-server/Moongate/commit/affe94998403a2416c4709fea64b37f4d2e7a04d))
* **CharacterHandler.cs:** add support for sending GeneralInformationPacket ([f858352](https://www.github.com/Moongate-server/Moongate/commit/f858352185a3b55154d00e18ce12ada22def041b))
* **Program.cs:** add GeneralInformationPacket to network service registration for ([cd621ea](https://www.github.com/Moongate-server/Moongate/commit/cd621ea3a0307854ac5150027b432316e72212f2))
* **server:** add new save files for accounts, characters, and mobiles ([3e3ff02](https://www.github.com/Moongate-server/Moongate/commit/3e3ff0207b24ea6330ae772b846163bde9577b79))

### Bug Fixes

* **Moongate.Core.csproj:** update ZLinq package version to 1.4.9 for bug fixes and improvements ([0da8105](https://www.github.com/Moongate-server/Moongate/commit/0da8105f202f417c92cdce4d4a813ba4830d588b))

<a name="0.4.0"></a>
## [0.4.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.4.0) (2025-05-27)

### Features

* **Entity.cs:** add MemoryPackable attribute to Entity class for serialization support ([2c70a0d](https://www.github.com/Moongate-server/Moongate/commit/2c70a0d99789ffa2784bef2874b7b2dad4ca9856))

<a name="0.3.0"></a>
## [0.3.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.3.0) (2025-05-27)

### Features

* **data:** add body type table configuration file for different body types ([b282936](https://www.github.com/Moongate-server/Moongate/commit/b28293613f0cb808841dc4d44ba8f85bdb0afd16))
* **data:** add bodyTable.cfg file with body type mappings for entities ([8099f90](https://www.github.com/Moongate-server/Moongate/commit/8099f907ace9a85eea9e4d2f793bc7fd7d30b96c))
* **GeneralInformationFactory.cs:** add GeneralInformationFactory class to create ([7bf69ac](https://www.github.com/Moongate-server/Moongate/commit/7bf69ac089fa403b3ee80cb1e6aa5dbdb6f1100c))
* **GeneralInformationPacket.cs:** add GeneralInformationPacket class implementing IUoNetworkPacket ([ac5c4ec](https://www.github.com/Moongate-server/Moongate/commit/ac5c4ecdc527e359d776954b394f833ad1a603d7))
* **Moongate.Server:** add support for new ItemEntity and MobileEntity classes ([ab6add9](https://www.github.com/Moongate-server/Moongate/commit/ab6add9bb018cbc8f7b82e1ba992444fab473fef))

<a name="0.2.0"></a>
## [0.2.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.2.0) (2025-05-27)

### Features

* added dictionary from UOX3 (thank you guys!) ([9ce3710](https://www.github.com/Moongate-server/Moongate/commit/9ce371015a372d1c7c22d8854092f190a7618915))
* added files from ModernUO ([b099d43](https://www.github.com/Moongate-server/Moongate/commit/b099d43b8e711773f123510ae4428c446a36168f))
* **CharacterHandler.cs:** add IMapService dependency to CharacterHandler constructor ([01b5224](https://www.github.com/Moongate-server/Moongate/commit/01b52249dd68f525a350ee43610cbac1145fcb75))
* **CollectionExtensions.cs:** add CollectionExtensions class with RandomElement method ([a32daea](https://www.github.com/Moongate-server/Moongate/commit/a32daea807dc020d6f6d72487486bf2b565e2ca9))
* **core:** add IDataFileLoaderService interface to support loading data files asynchronously ([6853479](https://www.github.com/Moongate-server/Moongate/commit/6853479a7be5664795c7eb6d66f33b03783bc20c))
* **data:** add new characters.moongate file to store character data ([0571f44](https://www.github.com/Moongate-server/Moongate/commit/0571f442303bf9bbe17c36e972542589c0b7acde))
* **DataLoader:** add IDataLoader interface to define data loading operations for generic types ([585d902](https://www.github.com/Moongate-server/Moongate/commit/585d90262f9eebb2bb88a43f833fed41d688c5b7))
* **DataLoaders:** add ExpansionLoader and SkillInfoLoader classes to load expansion and skill information from JSON files ([9acd418](https://www.github.com/Moongate-server/Moongate/commit/9acd418e5a08a701370fa75cf20a6c4697999b33))
* **Deflate.cs:** add Deflate class for compression using LibDeflateBinding ([53e5bb8](https://www.github.com/Moongate-server/Moongate/commit/53e5bb8818d262ef45ed558b52b4071f81a4189e))
* **Entry3D.cs, Entry5D.cs, UoFiles.cs:** add new data structures Entry3D and Entry5D for handling 3D and 5D entries in Moongate.Uo.Data.Files namespace ([8aa892f](https://www.github.com/Moongate-server/Moongate/commit/8aa892f8283f1ef2e1f81b637b4d57da5f0f2482))
* **Json:** add FlagsConverter class to handle JSON serialization of flag enums ([fb27362](https://www.github.com/Moongate-server/Moongate/commit/fb2736243f4eded3a13f6eeb17a7a4421271c47f))
* **Map.cs:** add new Map class to handle map data and registration ([ffd6434](https://www.github.com/Moongate-server/Moongate/commit/ffd6434b7f2b576e571f3618f3c5e582f0b00fc3))
* **moongate:** add support for specifying Ultima Online directory in Moongate ([9a8fd18](https://www.github.com/Moongate-server/Moongate/commit/9a8fd184ce4d276864a6838d9e56dd067e256eea))
* **Moongate:** add support for new packets CharacterSelectPacket and ClientVersionPacket ([6084b0b](https://www.github.com/Moongate-server/Moongate/commit/6084b0b146bc7d4fd2ce5a8b4fc6943e5ed1f73f))
* **Moongate.Core.csproj:** update ZLinq package version to 1.4.8 ([4e848e3](https://www.github.com/Moongate-server/Moongate/commit/4e848e3d50001f378725a178381d39fbe179eb5f))
* **Moongate.Uo.Data:** add new classes Skills, TileData, UOPEntry, UOPFiles, and Verdata to handle game data structures and files ([d632021](https://www.github.com/Moongate-server/Moongate/commit/d6320218f7f78b3bdd2a86ec3b188bd273c03f36))
* **Moongate.Uo.Data:** add new structs HuedTile, LandTile, MTile, MultiData, MultiTileEntry, MultiComponentList to handle tile data for Moongate game engine ([e9eade6](https://www.github.com/Moongate-server/Moongate/commit/e9eade6752333437088df24ca9ea42437da0f972))
* **Moongate.Uo.Data.csproj:** add AllowUnsafeBlocks property to enable unsafe code ([edea5bc](https://www.github.com/Moongate-server/Moongate/commit/edea5bc6534b59e771099f758f5072a788809e5e))
* **Mul:** add new Art class to handle art-related functionalities ([2cbe42a](https://www.github.com/Moongate-server/Moongate/commit/2cbe42a2d9b7bca99811875b6ae614ce90a69093))
* **Mul:** add support for loading and configuring multi data from UOP and MUL files ([3894e27](https://www.github.com/Moongate-server/Moongate/commit/3894e27537bffb8570b9c14c41f305f8bc476abf))
* **MultiDataLoader.cs:** add MultiDataLoader class to handle loading of ([a6d2e1d](https://www.github.com/Moongate-server/Moongate/commit/a6d2e1d217a9a329a6939f19af1a791ad9a69fe7))
* **NetClient.cs:** add HaveCompression property to NetClient class for compression support ([e39e591](https://www.github.com/Moongate-server/Moongate/commit/e39e5917f7ddb50ad5675a5c3312c742249886fd))
* **Network:** add CharacterCreationPacket to handle character creation process ([5467cee](https://www.github.com/Moongate-server/Moongate/commit/5467cee286fd41d5e924cd1d5b5cfb2b1db32755))
* **NewItemTileDataMul.cs, StaticTile.cs:** add new data structures for handling item tile data and static tiles in the game to improve organization and efficiency ([3a7cc1d](https://www.github.com/Moongate-server/Moongate/commit/3a7cc1d0c6cdd39b0277e39fe9a77042ca4fec5f))
* **Point2D.cs:** add a new file for the Point2D struct to represent 2D points ([1d15b93](https://www.github.com/Moongate-server/Moongate/commit/1d15b93b5f44342a72349e5f827fdaf478f5177d))
* **ProcessStats.cs:** simplify calculation of average processing time ([78f107b](https://www.github.com/Moongate-server/Moongate/commit/78f107b12787c13bd274a9b6bbcd3c5035a1590b))
* **ProfessionsLoader.cs:** add ProfessionsLoader class to load profession data from file ([6b98e16](https://www.github.com/Moongate-server/Moongate/commit/6b98e1605dab73dd5c40dee3d02548af3fba1842))
* **Program.cs:** register CharacterEntity type in addition to AccountEntity for serialization ([bb0f843](https://www.github.com/Moongate-server/Moongate/commit/bb0f8438364ad12e165a6e2b46c3f1476a02449c))
* **Program.cs:** remove unnecessary blank lines for better code readability ([2ebd40c](https://www.github.com/Moongate-server/Moongate/commit/2ebd40ccb0355e382c4aaa6aa8b8851141ddc87e))
* **ResourceUtils.cs:** add method EmbeddedNameToPath to convert embedded resource ([51ebe91](https://www.github.com/Moongate-server/Moongate/commit/51ebe91bfdc014f83256e2518cfb16cab6f0313c))
* **ServerClientVersionLoader.cs:** update namespace reference for UoContext in ServerClientVersionLoader to match new location ([a3deb7e](https://www.github.com/Moongate-server/Moongate/commit/a3deb7e923e04ec79273a316c21d92a9e83b629b))
* **ShardConfig.cs:** add Language property with default value "eng" for ShardConfig ([e5d1e04](https://www.github.com/Moongate-server/Moongate/commit/e5d1e04d5f18eb71c99bc6567062e62c22d52149))
* **SkillGroup.cs, SkillInfo.cs:** add new classes SkillGroup and SkillInfo to handle skill-related data in the application ([958d107](https://www.github.com/Moongate-server/Moongate/commit/958d1070a56cafac8d4006c8b4d332a1d0fa210e))
* **TileMatrix.cs:** add new TileMatrix class to manage tile data for improved organization and efficiency ([db704e7](https://www.github.com/Moongate-server/Moongate/commit/db704e7d95b08195aa0757016ba25d4b439e5721))
* **Utility.cs:** add Utility class with various helper methods for string ([232bdd8](https://www.github.com/Moongate-server/Moongate/commit/232bdd83f226cd9296f622e1c5da03f3c39f18d8))

<a name="0.1.0"></a>
## [0.1.0](https://www.github.com/Moongate-server/Moongate/releases/tag/v0.1.0) (2025-05-23)

### Features

* add moongate.json configuration file with empty shard object ([4345846](https://www.github.com/Moongate-server/Moongate/commit/4345846cec8956cec07dcd117218317a16891ce9))
* **__index.lua:** add auto-generated Lua script for logger functionality ([b93f4fd](https://www.github.com/Moongate-server/Moongate/commit/b93f4fdbbce86d9eaf271cb4868eae31f6d03dfe))
* **__index.lua:** add functions to handle text template variables and builders ([7a3ab7a](https://www.github.com/Moongate-server/Moongate/commit/7a3ab7a22970794d630d9434c8374d8a4f592a08))
* **AccountManagerService.cs:** add Login method to AccountManagerService to handle user login functionality ([61e53dd](https://www.github.com/Moongate-server/Moongate/commit/61e53dd8bf27251971f66502dce02aca0f4f1384))
* **ARCHITECTURE.md:** add architectural decisions for Moongate project ([1b48312](https://www.github.com/Moongate-server/Moongate/commit/1b483120d0e7950ff8a112cde241436f21868473))
* **ARCHITECTURE.md:** update technology choices and components in ARCHITECTURE.md ([4025427](https://www.github.com/Moongate-server/Moongate/commit/40254274ce625aa6f9d465a473247818f4e0b9a2))
* **Attributes:** improve EntityTypeAttribute constructor to accept typeId parameter ([3581829](https://www.github.com/Moongate-server/Moongate/commit/3581829b010573a384840acf5017f6d9a1641f77))
* **CircularBuffer.cs:** add CircularBuffer class to Moongate.Core.Buffers ([7a2a380](https://www.github.com/Moongate-server/Moongate/commit/7a2a3809899053895712d408649a5a58d8f5ea0e))
* **CommandDefinitionData.cs:** add CommandDefinitionData record to store command details ([0f8d165](https://www.github.com/Moongate-server/Moongate/commit/0f8d1657a1ffd7d8071ffb61a5bcef503e86c8ff))
* **core:** add MoonTcpServerOptions class to handle buffer size and backlog ([6be3db3](https://www.github.com/Moongate-server/Moongate/commit/6be3db39ee546741dcc142b5d3677f12657680bc))
* **core:** add new classes and interfaces for event dispatching, metrics, ([24089c6](https://www.github.com/Moongate-server/Moongate/commit/24089c65f4f53bdea146791cbaab73c0c0e7e3e7))
* **core:** add new classes for handling diagnostic, scheduler, and server events ([afaad16](https://www.github.com/Moongate-server/Moongate/commit/afaad16e7c372530862387301bc5737802d91c63))
* **dependabot.yml:** add Dependabot configuration for NuGet packages and GitHub Actions to automate updates and pull requests ([fb80bcf](https://www.github.com/Moongate-server/Moongate/commit/fb80bcf6a52c810da95ac75dbc251cfb663749d9))
* **DirectoriesConfig.cs:** add DirectoriesConfig class to manage directory paths and creation ([45677f8](https://www.github.com/Moongate-server/Moongate/commit/45677f8fc27d5df0d421e93a3f68be544a8251aa))
* **EntityRegistrationBuilder.cs:** add static Instance property for singleton pattern ([dd4175f](https://www.github.com/Moongate-server/Moongate/commit/dd4175f44634c59d042fcd8cbfbe9a59729f2ba9))
* **EntityTypeAttribute.cs:** add EntityTypeAttribute to mark classes as persistable entities ([f9a22a8](https://www.github.com/Moongate-server/Moongate/commit/f9a22a85a7b0d4cf73a8a1b794cd2ee2f84c7204))
* **GameLoginHandler.cs:** add GameLoginHandler to handle game server login packets ([7b2aa48](https://www.github.com/Moongate-server/Moongate/commit/7b2aa480f8a15989424b18c1176cbc415bd98edf))
* **handlers:** add LoginHandler class to handle login packets in the server ([241a157](https://www.github.com/Moongate-server/Moongate/commit/241a157ad69d567446a873e3dd9e56e58381d6ba))
* **IMoongateService.cs:** add IMoongateService interface to define Moongate services contract ([24e7eaf](https://www.github.com/Moongate-server/Moongate/commit/24e7eaf7ce1458de792312032022cbb4c91646ff))
* **INetMiddleware.cs:** add missing newline in INetMiddleware interface ([2e56f5f](https://www.github.com/Moongate-server/Moongate/commit/2e56f5fd1d3d93d6d883768924b854128746c4c7))
* **IScriptEngineService.cs:** add AddEnum method to handle enum values in a more structured way ([e7fee68](https://www.github.com/Moongate-server/Moongate/commit/e7fee680207bb2c1b3cc207bf99b077a058934ae))
* **LoginHandler.cs:** add sending of LoginDeniedPacket with AccountBlocked reason when login is denied ([102370e](https://www.github.com/Moongate-server/Moongate/commit/102370e3261af261d226bea924b255b264fd6a2c))
* **Moongate:** add MoongateServerArgs class to store server configuration options ([1d7157e](https://www.github.com/Moongate-server/Moongate/commit/1d7157e939ecc0229249e5b69241c87ea7449fc6))
* **Moongate:** add support for publishing account events when created or logged in ([0898fd5](https://www.github.com/Moongate-server/Moongate/commit/0898fd5b14e753b5b6717b94265df39207e82e1e))
* **Moongate.Core:** add support for defining script function attributes with ([b808809](https://www.github.com/Moongate-server/Moongate/commit/b808809964255c7290f0fb9edfda2df101073ff2))
* **Moongate.Core:** implement STArrayPool for single-threaded unsafe usage ([8edf7ed](https://www.github.com/Moongate-server/Moongate/commit/8edf7ed0793ec2be1b069b87062bdf339225f0ef))
* **Moongate.Core.Network:** add IUoNetworkPacket interface for defining network packets ([0a7c7a8](https://www.github.com/Moongate-server/Moongate/commit/0a7c7a86b87c9c427af0d379b93ec5483c5a0826))
* **moongate.json:** add "logPackets" field to network configuration for enabling packet logging ([fad8a4d](https://www.github.com/Moongate-server/Moongate/commit/fad8a4d1d7aeb52ba702ab6ce4a49d850c7037c5))
* **moongate.json:** add network configuration with loginPort, gamePort, and isPingServerEnabled ([a0bb03e](https://www.github.com/Moongate-server/Moongate/commit/a0bb03edbce838dcb4d7e06988ae7d06b33f2462))
* **moongate.json:** add webServer section with port and enabled properties ([378a035](https://www.github.com/Moongate-server/Moongate/commit/378a0357e3d6ebf6eabbc8be5837f190bc3e56f8))
* **Moongate.Server:** add Entities folder to project structure for better organization ([d765b02](https://www.github.com/Moongate-server/Moongate/commit/d765b0214c92a48b4b75955ec69f1b7c4e84d629))
* **Moongate.sln:** add Moongate.Core.Web project to the solution ([977ec96](https://www.github.com/Moongate-server/Moongate/commit/977ec96eb13b06fc113b2fd659e54a7e21ecd2b8))
* **Moongate.sln:** add Moongate.Uo.Network project to the solution ([ed1676e](https://www.github.com/Moongate-server/Moongate/commit/ed1676edcab943b114419bd7cb6b491a992b6288))
* **Moongate.sln:** add Moongate.Uo.Services project to the solution ([32583a7](https://www.github.com/Moongate-server/Moongate/commit/32583a7427d1a97502b2e23279398f7879269469))
* **Moongate.sln:** add new project Moongate.Uo.Data to the solution ([62aac38](https://www.github.com/Moongate-server/Moongate/commit/62aac38a4ae3fc83426d02aed224ebc175f06214))
* **Moongate.sln:** rename Moongate.Core.Persistence project to Moongate.Persistence ([0c8d061](https://www.github.com/Moongate-server/Moongate/commit/0c8d06199a213b7f3b82f5b6edd8f4a892e8dfca))
* **NetClient.cs:** change exception throwing to returning false when not connected ([2531225](https://www.github.com/Moongate-server/Moongate/commit/25312250cd936a51900264e6817cd7f79b00d102))
* **Network:** add ClientConnectedEvent and ClientDisconnectedEvent records to handle client connections and disconnections ([48dc74b](https://www.github.com/Moongate-server/Moongate/commit/48dc74b2293aa61326bfbf1669ab1cd99849372c))
* **Network:** add INetInterceptor interface for network interception functionality ([c6476a9](https://www.github.com/Moongate-server/Moongate/commit/c6476a9dcaefa2b1b97034d9f6aeb92afd4488da))
* **Network:** add IpAddressExtensions to handle IP address serialization to integers ([e7aa16c](https://www.github.com/Moongate-server/Moongate/commit/e7aa16c77aaf55f22f7a207aed2d1f22fa355d54))
* **PooledRefList.cs:** add a new PooledRefList data structure to the Moongate.Core.Collections namespace to provide a variable-size list implementation using an array of objects. The list automatically increases its capacity as elements are added, and supports various methods for manipulation and retrieval of elements. ([7486033](https://www.github.com/Moongate-server/Moongate/commit/748603344bab53e27ee53575744829af0e8a4bd2))
* **ProcessQueueConfig.cs:** add ProcessQueueConfig class with MaxParallelTask property ([b1d80c4](https://www.github.com/Moongate-server/Moongate/commit/b1d80c4a935507fcee42eb8da306cc9756e68069))
* **Program.cs:** add registration of packet handler for SeedPacket to log client version ([0dec07a](https://www.github.com/Moongate-server/Moongate/commit/0dec07a596df06a37368d44f98029094f862fd3c))
* **Program.cs:** register SeedPacket class to handle packet with opcode 0xEF ([c99384c](https://www.github.com/Moongate-server/Moongate/commit/c99384cdf369fb82cf62569550ceb06569b37413))
* **run_aot.sh:** add new script 'run_aot.sh' to run ahead-of-time compilation ([0508e5d](https://www.github.com/Moongate-server/Moongate/commit/0508e5d8daf818c298c033f02ce2f5af2c290e84))
* **ScriptFunctionAttribute.cs:** modify ScriptFunctionAttribute constructor to accept helpText parameter and set it to the HelpText property ([0f61ef3](https://www.github.com/Moongate-server/Moongate/commit/0f61ef34c8906549f4ddf514301d10bc8c665bcc))
* **ScriptFunctionAttribute.cs, ScriptModuleAttribute.cs, ScriptEngineConfig.cs, EventLoopMetrics.cs, ScriptModuleData.cs, IMoongateService.cs, IEventLoopService.cs, IScriptEngineService.cs, ITimerService.cs, EventLoopPriority.cs:** add new classes and interfaces for script attributes, engine configuration, event loop metrics, script module data, services interfaces, and event loop priority types to enhance script management and event loop functionality. ([ff23679](https://www.github.com/Moongate-server/Moongate/commit/ff23679b6a0eed6c39bc50600806c74dc4daeade))
* **scripts:** add support for registering console commands in Lua scripts ([403ef98](https://www.github.com/Moongate-server/Moongate/commit/403ef981673a5731c9a4b2195976e4e0a23afa87))
* **scripts:** add TimerRegisterData class and related functions for timer management ([124b148](https://www.github.com/Moongate-server/Moongate/commit/124b148713199b061051b079dac1860ef4dfc03b))
* **server:** add new classes for DiagnosticServiceConfig, EventLoopConfig, ([533f285](https://www.github.com/Moongate-server/Moongate/commit/533f28523b248ebbb451790fe89dd7362fe21be1))
* **StringHelpers.cs:** add InsensitiveStringHelpers class to provide case-insensitive string comparison and manipulation methods ([4bd1aad](https://www.github.com/Moongate-server/Moongate/commit/4bd1aade841f7e271b843e049d77ba55b26c6b66))
* **StringMethodExtension.cs:** add extension methods for various string case conversions ([34c85dd](https://www.github.com/Moongate-server/Moongate/commit/34c85dd1c848a3b1a5aefe0c7060410dd9ae5479))
* **StringUtils.cs:** add utility methods for string case conversion (snake_case, camelCase, PascalCase, kebab-case, UPPER_SNAKE_CASE, Title Case) to improve code readability and maintainability ([33038d9](https://www.github.com/Moongate-server/Moongate/commit/33038d9b37cf107e5fbaaa24e83a767c2fa743ef))
* **ValueStringBuilder.cs:** add ValueStringBuilder class to Moongate.Core.Buffers namespace ([13eb230](https://www.github.com/Moongate-server/Moongate/commit/13eb2307b5b03d54175ff93b2124744b2b22f354))

### Bug Fixes

* **Moongate.Core.csproj:** correct package reference for DryIoc library to remove unnecessary '.Dll' suffix ([3e45897](https://www.github.com/Moongate-server/Moongate/commit/3e45897ed4865431ef189e06f72626c15299bd5d))

