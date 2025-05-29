using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;

/// <summary>
/// Enable Map Diff subcommand data (0x18)
/// </summary>
public sealed class EnableMapDiffData : ISubcommandData
{
    /// <summary>Number of maps</summary>
    public uint NumberOfMaps { get; set; }

    /// <summary>Map patch counts for each map</summary>
    public uint[] MapPatches { get; set; } = [];

    /// <summary>Static patch counts for each map</summary>
    public uint[] StaticPatches { get; set; } = [];

    /// <inheritdoc />
    public int Length => 4 + ((int)NumberOfMaps * 8);

    /// <inheritdoc />
    public void Read(SpanReader reader)
    {
        NumberOfMaps = reader.ReadUInt32();
        MapPatches = new uint[NumberOfMaps];
        StaticPatches = new uint[NumberOfMaps];

        for (int i = 0; i < NumberOfMaps; i++)
        {
            MapPatches[i] = reader.ReadUInt32();
            StaticPatches[i] = reader.ReadUInt32();
        }
    }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write(NumberOfMaps);
        for (int i = 0; i < NumberOfMaps; i++)
        {
            writer.Write(MapPatches[i]);
            writer.Write(StaticPatches[i]);
        }

        return writer.ToArray();
    }
}
