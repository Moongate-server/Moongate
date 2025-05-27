using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Players;

public class DrawGamePlayerPacket : IUoNetworkPacket
{
    public byte OpCode => 0x20;
    public int Length => 19;

    public Serial MobileId { get; set; } = Serial.Zero;

    public int BodyType { get; set; }

    public int Hue { get; set; }

    public Point3D Position { get; set; } = Point3D.Zero;

    public Direction Direction { get; set; } = Direction.North;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(MobileId.Value);
        writer.Write((short)BodyType);
        writer.Write((byte)0);
        writer.Write((short)Hue);
        writer.Write((byte)0);
        writer.Write((short)Position.X);
        writer.Write((short)Position.Y);
        writer.Write((byte)0);
        writer.Write((byte)Direction);
        writer.Write((byte)Position.Z);
        return writer.ToArray();
    }
}
