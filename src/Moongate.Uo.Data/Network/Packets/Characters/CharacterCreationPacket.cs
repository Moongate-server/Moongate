using Moongate.Core.Spans;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Races.Base;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Characters;

public class CharacterCreationPacket : IUoNetworkPacket
{
    public byte OpCode => 0xF8;
    public int Length => 106;

    public string Name { get; set; }

    public int Slot { get; set; }

    public int LoginCount { get; set; }
    public ClientFlags ClientFlags { get; set; }

    public Dictionary<SkillName, int> Skills { get; set; } = new();

    public ProfessionInfo Profession { get; set; } = new();

    public SexType Sex { get; set; }

    public int Str { get; set; }

    public int Dex { get; set; }

    public int Int { get; set; }

    public bool IsFemale { get; set; }

    public int Hue { get; set; }

    public short HairStyle { get; set; }

    public short HairColor { get; set; }

    public short FacialHair { get; set; }

    public short FacialHairColor { get; set; }

    public short StartingLocation { get; set; }

    public Race Race { get; set; }

    public short ShirtColor { get; set; }

    public short PantsColor { get; set; }

    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        reader.ReadInt32(); // (0xedededed)
        reader.ReadInt32(); // (0xffffffff)
        reader.ReadByte();  //(0xffffffff)

        Name = reader.ReadAscii(30);

        reader.ReadByte();
        reader.ReadByte();

        ClientFlags = (ClientFlags)reader.ReadInt32();

        reader.ReadInt32();

        LoginCount = reader.ReadInt32();
        Profession = ProfessionInfo.Professions[reader.ReadByte()];

        reader.ReadBytes(15);

        var genderRace = reader.ReadByte();
        Sex = (SexType)genderRace;

        IsFemale = genderRace % 2 != 0;



        var raceID = UoContext.StygianAbyss ? (byte)(genderRace < 4 ? 0 : genderRace / 2 - 1) : (byte)(genderRace / 2);
        Race = Race.Races[raceID] ?? Race.DefaultRace;



        Str = reader.ReadByte();
        Dex = reader.ReadByte();
        Int = reader.ReadByte();

        for (var i = 0; i < 4; i++)
        {
            var skillName = (SkillName)reader.ReadByte();
            var skillValue = reader.ReadByte();

            Skills.Add(skillName, skillValue);
        }


        Hue = reader.ReadUInt16();

        HairStyle = reader.ReadInt16();
        HairColor = reader.ReadInt16();

        FacialHair = reader.ReadInt16();
        FacialHairColor = reader.ReadInt16();

        StartingLocation = reader.ReadInt16();

        reader.ReadInt16(); // (0x00000000)

        Slot = reader.ReadInt16();

        reader.ReadInt32();

        ShirtColor = reader.ReadInt16();

        PantsColor = reader.ReadInt16();

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        return ReadOnlyMemory<byte>.Empty;
    }
}
