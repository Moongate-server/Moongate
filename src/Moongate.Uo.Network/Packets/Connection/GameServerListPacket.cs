using Moongate.Core.Extensions.Network;
using Moongate.Core.Spans;
using Moongate.Uo.Network.Data.Entries;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets.Connection;

public class GameServerListPacket : IUoNetworkPacket
{
    public byte OpCode => 0xA8;
    public int Length => -1;

    public byte SystemInfoFlag { get; set; } = 0x5D;

    /// <summary>
    /// Collection of game servers to be included in the list.
    /// Maximum of 255 servers can be included.
    /// </summary>
    public List<GameServerEntry> Servers { get; } = new();

    /// <summary>
    /// Adds a server to the list of available game servers.
    /// Only adds the server if the list contains fewer than 255 servers.
    /// </summary>
    /// <param name="server">The server entry to add to the list.</param>
    public void AddServer(GameServerEntry server)
    {
        if (Servers.Count < 255)
        {
            Servers.Add(server);
        }
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        var info = Servers.ToArray();
        var length = 6 + 40 * info.Length;
        writer.Write((ushort)length);
        writer.Write((byte)0x5D);
        writer.Write((ushort)info.Length);

        for (var i = 0; i < info.Length; ++i)
        {
            var si = info[i];

            writer.Write((ushort)i);
            writer.WriteAscii(si.Name, 32);
            writer.Write((byte)si.LoadPercent);
            writer.Write((sbyte)si.TimeZone);
            // UO only supports IPv4
            writer.Write(si.IP.ToRawAddress());
        }

        return writer.Span.ToArray();

    }

    public override string ToString()
    {
        return base.ToString() + $" {{ SystemInfoFlag: 0x{SystemInfoFlag:X2}, ServerCount: {Servers.Count} }}";
    }
}
