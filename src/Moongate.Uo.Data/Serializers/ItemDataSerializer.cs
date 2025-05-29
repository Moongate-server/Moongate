using MemoryPack;
using Moongate.Uo.Data.Mul;

namespace Moongate.Uo.Data.Serializers;

public class ItemDataSerializer : MemoryPackFormatter<ItemData>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ItemData value)
    {
        var name = value.Name;
        writer.WriteString(name);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref ItemData value)
    {
        var name = reader.ReadString();
        value = TileData.ItemDataByName(name);
    }
}
