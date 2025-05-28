using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Seasons;

public class SeasonPacket : IUoNetworkPacket
{
    public byte OpCode => 0xBC;
    public int Length => 3;

    public Season Season { get; set; }
    public bool PlaySounds { get; set; }

    public SeasonPacket(Season season = Season.Spring, bool playSounds = false)
    {
        Season = season;
        PlaySounds = playSounds;
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write((byte)Season);
        writer.Write(PlaySounds);

        return writer.Span.ToArray();
    }
}
