using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using DryIoc;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Events.Network;
using Moongate.Core.Directories;
using Moongate.Core.Extensions.Buffers;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Network.Data;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Core.Services.Base;
using Moongate.Core.Spans;
using Moongate.Core.Types;
using Moongate.Uo.Network.Compression;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Handlers;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Services.System;

public class NetworkService : AbstractBaseMoongateStartStopService, INetworkService
{
    private const bool _useEventLoop = true;

    public event INetworkService.ClientConnectedDelegate? ClientConnected;
    public event INetworkService.ClientDisconnectedDelegate? ClientDisconnected;

    private readonly ISchedulerSystemService _schedulerSystem;

    private readonly Dictionary<byte, List<INetworkService.PacketReceivedDelegate>> _packetHandlers = new();

    private readonly ISessionManagerService _sessionManagerService;
    private readonly DirectoriesConfig _directoriesConfig;
    private readonly IEventLoopService _eventLoopService;
    private readonly MoongateServerConfig _moongateServerConfig;
    private readonly MoonTcpServerOptions _moonTcpServerOptions = new();
    private readonly List<MoongateTcpServer> _loginServers = new();
    private readonly List<MoongateTcpServer> _gameServers = new();

    private readonly Dictionary<byte, Func<IUoNetworkPacket>> _packets = new();
    private readonly Dictionary<byte, int> _packetSizes = new();

    private readonly ConcurrentDictionary<int, WaitingLoginSession> _inLoginWaitingSessions = new();

    private readonly IContainer _container;
    private readonly ConcurrentDictionary<string, NetClient> _clients = new();
    private readonly IEventBusService _eventBusService;

    public NetworkService(
        MoongateServerConfig moongateServerConfig, IEventBusService eventBusService,
        ISessionManagerService sessionManagerService, IEventLoopService eventLoopService, IContainer container,
        DirectoriesConfig directoriesConfig, ISchedulerSystemService schedulerSystem
    ) : base(
        Log.ForContext<NetworkService>()
    )
    {
        _moongateServerConfig = moongateServerConfig;
        _eventBusService = eventBusService;
        _sessionManagerService = sessionManagerService;
        _eventLoopService = eventLoopService;
        _container = container;
        _directoriesConfig = directoriesConfig;
        _schedulerSystem = schedulerSystem;

        PrepareTcpServers();
        StartWaitingSessionsCleaner();
    }

