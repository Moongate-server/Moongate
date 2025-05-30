using Moongate.Core.Data.Ids;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Login;

public class LoginConfirmPacket : IUoNetworkPacket
{
    public byte OpCode => 0x1B;
    public int Length => 37;

    public Serial CharacterSerial { get; set; } = Serial.Zero;

    public Body Body { get; set; }

    public Point3D Location { get; set; } = Point3D.Zero;

    public Direction Direction { get; set; } = Direction.North;

    public Map Map { get; set; } = Map.Felucca;

    public LoginConfirmPacket(MobileEntity? mobileEntity = null)
    {
        if (mobileEntity != null)
        {
            CharacterSerial = mobileEntity.Serial;
            Body = mobileEntity.Body;
            Location = mobileEntity.Location;
            Direction = mobileEntity.Direction;
            Map = mobileEntity.Map;
        }
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    /*
     *
     * Packet Build
       BYTE[1] cmd
       BYTE[4] player serial
       BYTE[4] unknown. always 0
       BYTE[2] body type
       BYTE[2] xLoc
       BYTE[2] yLoc
       BYTE[1] Unknown 0
       BYTE[1] zLoc
       BYTE[1] facing
       BYTE[4] unknown 0x0
       BYTE[4] unknown 0x0
       BYTE[1] unknown 0x0
       BYTE[2] server boundary width minus eight.
       BYTE[2] server boundary Height
       BYTE[2] unknown 0x0
       BYTE[4] unknown 0x0


     */

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(OpCode);
        writer.Write(CharacterSerial.Value);
        writer.Write(0);
        writer.Write((short)Body);
        writer.Write((short)Location.X);
        writer.Write((short)Location.Y);
        writer.Write((byte)0);
        writer.Write((byte)Location.Z);
        writer.Write((byte)Direction);
        writer.Write(0); // Unknown
        writer.Write(0); // Unknown
        writer.Write((byte)0); // Unknown

        writer.Write((short)(Map?.Width ?? Map.Felucca.Width));
        writer.Write((short)(Map?.Height ?? Map.Felucca.Height));
        writer.Write((short)0); // Unknown
        writer.Write(0); // Unknown
        //writer.Clear(writer.Capacity - writer.Position); // Remaining is zero

        return writer.ToArray();
    }
}
