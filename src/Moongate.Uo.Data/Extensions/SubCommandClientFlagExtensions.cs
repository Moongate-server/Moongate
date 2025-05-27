using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data.Extensions;

/// <summary>
/// Extension methods for SubcommandClientFlag enum
/// </summary>
public static class SubCommandSubcommandClientFlagExtensions
{
    /// <summary>
    /// Parses client flags from an integer value
    /// </summary>
    /// <param name="value">Integer value containing flags</param>
    /// <returns>Parsed SubcommandClientFlag enum</returns>
    public static SubcommandClientFlag Parse(int value) => (SubcommandClientFlag)(uint)value;

    /// <summary>
    /// Checks if the client supports Third Dawn (3D client)
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if Third Dawn is supported</returns>
    public static bool SupportsThirdDawn(this SubcommandClientFlag flags) => flags.HasFlag(SubcommandClientFlag.ThirdDawn);

    /// <summary>
    /// Checks if the client supports post-AOS features
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if post-AOS features are supported</returns>
    public static bool SupportsPostAOS(this SubcommandClientFlag flags) => flags.HasFlag(SubcommandClientFlag.PostAOS);

    /// <summary>
    /// Checks if the client is Kingdom Reborn
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if client is Kingdom Reborn</returns>
    public static bool IsKingdomReborn(this SubcommandClientFlag flags) => flags.HasFlag(SubcommandClientFlag.KingdomReborn);

    /// <summary>
    /// Checks if the client is Enhanced Client
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if client is Enhanced Client</returns>
    public static bool IsEnhancedClient(this SubcommandClientFlag flags) =>
        flags.HasFlag(SubcommandClientFlag.EnhancedClient);

    /// <summary>
    /// Checks if the client supports Stygian Abyss features
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if Stygian Abyss is supported</returns>
    public static bool SupportsStygianAbyss(this SubcommandClientFlag flags) =>
        flags.HasFlag(SubcommandClientFlag.StygianAbyss);

    /// <summary>
    /// Checks if the client supports High Seas features
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if High Seas is supported</returns>
    public static bool SupportsHighSeas(this SubcommandClientFlag flags) => flags.HasFlag(SubcommandClientFlag.HighSeas);

    /// <summary>
    /// Checks if the client is 64-bit
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if client is 64-bit</returns>
    public static bool Is64Bit(this SubcommandClientFlag flags) => flags.HasFlag(SubcommandClientFlag.Client64Bit);

    /// <summary>
    /// Checks if the client supports custom housing
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>True if custom housing is supported</returns>
    public static bool SupportsCustomHousing(this SubcommandClientFlag flags) =>
        flags.HasFlag(SubcommandClientFlag.CustomHousing);

    /// <summary>
    /// Determines the client type based on flags
    /// </summary>
    /// <param name="flags">Client flags to analyze</param>
    /// <returns>Client type description</returns>
    public static string GetClientType(this SubcommandClientFlag flags)
    {
        if (flags.HasFlag(SubcommandClientFlag.EnhancedClient))
            return "Enhanced Client";
        if (flags.HasFlag(SubcommandClientFlag.KingdomReborn))
            return "Kingdom Reborn";
        if (flags.HasFlag(SubcommandClientFlag.ThirdDawn))
            return "Third Dawn";

        return "Classic Client";
    }

    /// <summary>
    /// Gets the latest expansion supported by the client
    /// </summary>
    /// <param name="flags">Client flags to check</param>
    /// <returns>Latest expansion name</returns>
    public static string GetLatestExpansion(this SubcommandClientFlag flags)
    {
        if (flags.HasFlag(SubcommandClientFlag.EndlessJourney))
            return "Endless Journey";
        if (flags.HasFlag(SubcommandClientFlag.TOL))
            return "Time of Legends";
        if (flags.HasFlag(SubcommandClientFlag.HighSeas))
            return "High Seas";
        if (flags.HasFlag(SubcommandClientFlag.StygianAbyss))
            return "Stygian Abyss";
        if (flags.HasFlag(SubcommandClientFlag.MondainsLegacy))
            return "Mondain's Legacy";
        if (flags.HasFlag(SubcommandClientFlag.SamuraiEmpire))
            return "Samurai Empire";
        if (flags.HasFlag(SubcommandClientFlag.PostAOS))
            return "Age of Shadows";

        return "Pre-AOS";
    }

    /// <summary>
    /// Gets a detailed description of all client capabilities
    /// </summary>
    /// <param name="flags">Client flags to describe</param>
    /// <returns>Detailed description string</returns>
    public static string GetDetailedDescription(this SubcommandClientFlag flags)
    {
        if (flags == SubcommandClientFlag.None)
            return "Classic Client (No special features)";

        var features = new List<string>();

        // Client type
        features.Add($"Type: {flags.GetClientType()}");

        // Latest expansion
        features.Add($"Expansion: {flags.GetLatestExpansion()}");

        // Special features
        var specialFeatures = new List<string>();

        if (flags.HasFlag(SubcommandClientFlag.Client64Bit))
            specialFeatures.Add("64-bit");
        if (flags.HasFlag(SubcommandClientFlag.NewMovementSystem))
            specialFeatures.Add("New Movement");
        if (flags.HasFlag(SubcommandClientFlag.CustomHousing))
            specialFeatures.Add("Custom Housing");
        if (flags.HasFlag(SubcommandClientFlag.VeteranRewards))
            specialFeatures.Add("Veteran Rewards");

        // Theme support
        var themes = new List<string>();
        if (flags.HasFlag(SubcommandClientFlag.Gothic))
            themes.Add("Gothic");
        if (flags.HasFlag(SubcommandClientFlag.Rustic))
            themes.Add("Rustic");
        if (flags.HasFlag(SubcommandClientFlag.Jungle))
            themes.Add("Jungle");
        if (flags.HasFlag(SubcommandClientFlag.Shadowguard))
            themes.Add("Shadowguard");

        if (specialFeatures.Count > 0)
            features.Add($"Special: {string.Join(", ", specialFeatures)}");

        if (themes.Count > 0)
            features.Add($"Themes: {string.Join(", ", themes)}");

        return string.Join(" | ", features);
    }

    /// <summary>
    /// Gets a compact summary of client capabilities
    /// </summary>
    /// <param name="flags">Client flags to summarize</param>
    /// <returns>Compact summary string</returns>
    public static string GetSummary(this SubcommandClientFlag flags)
    {
        var clientType = flags.GetClientType();
        var expansion = flags.GetLatestExpansion();
        var is64Bit = flags.Is64Bit() ? " (64-bit)" : "";

        return $"{clientType} - {expansion}{is64Bit}";
    }
}
