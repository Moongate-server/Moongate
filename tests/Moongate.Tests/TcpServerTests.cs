using System.Net;
using Moongate.Core.Network.Data;
using Moongate.Core.Network.Servers.Tcp;

namespace Moongate.Tests;

[TestFixture]
public class TcpServerTests
{
    [Test]
    public Task TestTcpServer()
    {
        // Arrange
        var server = new MoongateTcpServer(
            new IPEndPoint(IPAddress.Loopback, 8080),
            new MoonTcpServerOptions
            {
                BufferSize = 8192,
                Backlog = 100
            }
        );

        // Act
        server.Start();


        // Cleanup
        server.Stop();

        // Assert that the server is stopped
        Assert.That(server.IsRunning, Is.False);

        return Task.CompletedTask;
    }

    [Test]
    public Task TestTcpServerWithCustomOptions()
    {
        // Arrange
        var server = new MoongateTcpServer(
            new IPEndPoint(IPAddress.Loopback, 8080),
            new MoonTcpServerOptions
            {
                BufferSize = 16384,
                Backlog = 200
            }
        );

        // Act
        server.Start();

        // Cleanup
        server.Stop();

        // Assert that the server is stopped
        Assert.That(server.IsRunning, Is.False);

        return Task.CompletedTask;
    }
}
