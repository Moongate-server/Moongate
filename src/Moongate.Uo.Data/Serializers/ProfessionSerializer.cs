using MemoryPack;

namespace Moongate.Uo.Data.Serializers;

public class ProfessionSerializer : MemoryPackFormatter<ProfessionInfo>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ProfessionInfo? value)
    {
        writer.WriteVarInt(value.ID);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref ProfessionInfo? value)
    {
        var id = reader.ReadVarIntInt32();
        ProfessionInfo.GetProfession(id, out var profession);
        value = profession ?? throw new InvalidOperationException($"Profession with ID {id} not found.");
    }
}
