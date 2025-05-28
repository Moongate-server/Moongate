using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.World;

public class CurrentTimePacket : IUoNetworkPacket
{
    public byte OpCode => 0x5B;
    public int Length => 4;
    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((byte)DateTime.Now.Hour); // Hour
        writer.Write((byte)DateTime.Now.Minute); // Minute
        writer.Write((byte)DateTime.Now.Second); // Second

        return writer.Span.ToArray();
    }
}
