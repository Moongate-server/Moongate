using Moongate.Core.Spans;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.SubCommands.Base.Interfaces;
using Moongate.Uo.Data.Network.Packets.GeneralInformation.Types;

namespace Moongate.Uo.Data.Network.Packets.GeneralInformation.Factory;

/// <summary>
/// Factory for creating General Information packets (0xBF) with various subcommands
/// </summary>
public static class GeneralInformationFactory
{
    #region Fast Walk Prevention

    /// <summary>
    /// Creates Initialize Fast Walk Prevention packet (0x01)
    /// </summary>
    /// <param name="keys">Six 32-bit keys for fast walk prevention</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateInitializeFastWalkPrevention(uint[] keys)
    {
        if (keys.Length != 6)
        {
            throw new ArgumentException("Must provide exactly 6 keys", nameof(keys));
        }

        using var writer = new SpanWriter(24); // 6 * 4 bytes
        foreach (var key in keys)
        {
            writer.Write(key);
        }

        return new GeneralInformationPacket(
            SubcommandType.UseTargetedItem,
            writer.Span.ToArray()
        );
    }

    /// <summary>
    /// Creates Cast Targeted Spell packet (0x2D)
    /// </summary>
    /// <param name="spellId">Spell ID to cast</param>
    /// <param name="targetSerial">Target serial</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateCastTargetedSpell(ushort spellId, uint targetSerial)
    {
        var data = new CastTargetedSpellData
        {
            SpellId = spellId,
            TargetSerial = targetSerial
        };

        using var writer = new SpanWriter(data.Length);
        data.Write(writer);

        return new GeneralInformationPacket(
            SubcommandType.CastTargetedSpell,
            writer.Span.ToArray()
        );
    }

    /// <summary>
    /// Creates Use Targeted Skill packet (0x2E)
    /// </summary>
    /// <param name="skillId">Skill ID (1-55, 0 = last skill)</param>
    /// <param name="targetSerial">Target serial</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateUseTargetedSkill(ushort skillId, uint targetSerial)
    {
        var data = new UseTargetedSkillData
        {
            SkillId = skillId,
            TargetSerial = targetSerial
        };

        using var writer = new SpanWriter(data.Length);
        data.Write(writer);

        return new GeneralInformationPacket(
            SubcommandType.UseTargetedSkill,
            writer.Span.ToArray()
        );
    }

    #endregion

    #region UI Management

    /// <summary>
    /// Creates Close User Interface Windows packet (0x16)
    /// </summary>
    /// <param name="windowId">Window ID to close</param>
    /// <param name="serial">Character or container serial</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateCloseUserInterfaceWindows(uint windowId, uint serial)
    {
        using var writer = new SpanWriter(8);
        writer.Write(windowId);
        writer.Write(serial);

        return new GeneralInformationPacket(
            SubcommandType.CloseUserInterfaceWindows,
            writer.Span.ToArray()
        );
    }

    /// <summary>Creates packet to close paperdoll window</summary>
    public static GeneralInformationPacket CreateClosePaperdoll(uint characterSerial) =>
        CreateCloseUserInterfaceWindows(0x01, characterSerial);

    /// <summary>Creates packet to close status window</summary>
    public static GeneralInformationPacket CreateCloseStatus(uint characterSerial) =>
        CreateCloseUserInterfaceWindows(0x02, characterSerial);

    /// <summary>Creates packet to close character profile window</summary>
    public static GeneralInformationPacket CreateCloseCharacterProfile(uint characterSerial) =>
        CreateCloseUserInterfaceWindows(0x08, characterSerial);

    /// <summary>Creates packet to close container window</summary>
    public static GeneralInformationPacket CreateCloseContainer(uint containerSerial) =>
        CreateCloseUserInterfaceWindows(0x0C, containerSerial);

    #endregion

    #region Popup Menus

    /// <summary>
    /// Creates Request Popup Menu packet (0x13)
    /// </summary>
    /// <param name="characterId">Character ID</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateRequestPopupMenu(uint characterId)
    {
        using var writer = new SpanWriter(4);
        writer.Write(characterId);

        return new GeneralInformationPacket(
            SubcommandType.RequestPopupMenu,
            writer.Span.ToArray()
        );
    }

    /// <summary>
    /// Creates Popup Entry Selection packet (0x15)
    /// </summary>
    /// <param name="characterId">Character ID</param>
    /// <param name="entryTag">Entry tag selected</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreatePopupEntrySelection(uint characterId, ushort entryTag)
    {
        using var writer = new SpanWriter(6);
        writer.Write(characterId);
        writer.Write(entryTag);

        return new GeneralInformationPacket(
            SubcommandType.PopupEntrySelection,
            writer.Span.ToArray()
        );
    }

    #endregion

    #region Extended Features

    /// <summary>
    /// Creates SE Ability Change packet (0x25)
    /// </summary>
    /// <param name="abilityId">Ability ID</param>
    /// <param name="enabled">True to enable, false to disable</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateSEAbilityChange(byte abilityId, bool enabled)
    {
        using var writer = new SpanWriter(2);
        writer.Write(abilityId);
        writer.Write((byte)(enabled ? 1 : 0));

        return new GeneralInformationPacket(
            SubcommandType.SEAbilityChange,
            writer.Span.ToArray()
        );
    }

    /// <summary>
    /// Creates Mount Speed packet (0x26)
    /// </summary>
    /// <param name="speed">Speed: 0=normal, 1=fast, 2=slow, >2=hybrid</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateMountSpeed(byte speed)
    {
        using var writer = new SpanWriter(1);
        writer.Write(speed);

        return new GeneralInformationPacket(
            SubcommandType.MountSpeed,
            writer.Span.ToArray()
        );
    }

    /// <summary>
    /// Creates Toggle Gargoyle Flying packet (0x32)
    /// </summary>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateToggleGargoyleFlying()
    {
        using var writer = new SpanWriter(6);
        writer.Write((uint)0x0100);   // unknown1
        writer.Write((ushort)0x0000); // unknown2

        return new GeneralInformationPacket(
            SubcommandType.ToggleGargoyleFlying,
            writer.Span.ToArray()
        );
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Creates a raw General Information packet with custom data
    /// </summary>
    /// <param name="subcommand">Subcommand type</param>
    /// <param name="data">Raw subcommand data</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateRaw(SubcommandType subcommand, ReadOnlyMemory<byte> data)
    {
        return new GeneralInformationPacket(subcommand, data);
    }

    /// <summary>
    /// Creates a General Information packet from typed subcommand data
    /// </summary>
    /// <typeparam name="T">Subcommand data type</typeparam>
    /// <param name="subcommand">Subcommand type</param>
    /// <param name="data">Typed subcommand data</param>
    /// <returns>GeneralInformationPacket instance</returns>
    public static GeneralInformationPacket CreateFromData<T>(SubcommandType subcommand, T data)
        where T : ISubcommandData
    {
        using var writer = new SpanWriter(data.Length);
        data.Write(writer);

        return new GeneralInformationPacket(subcommand, writer.Span.ToArray());
    }

    #endregion
}
