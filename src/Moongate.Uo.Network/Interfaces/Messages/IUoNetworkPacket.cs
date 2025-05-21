using Moongate.Core.Spans;

namespace Moongate.Uo.Network.Interfaces.Messages;

public interface IUoNetworkPacket
{
    byte OpCode { get; }

    void Read(SpanReader reader);

    ReadOnlyMemory<byte> Write(SpanWriter writer);

}
