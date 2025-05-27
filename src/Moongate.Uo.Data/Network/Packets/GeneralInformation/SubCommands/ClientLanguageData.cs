using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Client Language subcommand data (0x0B)
/// </summary>
public sealed class ClientLanguageData : ISubcommandData
{
    /// <summary>Language code (e.g., "ENU" for English)</summary>
    public string Language { get; set; } = "ENU";

    /// <inheritdoc />
    public int Length => 4; // 3 bytes + 1 null terminator

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        Language = reader.ReadAscii(3);
        reader.ReadByte(); // null terminator
    }

    /// <inheritdoc />
    public void Write(SpanWriter writer)
    {
        writer.WriteAscii(Language, 3);
        writer.Write((byte)0); // null terminator
    }
}
