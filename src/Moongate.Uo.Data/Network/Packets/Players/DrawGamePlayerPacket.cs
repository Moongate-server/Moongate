using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Players;

public class DrawGamePlayerPacket : IUoNetworkPacket
{
    public byte OpCode => 0x20;
    public int Length => 19;


    public MobileEntity Mobile { get; set; }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public DrawGamePlayerPacket(MobileEntity? mobileEntity = null)
    {
        if (mobileEntity != null)
        {
            Mobile = mobileEntity;
        }
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);

        writer.Write((short)Mobile.Body);
        writer.Write((byte)0);
        writer.Write((short)(Mobile.SolidHueOverride >= 0 ? Mobile.SolidHueOverride : Mobile.Hue));
        // See Mobile.cs in modern UO
        writer.Write((byte)Mobile.Status);

        writer.Write((short)Mobile.X);
        writer.Write((short)Mobile.Y);
        writer.Write((short)0);
        writer.Write((byte)Mobile.Direction);
        writer.Write((sbyte)Mobile.Z);

        return writer.ToArray();
    }
}
