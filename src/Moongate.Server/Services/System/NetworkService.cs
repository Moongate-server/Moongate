using System.Net;
using System.Net.NetworkInformation;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Network.Data;
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
            _loginServers.Add(new MoongateTcpServer($"login_{index}", endPoint, _moonTcpServerOptions));
            _gameServers.Add(
                new MoongateTcpServer(
                    $"game_{index}",
                    new IPEndPoint(endPoint.Address, _moongateServerConfig.Network.GamePort),
                    _moonTcpServerOptions
                )
            );
            index++;
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

    public static IEnumerable<IPEndPoint> GetListeningAddresses(IPEndPoint ipep) =>
        NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(adapter =>
                adapter.GetIPProperties()
                    .UnicastAddresses
                    .Where(uip => ipep.AddressFamily == uip.Address.AddressFamily)
                    .Select(uip => new IPEndPoint(uip.Address, ipep.Port))
            );
}
