using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;
using Moongate.Uo.Network.Types;

namespace Moongate.Uo.Network.Packets;

public class LoginDeniedPacket : IUoNetworkPacket
{
    public LoginDeniedReasonType Reason { get; set; }

    public byte OpCode => 0x82;
    public int Length => 2;

    public LoginDeniedPacket()
    {
    }

    public LoginDeniedPacket(LoginDeniedReasonType reason)
    {
        Reason = reason;
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((byte)Reason);

        return writer.Span.ToArray();
    }
}
