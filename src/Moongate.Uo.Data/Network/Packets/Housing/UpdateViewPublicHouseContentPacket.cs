using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Housing;

public class UpdateViewPublicHouseContentPacket : IUoNetworkPacket
{
    public byte OpCode => 0xFB;
    public int Length => 2;

    public bool Show { get; set; }

    public bool Read(SpanReader reader)
    {

        reader.ReadByte();
        Show = reader.ReadBoolean();

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        throw new NotImplementedException();
    }
}
