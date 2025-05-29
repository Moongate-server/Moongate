using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Close Generic Gump subcommand data (0x04)
/// </summary>
public sealed class CloseGenericGumpData : ISubcommandData
{
    /// <summary>Dialog ID to close</summary>
    public uint DialogId { get; set; }

    /// <summary>Button ID response</summary>
    public uint ButtonId { get; set; }

    /// <inheritdoc />
    public int Length => 8;

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        DialogId = reader.ReadUInt32();
        ButtonId = reader.ReadUInt32();
    }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(DialogId);
        writer.Write(ButtonId);

        return writer.ToArray();
    }
}
