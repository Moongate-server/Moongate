using MemoryPack;
using Moongate.Uo.Data.Geometry;

namespace Moongate.Uo.Data.Serializers;

public class Point3dSerializer : MemoryPackFormatter<Point3D>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Point3D value)
    {
        writer.WriteVarInt(value.X);
        writer.WriteVarInt(value.Y);
        writer.WriteVarInt(value.Z);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Point3D value)
    {
        value.X = reader.ReadVarIntInt32();
        value.Y = reader.ReadVarIntInt32();
        value.Z = reader.ReadVarIntInt32();
    }
}
