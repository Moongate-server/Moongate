using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation;

public class GeneralInformationPacket : IUoNetworkPacket
{
    public byte OpCode => 0xBF;
    public int Length { get; private set; }

    /// <summary>
    /// Gets the subcommand type
    /// </summary>
    public SubcommandType Subcommand { get; private set; }

    /// <summary>
    /// Gets the raw subcommand data
    /// </summary>
    public ReadOnlyMemory<byte> SubcommandData { get; private set; }


    /// <summary>
    /// Initializes a new GeneralInformationPacket
    /// </summary>
    public GeneralInformationPacket()
    {
    }

    /// <summary>
    /// Initializes a new GeneralInformationPacket with subcommand data
    /// </summary>
    /// <param name="subcommand">Subcommand type</param>
    /// <param name="data">Subcommand data</param>
    public GeneralInformationPacket(SubcommandType subcommand, ReadOnlyMemory<byte> data)
    {
        Subcommand = subcommand;
        SubcommandData = data;
        Length = 5 + data.Length; // 1 + 2 + 2 + data length
    }

    public GeneralInformationPacket(SubcommandType subcommand, ISubcommandData data)
    {
        Subcommand = subcommand;
        SubcommandData = ReadOnlyMemory<byte>.Empty;

        using var writer = new SpanWriter(1, true);
        SubcommandData = data.Write(writer);
        Length = 5 + data.Length;

    }

    /// <summary>
    /// Creates a typed subcommand parser for this packet
    /// </summary>
    /// <returns>Subcommand parser instance</returns>
    public ISubcommandParser CreateParser()
    {
        return new SubcommandParser(Subcommand, SubcommandData);
    }

    /// <inheritdoc />
    public bool Read(SpanReader reader)
    {
        try
        {
            // Read packet length
            Length = reader.ReadUInt16();
            if (Length < 5)
            {
                return false;
            }

            // Read subcommand type
            Subcommand = (SubcommandType)reader.ReadUInt16();

            // Read remaining data
            var dataLength = Length - 5; // Total length - header (1 + 2 + 2)
            if (dataLength > 0)
            {
                var data = new byte[dataLength];
                for (int i = 0; i < dataLength; i++)
                {
                    data[i] = reader.ReadByte();
                }
                SubcommandData = data;
            }
            else
            {
                SubcommandData = ReadOnlyMemory<byte>.Empty;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        var startPosition = writer.Position;

        writer.Write(OpCode);
        writer.Write((ushort)Length);
        writer.Write((ushort)Subcommand);

        if (!SubcommandData.IsEmpty)
        {
            writer.Write(SubcommandData.Span);
        }

        var endPosition = writer.Position;
        return writer.RawBuffer.Slice(startPosition, endPosition - startPosition).ToArray();
    }
}
