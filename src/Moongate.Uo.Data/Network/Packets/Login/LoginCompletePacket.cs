using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Login;

public class LoginCompletePacket : IUoNetworkPacket
{
    public byte OpCode => 0x55;
    public int Length => 1;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);

        return writer.ToArray();
    }
}
