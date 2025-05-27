using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Damage subcommand data (0x22)
/// </summary>
public sealed class DamageData : ISubcommandData
{
    /// <summary>Unknown field, always 1</summary>
    public ushort Unknown { get; set; } = 1;

    /// <summary>Target serial</summary>
    public uint Serial { get; set; }

    /// <summary>Damage amount</summary>
    public byte Damage { get; set; }

    /// <inheritdoc />
    public int Length => 7;

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        Unknown = reader.ReadUInt16();
        Serial = reader.ReadUInt32();
        Damage = reader.ReadByte();
    }

    /// <inheritdoc />
    public void Write(SpanWriter writer)
    {
        writer.Write(Unknown);
        writer.Write(Serial);
        writer.Write(Damage);
    }
}
