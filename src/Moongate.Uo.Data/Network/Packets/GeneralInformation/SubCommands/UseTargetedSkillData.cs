using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Use Targeted Skill subcommand data (0x2E)
/// </summary>
public sealed class UseTargetedSkillData : ISubcommandData
{
    /// <summary>Skill ID (1-55, 0 = last skill)</summary>
    public ushort SkillId { get; set; }

    /// <summary>Target serial</summary>
    public uint TargetSerial { get; set; }

    /// <inheritdoc />
    public int Length => 6;

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        SkillId = reader.ReadUInt16();
        TargetSerial = reader.ReadUInt32();
    }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(SkillId);
        writer.Write(TargetSerial);

        return writer.ToArray();
    }
}
