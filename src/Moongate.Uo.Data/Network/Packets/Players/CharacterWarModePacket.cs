using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Players;

public class CharacterWarModePacket : IUoNetworkPacket
{
    public byte OpCode => 0x72;
    public int Length => 5;
    public bool IsWarMode { get; set; }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(IsWarMode);
        writer.Write((byte)0);
        writer.Write((byte)0x32);
        writer.Write((byte)0);


        return writer.ToArray();
    }
}
