using MemoryPack;

namespace Moongate.Uo.Data.Serializers;

public class BodySerializer : MemoryPackFormatter<Body>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Body value)
    {
        writer.WriteVarInt(value.BodyID);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Body value)
    {
        var id = reader.ReadVarIntInt32();

        value = new Body(id);
    }
}
