using System.Net;
using System.Net.NetworkInformation;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Network.Data;
using Moongate.Core.Network.Interfaces.Messages;
using Moongate.Core.Network.Servers.Tcp;
using Moongate.Core.Services.Base;
using Serilog;

namespace Moongate.Server.Services.System;

public class NetworkService : AbstractBaseMoongateStartStopService, INetworkService
{
    private readonly MoongateServerConfig _moongateServerConfig;
    private readonly MoonTcpServerOptions _moonTcpServerOptions = new();
    private readonly List<MoongateTcpServer> _loginServers = new();
    private readonly List<MoongateTcpServer> _gameServers = new();
    private readonly Dictionary<byte, List<INetworkService.PacketReceivedHandler>> _packetHandlers = new();
    private readonly Dictionary<byte, INetworkService.ByteReceivedHandler> _rawPacketHandlers = new();
    private readonly Dictionary<byte, int> _packetSizes = new();

    public NetworkService(MoongateServerConfig moongateServerConfig) : base(Log.ForContext<NetworkService>())
    {
        _moongateServerConfig = moongateServerConfig;

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

            if (!_rawPacketHandlers.TryGetValue(opcode, out var handler))
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


            var currentPacket = remainingBuffer[..packetSize];
            var packet = handler(server.Id, client.Id, currentPacket);

            if (packet == null)
            {
                Logger.Warning("Failed to parse packet with opcode: 0x{Opcode:X2}", opcode);
                break;
            }

            DispatchPacket(server.Id, client.Id, packet);

            remainingBuffer = remainingBuffer.Slice(packetSize);
        }


        if (remainingBuffer.Length > 0)
        {
            Logger.Debug("Remaining unprocessed data: {Length} bytes", remainingBuffer.Length);
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


    public void AddOpCodeHandler(byte opCode, int length, INetworkService.ByteReceivedHandler handler)
    {
        if (_rawPacketHandlers.ContainsKey(opCode))
        {
            Logger.Warning("Packet handler for opCode {OpCode} already exists. Overwriting.", opCode);
        }

        _rawPacketHandlers[opCode] = handler;

        if (_packetSizes.ContainsKey(opCode))
        {
            Logger.Warning("Packet size for opCode {OpCode} already exists. Overwriting.", opCode);
        }

        _packetSizes[opCode] = length;
    }

    private void DispatchPacket(string serverId, string sessionId, IUoNetworkPacket packet)
    {
        if (_packetHandlers.TryGetValue(packet.OpCode, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler(serverId, sessionId, packet);
            }
        }
        else
        {
            Logger.Warning("No handler found for packet with opCode: {OpCode:X2}", packet.OpCode);
        }
    }

    public void ChangeOpCodeSize(byte opCode, int length)
    {
        _packetSizes[opCode] = length;
    }

    public void AddPacketHandler(byte opCode, INetworkService.PacketReceivedHandler handler)
    {
        if (!_packetHandlers.TryGetValue(opCode, out var handlers))
        {
            handlers = new List<INetworkService.PacketReceivedHandler>();
            _packetHandlers[opCode] = handlers;
        }

        Logger.Verbose("Adding packet handler for opCode 0x{OpCode:X2}", opCode);
        handlers.Add(handler);
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
