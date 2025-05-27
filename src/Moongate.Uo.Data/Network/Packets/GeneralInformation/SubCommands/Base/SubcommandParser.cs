using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.Types;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base;

/// <summary>
/// Parser for General Information subcommands
/// </summary>
public sealed class SubcommandParser : ISubcommandParser
{
    private readonly ReadOnlyMemory<byte> _data;

    /// <inheritdoc />
    public SubcommandType Type { get; }

    /// <summary>
    /// Initializes a new SubcommandParser
    /// </summary>
    /// <param name="type">Subcommand type</param>
    /// <param name="data">Subcommand data</param>
    public SubcommandParser(SubcommandType type, ReadOnlyMemory<byte> data)
    {
        Type = type;
        _data = data;
    }

    /// <inheritdoc />
    public T Parse<T>() where T : class, new()
    {
        var instance = new T();

        if (instance is ISubcommandData subcommandData)
        {
            var reader = new SpanReader(_data.Span);
            subcommandData.Read(reader);
        }

        return instance;
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> GetRawData() => _data.Span;
}
