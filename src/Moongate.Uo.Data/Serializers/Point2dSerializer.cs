using MemoryPack;
using Moongate.Uo.Data.Geometry;

namespace Moongate.Uo.Data.Serializers;

public class Point2dSerializer : MemoryPackFormatter<Point2D>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Point2D value)
    {
        writer.WriteVarInt(value.X);
        writer.WriteVarInt(value.Y);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Point2D value)
    {
        value.X = reader.ReadVarIntInt32();
        value.Y = reader.ReadVarIntInt32();
    }
}