    private void StartWaitingSessionsCleaner()
    {
        _schedulerSystem.RegisterJob(
            "networkWaitingSessionsCleaner",
            () =>
            {
                foreach (var session in _inLoginWaitingSessions)
                {
                    if (session.Value.IsExpired)
                    {
                        Logger.Debug("Removing expired session with AuthSession: {AuthId}", session.Key);
                        _inLoginWaitingSessions.TryRemove(session.Key, out _);
                    }
                }

                return Task.CompletedTask;
            },
            TimeSpan.FromMinutes(1)
        );
    }

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var server in _loginServers)
        {
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error starting login server {ServerId}", server.Id);
            }
        }

        foreach (var server in _gameServers)
        {
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error starting game server {ServerId}", server.Id);
            }
        }

        return Task.CompletedTask;
    }

    private void PrepareTcpServers()
    {
        var index = 0;
        foreach (var endPoint in GetListeningAddresses(
                     new IPEndPoint(IPAddress.Any, _moongateServerConfig.Network.LoginPort)
                 ))
        {
            var loginServer = new MoongateTcpServer($"login_{index}", endPoint, _moonTcpServerOptions);
            loginServer.OnClientDataReceived += (client, memory) => ParsePacket(loginServer, client, memory);
            loginServer.OnClientConnected += client => OnClientConnected(loginServer, client);
            loginServer.OnClientDisconnected += client => OnClientDisconnected(loginServer, client);


            var gameServer = new MoongateTcpServer(
                $"game_{index}",
                new IPEndPoint(endPoint.Address, _moongateServerConfig.Network.GamePort),
                _moonTcpServerOptions
            );

            gameServer.OnClientDataReceived += (client, memory) => ParsePacket(loginServer, client, memory);
            gameServer.OnClientConnected += client => OnClientConnected(loginServer, client);
            gameServer.OnClientDisconnected += client => OnClientDisconnected(loginServer, client);


            _loginServers.Add(loginServer);
            _gameServers.Add(gameServer);
            index++;
        }
    }

    private void OnClientDisconnected(MoongateTcpServer server, NetClient obj)
    {
        if (!_clients.TryRemove(obj.Id, out var client))
        {
            Logger.Warning("Client {ClientId} not found in the list of connected clients", obj.Id);
        }

        var sessionData = _sessionManagerService.GetSession(obj.Id);

        if (sessionData.PutInLimbo && sessionData.AuthId >= 0)
        {
            _inLoginWaitingSessions.TryAdd(sessionData.AuthId, new WaitingLoginSession(sessionData, DateTime.UtcNow));

            Logger.Debug("Client {ClientId} put in limbo with AuthSessionKey: {SessionKey}", obj.Id, sessionData.AuthId);
        }

        sessionData.OnSendPacket -= SendPacket;
        ClientDisconnected?.Invoke(server.Id, obj.Id);
        _eventBusService.PublishAsync(new ClientDisconnectedEvent(server.Id, obj.Id));
        _sessionManagerService.DeleteSession(obj.Id);
    }

    private void OnClientConnected(MoongateTcpServer server, NetClient obj)
    {
        if (_clients.TryAdd(obj.Id, obj))
        {
            var session = _sessionManagerService.CreateSession(obj.Id);
            session.Client = obj;
            session.OnSendPacket += SendPacket;

            Logger.Debug("Added session Id {Session}", obj.Id);
            ClientConnected?.Invoke(server.Id, obj.Id, obj);
            _eventBusService.PublishAsync(new ClientConnectedEvent(server.Id, obj.Id));
        }
    }

    private void SendPacket(NetClient client, IUoNetworkPacket packet)
    {
        try
        {
            var realLength = packet.Length;

            if (realLength == -1)
            {
                realLength = 16;
            }

            using var packetBuffer = new SpanWriter(stackalloc byte[realLength], realLength == 16);

            var bufferToSend = packet.Write(packetBuffer);

            LogPacket(client.Id, bufferToSend, false, client.HaveCompression);

            SendBuffer(client, bufferToSend);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error sending packet to client {ClientId}", client.Id);
        }
    }

    private void SendBuffer(NetClient client, ReadOnlyMemory<byte> buffer)
    {
        try
        {
            if (_useEventLoop)
            {
                _eventLoopService.EnqueueAction(
                    $" network_send_{client.Id}",
                    () => client.Send(buffer)
                );
            }
            else
            {
                client.Send(buffer);
            }

            client.Send(buffer);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error sending buffer to client {ClientId}", client.Id);
        }
    }

    private void ParsePacket(MoongateTcpServer server, NetClient client, ReadOnlyMemory<byte> buffer)
    {
        var remainingBuffer = buffer;

        /// FIXME: Not beautiful, but it works
        if (remainingBuffer.Length == 69)
        {
            remainingBuffer = remainingBuffer[4..];
        }

        while (remainingBuffer.Length > 0)
        {
            var span = remainingBuffer.Span;

            if (span.Length < 1)
            {
                break;
            }

            byte opcode = span[0];

            if (!_packets.TryGetValue(opcode, out var handler))
            {
                Logger.Warning("Received unknown packet opcode: 0x{Opcode:X2}", opcode);
                break;
            }

            if (!_packetSizes.TryGetValue(opcode, out var expectedSize))
            {
                Logger.Warning("No size defined for packet opcode: 0x{Opcode:X2}", opcode);
                break;
            }

            var headerSize = 1;
            int packetSize;

            if (expectedSize == -1)
            {
                if (span.Length < 2)
                {
                    break;
                }

                headerSize = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(1, 3));
                packetSize = headerSize;

                if (span.Length < packetSize)
                {
                    break;
                }
            }
            else
            {
                packetSize = expectedSize;

                if (span.Length < packetSize)
                {
                    break;
                }
            }


            var currentPacketBuffer = remainingBuffer[..packetSize];
            LogPacket(client.Id, currentPacketBuffer, true);

            using var packetBuffer = new SpanReader(currentPacketBuffer.Span);

            var packet = handler();

            try
            {
                var success = packet.Read(packetBuffer);

                if (!success)
                {
                    Logger.Warning("Failed to read packet with opcode: 0x{Opcode:X2}", opcode);
                    break;
                }

                DispatchPacket(server.Id, client.Id, packet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error reading packet with opcode: 0x{Opcode:X2}", opcode);
            }


            remainingBuffer = remainingBuffer[packetSize..];
        }


        if (remainingBuffer.Length > 0)
        {
            Logger.Debug("Remaining unprocessed data: {Length} bytes", remainingBuffer.Length);
        }
    }


    private void DispatchPacket(string serverId, string clientId, IUoNetworkPacket packet)
    {
        // Dispatch the packet to the appropriate handler
        // This is where you would implement your packet handling logic
        Logger.Debug(
            "Dispatching packet with opcode: {OpCode} from client {ClientId}: {Content}",
            packet.OpCode.FormatHexValue(),
            clientId,
            packet.ToString()
        );

        var handlers = _packetHandlers.GetValueOrDefault(packet.OpCode);

        if (handlers != null)
        {
            var session = _sessionManagerService.GetSession(clientId);
            foreach (var handler in handlers)
            {
                if (_useEventLoop)
                {
                    _eventLoopService.EnqueueAction("NetworkService", () => handler(session, packet));
                }
                else
                {
                    try
                    {
                        handler(session, packet);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error processing packet with opcode: 0x{Opcode:X2}", packet.OpCode);
                    }
                }
            }
        }
        else
        {
            Logger.Warning("No handlers registered for packet with opcode: 0x{Opcode:X2}", packet.OpCode);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var server in _loginServers)
        {
            try
            {
                server.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error stopping login server {ServerId}", server.Id);
            }
        }

        foreach (var server in _gameServers)
        {
            try
            {
                server.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error stopping game server {ServerId}", server.Id);
            }
        }

        return Task.CompletedTask;
    }


    public void ChangeOpCodeSize(byte opCode, int length)
    {
        _packetSizes[opCode] = length;
    }

    public void RegisterPacket<TPacket>() where TPacket : IUoNetworkPacket, new()
    {
        var packet = new TPacket();
        var opCode = packet.OpCode;

        if (_packets.ContainsKey(opCode))
        {
            Logger.Warning("Packet with opCode {OpCode} already registered. Overwriting.", opCode);
        }

        _packets[opCode] = () => new TPacket();
        _packetSizes[opCode] = packet.Length;

        Logger.Debug(
            "Registered packet with opCode {OpCode:X2} ({Type})",
            "0x" + opCode.ToString("X2"),
            typeof(TPacket).Name
        );
    }

    public void RegisterPacketHandler<TPacket>(INetworkService.PacketReceivedDelegate handler)
        where TPacket : IUoNetworkPacket, new()
    {
        var packet = new TPacket();
        var opCode = packet.OpCode;

        if (!_packetHandlers.TryGetValue(opCode, out List<INetworkService.PacketReceivedDelegate>? value))
        {
            value = new List<INetworkService.PacketReceivedDelegate>();
            _packetHandlers[opCode] = value;
        }

        value.Add(handler);

        Logger.Debug("Registered packet handler for opCode 0x{OpCode:X2}", opCode);
    }

    public void RegisterPacketHandler<TPacket, THandler>()
        where TPacket : IUoNetworkPacket, new() where THandler : IPacketListener
    {
        var packet = new TPacket();
        var opCode = packet.OpCode;

        if (!_packetHandlers.TryGetValue(opCode, out List<INetworkService.PacketReceivedDelegate>? value))
        {
            value = [];
            _packetHandlers[opCode] = value;
        }

        if (!_container.IsRegistered<THandler>())
        {
            _container.Register<THandler>(Reuse.Singleton);
        }

        var handler = _container.Resolve<THandler>();


        value.Add((session, networkPacket) =>
            {
                if (_useEventLoop)
                {
                    _eventLoopService.EnqueueAction(
                        $"packet_listener_{typeof(THandler).Name}",
                        () => handler.OnPacketReceivedAsync(session, networkPacket).Wait()
                    );
                }
                else
                {
                    Task.Run(async () => await handler.OnPacketReceivedAsync(session, networkPacket));
                }
            }
        );

        Logger.Debug(
            "Registered packet handler (via DI) for opCode {OpCode} in handler name: {HandleTypeName}",
            "0x" + opCode.ToString("X2"),
            typeof(THandler).Name
        );
    }


    public void Send(string sessionId, ReadOnlyMemory<byte> buffer)
    {
        if (_clients.TryGetValue(sessionId, out var client))
        {
            client.Send(buffer);
        }
        else
        {
            Logger.Warning("Client {ClientId} not found in the list of connected clients", sessionId);
        }
    }

    public NetClient? GetClient(string sessionId, bool throwIfNotFound = true)
    {
        if (_clients.TryGetValue(sessionId, out var client))
        {
            return client;
        }

        if (throwIfNotFound)
        {
            throw new KeyNotFoundException($"Client with session ID {sessionId} not found.");
        }

        return null;
    }

    public SessionData? GetInLimboSession(int sessionAuthId)
    {
        return _inLoginWaitingSessions.GetValueOrDefault(sessionAuthId)?.SessionData;
    }

    public bool RemoveInLimboSession(int sessionAuthId)
    {
        return _inLoginWaitingSessions.Remove(sessionAuthId, out _);
    }

    private void LogPacket(string sessionId, ReadOnlyMemory<byte> buffer, bool IsReceived, bool haveCompression = false)
    {
        if (!_moongateServerConfig.Network.LogPackets)
        {
            return;
        }

        var logPath = Path.Combine(_directoriesConfig[DirectoryType.Logs], "packets.log");

        using var sw = new StreamWriter(logPath, true);

        var direction = IsReceived ? "<-" : "->";
        var opCode = "OPCODE: 0x" + buffer.Span[0].ToString("X2");

        int compressionSize = 0;
        if (haveCompression)
        {
            var tmpInBuffer = buffer.Span.ToArray();
            Span<byte> tmpOutBuffer = stackalloc byte[tmpInBuffer.Length];
            compressionSize = NetworkCompression.Compress(tmpInBuffer, tmpOutBuffer);
        }

        Logger.Debug(
            "{Direction} {SessionId} {OpCode} | Data size: {DataSize} bytes | Compression: {Compression}, Compression Size: {CompressionSize}",
            direction,
            sessionId,
            opCode,
            buffer.Length,
            haveCompression,
            compressionSize
        );

        sw.WriteLine(
            $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {opCode}  | {direction} | Session ID: {sessionId} | Data size: {buffer.Length} bytes"
        );
        sw.FormatBuffer(buffer.Span);
        sw.WriteLine(new string('-', 50));
    }


    public static IEnumerable<IPEndPoint> GetListeningAddresses(IPEndPoint ipep) =>
        NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(adapter =>
                adapter.GetIPProperties()
                    .UnicastAddresses
                    .Where(uip => ipep.AddressFamily == uip.Address.AddressFamily)
                    .Select(uip => new IPEndPoint(uip.Address, ipep.Port))
            );
}

internal record WaitingLoginSession(SessionData SessionData, DateTime CreatedAt)
{
    public bool IsExpired => (DateTime.UtcNow - CreatedAt).TotalSeconds > 60;
}
