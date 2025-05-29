using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class UpdateStatusBarPacket : IUoNetworkPacket
{
    public MobileEntity Mobile { get; set; }

    public byte OpCode => 0x17;
    public int Length => 12;

    public bool IsEnabled { get; set; }

    public HealthBarColorType HealthBarColor { get; set; }

    public UpdateStatusBarPacket(MobileEntity? mobile = null)
    {
        if (mobile is not null)
        {
            Mobile = mobile;
        }
    }

    public bool Read(SpanReader reader)
    {



        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        /**
        *
        *
       BYTE[1] 0x17
       BYTE[2] Length
       BYTE[4] Mobile Serial
       BYTE[2] 0x0001
       BYTE[2] HealthBar Color (1=green, 2=yellow, >2=red?)
       BYTE[1] Flag (0=Remove health bar color, 1=Enable health bar color)
        */


        writer.Write(OpCode);
        writer.Write((short)Length);
        writer.Write(Mobile.Serial);
        writer.Write((short)0x0001); // Unknown, always 1
        writer.Write((short)HealthBarColor); // Health bar color
        writer.Write((byte)(IsEnabled ? 1 : 0)); // Flag to enable or remove health bar color

        return writer.ToArray();
    }
}
