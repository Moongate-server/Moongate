using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Set Cursor Hue/Set Map subcommand data (0x08)
/// </summary>
public sealed class SetCursorHueSetMapData : ISubcommandData
{
    /// <summary>
    /// Map ID: 0=Felucca, 1=Trammel, 2=Ilshenar, 3=Malas, 4=Tokuno, 5=TerMur
    /// </summary>
    public byte MapId { get; set; }

    /// <inheritdoc />
    public int Length => 1;

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        MapId = reader.ReadByte();
    }

    /// <inheritdoc />
    public void Write(SpanWriter writer)
    {
        writer.Write(MapId);
    }
}
