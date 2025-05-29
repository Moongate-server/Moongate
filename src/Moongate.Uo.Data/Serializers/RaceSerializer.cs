using MemoryPack;
using Moongate.Uo.Data.Races.Base;

namespace Moongate.Uo.Data.Serializers;

public class RaceSerializer : MemoryPackFormatter<Race>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Race? value)
    {
        writer.WriteVarInt(value.RaceID);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Race? value)
    {
        var id = reader.ReadVarIntInt32();
        value =  Race.GetRace(id);
    }
}
