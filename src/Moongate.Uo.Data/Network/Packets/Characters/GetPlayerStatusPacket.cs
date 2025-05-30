using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class GetPlayerStatusPacket : IUoNetworkPacket
{
    public byte OpCode => 0x34;
    public int Length => 10;

    public GetPlayerStatusType StatusType { get; set; }

    public Serial MobileId { get; set; } = Serial.Zero;

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        reader.ReadInt32();
        StatusType = (GetPlayerStatusType)reader.ReadByte();
        MobileId = new Serial(reader.ReadUInt32());

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;

    }

    public override string ToString()
    {
        return $"{nameof(GetPlayerStatusPacket)}: StatusType={StatusType}, MobileId={MobileId}";
    }
}
