using MemoryPack;

namespace Moongate.Uo.Data.Serializers;

public class MapSerializer : MemoryPackFormatter<Map>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Map? value)
    {
        writer.WriteVarInt(value.MapID);

        // Serialize other properties as needed
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Map? value)
    {
        var id = reader.ReadVarIntInt32();
        value = Map.GetMap(id);
    }
}
