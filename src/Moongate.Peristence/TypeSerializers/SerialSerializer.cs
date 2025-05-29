using System.Buffers;
using MemoryPack;
using Moongate.Core.Data.Ids;

namespace Moongate.Persistence.TypeSerializers;

public class SerialSerializer : MemoryPackFormatter<Serial>
{
    public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Serial value)
    {
        writer.WriteVarInt(value.Value);
    }

    public override void Deserialize(ref MemoryPackReader reader, scoped ref Serial value)
    {
        var val = reader.ReadVarIntUInt32();

        value = new Serial(val);
    }
}
