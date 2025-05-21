using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Events.Network;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Network.Data;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Core.Services.Base;
using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Services.System;

public class NetworkService : AbstractBaseMoongateStartStopService, INetworkService
{
    private readonly bool _useEventLoop = true;

    public event INetworkService.ClientConnectedDelegate? ClientConnected;
    public event INetworkService.ClientDisconnectedDelegate? ClientDisconnected;

    private readonly Dictionary<byte, List<INetworkService.PacketReceivedDelegate>> _packetHandlers = new();

    private readonly ISessionManagerService _sessionManagerService;
    private readonly IEventLoopService _eventLoopService;
    private readonly MoongateServerConfig _moongateServerConfig;
    private readonly MoonTcpServerOptions _moonTcpServerOptions = new();
    private readonly List<MoongateTcpServer> _loginServers = new();
    private readonly List<MoongateTcpServer> _gameServers = new();

    private readonly Dictionary<byte, Func<IUoNetworkPacket>> _packets = new();
    private readonly Dictionary<byte, int> _packetSizes = new();


    private readonly ConcurrentDictionary<string, NetClient> _clients = new();
    private readonly IEventBusService _eventBusService;

    public NetworkService(
        MoongateServerConfig moongateServerConfig, IEventBusService eventBusService,
        ISessionManagerService sessionManagerService, IEventLoopService eventLoopService
    ) : base(
        Log.ForContext<NetworkService>()
    )
    {
        _moongateServerConfig = moongateServerConfig;
        _eventBusService = eventBusService;
        _sessionManagerService = sessionManagerService;
        _eventLoopService = eventLoopService;

        PrepareTcpServers();
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

            Logger.Debug("Added session Id {Session}", obj.Id);
            ClientConnected?.Invoke(server.Id, obj.Id, obj);
            _eventBusService.PublishAsync(new ClientConnectedEvent(server.Id, obj.Id));
        }
    }

    private void ParsePacket(MoongateTcpServer server, NetClient client, ReadOnlyMemory<byte> buffer)
    {
        var remainingBuffer = buffer;

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

                headerSize = 2;
                packetSize = span[1] + headerSize;

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
            "Dispatching packet with opcode: 0x{Opcode:X2} from client {ClientId}: {Content}",
            packet.OpCode,
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

        Logger.Debug("Registered packet with opCode 0x{OpCode:X2}", opCode);
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


    public static IEnumerable<IPEndPoint> GetListeningAddresses(IPEndPoint ipep) =>
        NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(adapter =>
                adapter.GetIPProperties()
                    .UnicastAddresses
                    .Where(uip => ipep.AddressFamily == uip.Address.AddressFamily)
                    .Select(uip => new IPEndPoint(uip.Address, ipep.Port))
            );
}
