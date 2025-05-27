using Moongate.Uo.Data.Network.Packets.GeneralInformation.Types;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;

/// <summary>
/// Interface for parsing subcommand data
/// </summary>
public interface ISubcommandParser
{
    /// <summary>Gets the subcommand type</summary>
    SubcommandType Type { get; }

    /// <summary>Parses the subcommand data as the specified type</summary>
    T Parse<T>() where T : class, new();

    /// <summary>Gets raw subcommand data</summary>
    ReadOnlySpan<byte> GetRawData();
}
