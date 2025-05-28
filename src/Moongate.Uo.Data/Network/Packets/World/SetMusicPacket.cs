using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.World;

public class SetMusicPacket : IUoNetworkPacket
{
    public byte OpCode => 0x6D;
    public int Length => 3;

    public MusicName Music { get; set; }

    public SetMusicPacket(MusicName music = MusicName.Approach)
    {
        Music = music;
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((short)Music);

        return writer.Span.ToArray();

    }
}
