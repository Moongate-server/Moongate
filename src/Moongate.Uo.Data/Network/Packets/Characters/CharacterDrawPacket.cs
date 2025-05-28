using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class CharacterDrawPacket : IUoNetworkPacket
{
    public byte OpCode => 0x78;
    public int Length => -1;

    public MobileEntity Mobile { get; set; }

    public CharacterDrawPacket(MobileEntity mobile = null)
    {
        Mobile = mobile;
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        var length = 20;

        writer.Write(OpCode);
        foreach (var (layer, item) in Mobile.GetItems())
        {
            length += 7;

            if (item.Hue != 0)
            {
                length += 2;
            }
        }

        writer.Write(length);

        writer.Write(Mobile.Serial.Value);
        writer.Write((short)Mobile.Race.MaleBody);
        writer.Write((short)Mobile.X);
        writer.Write((short)Mobile.Y);
        writer.Write((byte)Mobile.Z);
        writer.Write((byte)Mobile.Direction);
        writer.Write((short)Mobile.Hue);
        writer.Write((byte)Mobile.Status);
        writer.Write((byte)Mobile.Notoriety);

        foreach (var (layer, item) in Mobile.GetItems())
        {
            var modelId = item.ModelId & 0x7FFF;
            var writeHue = item.Hue != 0;
            if (writeHue)
            {
                modelId |= 0x8000;
            }

            writer.Write(item.Serial.Value);
            writer.Write((short)modelId);
            writer.Write((byte)layer);

            if (writeHue)
            {
                writer.Write((short)item.Hue);
            }
        }

        writer.Write((byte)0);


        return writer.ToArray();
    }
}
