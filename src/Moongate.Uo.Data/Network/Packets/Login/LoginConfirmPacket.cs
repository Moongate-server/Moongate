using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Login;

public class LoginConfirmPacket : IUoNetworkPacket
{
    public byte OpCode => 0x1B;
    public int Length => 37;

    public Serial CharacterSerial { get; set; } = Serial.Zero;

    public short ModelId { get; set; }

    public Point3D Location { get; set; } = Point3D.Zero;

    public Direction Direction { get; set; } = Direction.North;


    public Map Map { get; set; } = Map.Felucca;

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(CharacterSerial.Value);
        writer.Write(0);
        writer.Write(ModelId);
        writer.Write(Location.X);
        writer.Write(Location.Y);
        writer.Write((byte)0);
        writer.Write(Location.Z);
        writer.Write((byte)Direction);
        writer.Write(0);
        writer.Write(0);
        writer.Write((byte)0);
        writer.Write((ushort)Map.Width - 8);
        writer.Write((ushort)Map.Height);
        writer.Write((short)0);
        writer.Write((byte)0);

        return writer.ToArray();
    }
}
