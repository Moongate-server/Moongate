using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class CharacterSelectPacket : IUoNetworkPacket
{
    public byte OpCode => 0x5D;
    public int Length => 73;


    public string Name { get; set; }

    public CharacterCreateFlags ClientFlags { get; set; }

    public int LoginCount { get; set; }

    public int Slot { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();

        reader.ReadInt32(); // (0xedededed)

        Name = reader.ReadAscii(30);

        reader.ReadBytes(2);

        ClientFlags = (CharacterCreateFlags)reader.ReadInt32();

        reader.ReadInt32();

        LoginCount = reader.ReadInt32();

        reader.ReadBytes(16);

        Slot = reader.ReadInt32();
        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        throw new NotImplementedException();
    }
}
