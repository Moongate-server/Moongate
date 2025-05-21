using Moongate.Core.Spans;

namespace Moongate.Uo.Network.Interfaces.Messages;

public interface IUoNetworkPacket
{
    byte OpCode { get; }

    int Length { get; }

    bool Read(SpanReader reader);

    ReadOnlyMemory<byte> Write(SpanWriter writer);

}
