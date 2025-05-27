using System.Text.Json.Serialization;
using Moongate.Core.Extensions.Strings;
using Moongate.Core.Json.Converters;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data;

public class ExpansionInfo
{
    public static bool ForceOldAnimations { get; private set; }
    
    public static string GetEraFolder(string parentDirectory)
    {
        var expansion = UoContext.Expansion;
        var folders = Directory.GetDirectories(
            parentDirectory,
            "*",
            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }
        );

        while (expansion-- >= 0)
        {
            foreach (var folder in folders)
            {
                var di = new DirectoryInfo(folder);
                if (di.Name.InsensitiveEquals(expansion.ToString()))
                {
                    return folder;
                }
            }
        }

        return null;
    }

    public static void StoreMapSelection(MapSelectionFlags mapSelectionFlags, Expansion expansion)
    {
        int expansionIndex = (int)expansion;
        Table[expansionIndex].MapSelectionFlags = mapSelectionFlags;
    }


    public ExpansionInfo(
        int id,
        string name,
        ClientFlags clientFlags,
        FeatureFlags supportedFeatures,
        CharacterListFlags charListFlags,
        HousingFlags customHousingFlag,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    ) : this(id, name, supportedFeatures, charListFlags, customHousingFlag, mobileStatusVersion, mapSelectionFlags) =>
        ClientFlags = clientFlags;

    public ExpansionInfo(
        int id,
        string name,
        ClientVersion requiredClient,
        FeatureFlags supportedFeatures,
        CharacterListFlags charListFlags,
        HousingFlags customHousingFlag,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    ) : this(id, name, supportedFeatures, charListFlags, customHousingFlag, mobileStatusVersion, mapSelectionFlags) =>
        RequiredClient = requiredClient;

    [JsonConstructor]
    public ExpansionInfo(
        int id,
        string name,
        FeatureFlags supportedFeatures,
        CharacterListFlags characterListFlags,
        HousingFlags housingFlags,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    )
    {
        Id = id;
        Name = name;

        SupportedFeatures = supportedFeatures;
        CharacterListFlags = characterListFlags;
        HousingFlags = housingFlags;
        MobileStatusVersion = mobileStatusVersion;
        MapSelectionFlags = mapSelectionFlags;
    }

    public static ExpansionInfo CoreExpansion => GetInfo(UoContext.Expansion);

    public static ExpansionInfo[] Table { get; set; }

    public int Id { get; }
    public string Name { get; set; }

    public ClientFlags ClientFlags { get; set; }

    [JsonConverter(typeof(FlagsConverter<FeatureFlags>))]
    public FeatureFlags SupportedFeatures { get; set; }

    [JsonConverter(typeof(FlagsConverter<CharacterListFlags>))]
    public CharacterListFlags CharacterListFlags { get; set; }

    public ClientVersion RequiredClient { get; set; }

    [JsonConverter(typeof(FlagsConverter<HousingFlags>))]
    public HousingFlags HousingFlags { get; set; }

    public int MobileStatusVersion { get; set; }

    [JsonConverter(typeof(FlagsConverter<MapSelectionFlags>))]
    public MapSelectionFlags MapSelectionFlags { get; set; }

    public static ExpansionInfo GetInfo(Expansion ex) => GetInfo((int)ex);

    public static ExpansionInfo GetInfo(int ex)
    {
        var v = ex;

        if (v < 0 || v >= Table.Length)
        {
            v = 0;
        }

        return Table[v];
    }

    public override string ToString() => Name;
}
