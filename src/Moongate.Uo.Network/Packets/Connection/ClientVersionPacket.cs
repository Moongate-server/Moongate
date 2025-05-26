using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets.Connection;

public class ClientVersionPacket : IUoNetworkPacket
{
    public byte OpCode => 0xBD;
    public int Length => -1;

    public string Version { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        reader.ReadInt16();

        Version = reader.ReadAscii(reader.Remaining);
        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((ushort)(3));

        return writer.ToArray();
    }
}
