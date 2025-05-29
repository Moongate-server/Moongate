using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class MobileStatusPacket : IUoNetworkPacket
{
    public MobileEntity Mobile { get; set; }
    public byte OpCode => 0x11;
    public int Length => -1;

    public bool CanBeRenamed { get; set; } = true;
    public int Version { get; set; } = 0;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public MobileStatusPacket(MobileEntity? mobile = null)
    {
        if (mobile != null)
        {
            Mobile = mobile;
        }
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {

        var name = Mobile.Name ?? "";

        writer.Write((byte)0x11); // Packet ID
        writer.Seek(2, SeekOrigin.Current);
        writer.Write(Mobile.Serial);
        writer.WriteAscii(name, 30);
        writer.WriteAttribute(90, 10, Version == 0, true);
        writer.Write(CanBeRenamed);
        writer.Write((byte)Version);

        if (Version <= 0)
        {
            writer.WritePacketLength();
            return writer.ToArray();
        }

        writer.Write(Mobile.Female);
        writer.Write((short)Mobile.Strength);
        writer.Write((short)Mobile.Dexterity);
        writer.Write((short)Mobile.Intelligence);

        writer.Write((short)Mobile.Stamina);
        writer.Write((short)Mobile.MaxStamina);
        writer.Write((short)Mobile.Mana);
        writer.Write((short)Mobile.MaxStamina);
        // Total gold
        writer.Write(100);

        // writer.Write((short)(Core.AOS ? beheld.PhysicalResistance : (int)(beheld.ArmorRating + 0.5)));
        // writer.Write((short)(Mobile.BodyWeight + beheld.TotalWeight));

        writer.Write((short)10);
        writer.Write((short)10);

        if (Version >= 5)
        {
            writer.Write((short)Mobile.MaxWeight);
            writer.Write((byte)(Mobile.Race?.RaceID + 1 ?? 0)); // Would be 0x00 if it's a non-ML enabled account but...
        }

        // writer.Write((short)beheld.StatCap);
        //
        // writer.Write((byte)beheld.Followers);
        // writer.Write((byte)beheld.FollowersMax);

        writer.Write((short)100);

        writer.Write((byte)0);
        writer.Write((byte)1);

        return writer.ToArray();
    }
}
