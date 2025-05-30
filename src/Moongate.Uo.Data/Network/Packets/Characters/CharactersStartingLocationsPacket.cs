using Moongate.Core.Spans;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Network.Packets.Data;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class CharactersStartingLocationsPacket : IUoNetworkPacket
{
    public byte OpCode => 0xA9;
    public int Length => -1;

    public ProtocolChanges ProtocolChanges { get; set; }

    public List<CityInfo> Cities { get; } = new();

    public List<CharacterEntry> Characters { get; } = new();


    public CharactersStartingLocationsPacket(ProtocolChanges protocolChanges = ProtocolChanges.None)
    {
        ProtocolChanges = protocolChanges;
    }

    public void FillCharacters(List<CharacterEntry>? characters = null, int size = 7)
    {
        Characters.Clear();

        if (characters != null)
        {
            Characters.AddRange(characters);
        }
        else
        {
            for (var i = 0; i < size; i++)
            {
                Characters.Add(null);
            }
        }
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        var client70130 = ProtocolChanges.HasFlag(ProtocolChanges.NewCharacterList);
        var textLength = client70130 ? 32 : 31;

        var cityInfo = Cities;


        var highSlot = -1;

        for (var i = Characters.Count - 1; i >= 0; i--)
        {
            if (Characters[i] != null)
            {
                highSlot = i;
                break;
            }
        }

        // Supported values are 1, 5, 6, or 7
        var count = Math.Max(highSlot + 1, 7);
        if (count is not 1 and < 5)
        {
            count = 5;
        }

        var length =
            (client70130 ? 11 + (textLength * 2 + 25) * cityInfo.Count : 9 + (textLength * 2 + 1) * cityInfo.Count) +
            count * 60;


        writer.Write(OpCode);
        writer.Write((ushort)length);
        writer.Write((byte)count); // TODO: It is probably more proper to use count.

        foreach (var character in Characters)
        {
            if (character == null)
            {
                writer.Clear(60);
                continue;
            }

            writer.WriteAscii(character.Name, 30);
            writer.WriteAscii(character.Password, 30);
        }

        writer.Write((byte)cityInfo.Count);

        for (int i = 0; i < cityInfo.Count; ++i)
        {
            var ci = cityInfo[i];

            writer.Write((byte)i);
            writer.WriteAscii(ci.City, textLength);
            writer.WriteAscii(ci.Building, textLength);
            if (client70130)
            {
                writer.Write(ci.X);
                writer.Write(ci.Y);
                writer.Write(ci.Z);
                writer.Write(ci.Map.Index);
                writer.Write(ci.Description);
                writer.Write(0);
            }
        }

        var flags = ExpansionInfo.CoreExpansion.CharacterListFlags;

        if (count > 6)
        {
            flags |= CharacterListFlags.SeventhCharacterSlot |
                     CharacterListFlags.SixthCharacterSlot; // 7th Character Slot - TODO: Is SixthCharacterSlot Required?
        }
        else if (count == 6)
        {
            flags |= CharacterListFlags.SixthCharacterSlot; // 6th Character Slot
        }
        else if (UoContext.SlotLimit == 1)
        {
            flags |= CharacterListFlags.SlotLimit &
                     CharacterListFlags.OneCharacterSlot; // Limit Characters & One Character
        }

        writer.Write((int)flags);
        if (client70130)
        {
            writer.Write((short)-1);
        }

        // 169 4 208


        return writer.ToSpan().Span.ToArray();
    }
}
