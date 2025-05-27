using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets.Connection;

public class GameServerLoginPacket : IUoNetworkPacket
{
    public byte OpCode => 0x91;
    public int Length => 65;

    public int AuthId { get; set; }

    public string Sid { get; set; }

    public string Password { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();

        AuthId = reader.ReadInt32();
        Sid = reader.ReadAscii(30);
        Password = reader.ReadAscii(30);

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }

    public override string ToString()
    {
        return $"GameServerLoginPacket: AuthId: {AuthId}, Sid: {Sid}, Password: <HIDDEN CREDENTIAL>";
    }
}
