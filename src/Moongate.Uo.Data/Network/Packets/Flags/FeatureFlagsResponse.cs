using Moongate.Core.Spans;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Flags;

/// <summary>
/// Represents a features packet sent from the server to the client
/// to enable or disable client features based on server configuration.
/// </summary>
/// <remarks>
/// OpCode: 0xB9
/// This packet is sent immediately after login to configure the client's feature set.
/// </remarks>
public class FeatureFlagsResponse : IUoNetworkPacket
{
    public byte OpCode => 0xB9;
    public int Length => 5;

    /// <summary>
    /// Gets or sets the feature flags as a 32-bit unsigned integer.
    /// Bits represent different features that can be enabled or disabled.
    /// </summary>
    /// <remarks>
    /// Feature flags:
    /// 0x00000001: Enable T2A features (chat, regions)
    /// 0x00000002: Enable Renaissance features
    /// 0x00000004: Enable Third Dawn features
    /// 0x00000008: Enable LBR features (skills, map)
    /// 0x00000010: Enable AOS features (skills, map, spells, fightbook)
    /// 0x00000020: Enable 6th character slot
    /// 0x00000040: Enable SE features
    /// 0x00000080: Enable ML features (elven race, spells, skills)
    /// 0x00000100: Enable 8th age splash screen
    /// 0x00000200: Enable 9th age splash screen
    /// 0x00000400: Enable 10th age features
    /// 0x00000800: Enable increased housing and bank storage
    /// 0x00001000: Enable 7th character slot
    /// 0x00002000: Enable KR faces
    /// 0x00004000: Enable trial account
    /// 0x00008000: Enable live account
    /// 0x00010000: Enable SA features (gargoyle race, spells, skills)
    /// 0x00020000: Enable HSA features
    /// 0x00040000: Enable Gothic housing tiles
    /// 0x00080000: Enable Rustic housing tiles
    /// 0x00100000: Enable Jungle housing tiles
    /// 0x00200000: Enable Shadowguard housing tiles
    /// 0x00400000: Enable TOL features
    /// 0x00800000: Enable Endless Journey account
    /// </remarks>
    public FeatureFlags Flags { get; set; }


    /// <summary>
    /// Creates a new instance of the FeatureFlags class with the specified flags.
    /// </summary>
    /// <param name="flags">The feature flags to set.</param>
    public FeatureFlagsResponse(FeatureFlags flags) : this()
    {
        Flags = flags;
    }

    public FeatureFlagsResponse()
    {

    }


    public bool Read(SpanReader reader)
    {
        reader.ReadByte();
        Flags = (FeatureFlags)reader.ReadUInt32();

        return true;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        writer.Write((byte)OpCode);
        writer.Write((uint)Flags);

        return writer.ToArray();
    }
}
