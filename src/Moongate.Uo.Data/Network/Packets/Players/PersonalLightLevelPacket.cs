using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Players;

public class PersonalLightLevelPacket : IUoNetworkPacket
{
    public byte OpCode => 0x4E;
    public int Length => 6;

    public Serial MobileId { get; set; } = Serial.Zero;

    public LightLevelType LightLevel { get; set; } = LightLevelType.Day;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public PersonalLightLevelPacket(MobileEntity? mobile = null , LightLevelType lightLevel = LightLevelType.Day)
    {
        MobileId = mobile?.Serial ?? Serial.Zero;
        LightLevel = lightLevel;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(MobileId.Value);
        writer.Write((byte)LightLevel);

        return writer.ToArray();
    }
}
