using System.Net;
using Moongate.Core.Extensions.Network;
using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets.Connection;

public class ConnectToGameServerPacket : IUoNetworkPacket
{
    public byte OpCode => 0x8C;
    public int Length => 11;

    /// <summary>
    /// Gets or sets the IP address of the game server to connect to.
    /// </summary>
    public IPAddress GameServerIP { get; set; } = IPAddress.None;

    /// <summary>
    /// Gets or sets the port of the game server to connect to.
    /// </summary>
    public ushort GameServerPort { get; set; }

    /// <summary>
    /// Gets or sets the session key to authenticate with the game server.
    /// This ensures the connection is coming from a properly authenticated client.
    /// </summary>
    public int SessionKey { get; set; }

    /// <summary>
    /// Reads the packet data from the provided packet reader.
    /// </summary>
    /// <param name="reader">The packet reader to read data from.</param>
    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.WriteLE(GameServerIP.ToRawAddress());
        writer.Write((short)GameServerPort);
        writer.Write(SessionKey);

        return writer.ToSpan().Span.ToArray();
    }
}
