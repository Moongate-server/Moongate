using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.World;

public class OverallLightLevelPacket : IUoNetworkPacket
{
    public LightLevelType LightLevel { get; set; }
    public byte OpCode => 0x4F;
    public int Length => 2;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((byte)LightLevel);


        return writer.ToArray();
    }
}
