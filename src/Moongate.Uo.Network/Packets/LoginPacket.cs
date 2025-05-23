using Moongate.Core.Spans;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Network.Packets;

public class LoginPacket : IUoNetworkPacket
{
    public int Seed { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public byte OpCode => 0x80;
    public int Length => 62;

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        Username = reader.ReadAscii(30);
        Password = reader.ReadAscii(30);

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }

    public override string ToString()
    {
        return $"LoginPacket: Username: {Username}, Password: <HIDDEN CREDENTIAL>";
    }
}
