using Moongate.Uo.Data.Tiles;
using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data;

public class Map : IComparable<Map>, IComparable
{
    public const int SectorSize = 16;
    public const int SectorShift = 4;
    public static int SectorActiveRange = 2;

    private static readonly Map[] _maps = new Map[0x100];
    private static readonly List<Map> _allMaps = new List<Map>();

    public static Map[] Maps => _maps;
    public static Map Felucca => _maps[0];
    public static Map Trammel => _maps[1];
    public static Map Ilshenar => _maps[2];
    public static Map Malas => _maps[3];
    public static Map Tokuno => _maps[4];
    public static Map TerMur => _maps[5];
    public static Map Internal => _maps[0x7F];

    public int Index { get; }
    public int MapID { get; }
    public int FileIndex { get; }
    public int Width { get; }
    public int Height { get; }
    public Season Season { get; }
    public string Name { get; }
    public MapRules Rules { get; }

    private readonly TileMatrix _tiles;


    private Map(int index, int mapId, int fileIndex, int width, int height, Season season, string name, MapRules rules)
    {
        Index = index;
        MapID = mapId;
        FileIndex = fileIndex;
        Width = width;
        Height = height;
        Season = season;
        Name = name;
        Rules = rules;


        _tiles = new TileMatrix(fileIndex, mapId, width, height);
    }

    public static Map RegisterMap(
        int index, int mapID, int fileIndex, int width, int height, Season season, string name, MapRules rules
    )
    {
        Map m = new Map(index, mapID, fileIndex, width, height, season, name, rules);
        _maps[index] = m;
        _allMaps.Add(m);
        return m;
    }


    public static Map GetMap(int index)
    {
        if (index < 0 || index >= _maps.Length)
        {
            return null;
        }

        return _maps[index];
    }


    public int CompareTo(Map other) => other == null ? 1 : string.Compare(Name, other.Name, StringComparison.Ordinal);
    public int CompareTo(object obj) => (obj is Map map) ? CompareTo(map) : 1;


    public LandTile GetLandTile(int x, int y)
    {
        return _tiles.GetLandTile(x, y);
    }

    // public StaticTile[] GetStaticTiles(int x, int y)
    // {
    //
    //     return _tiles.GetStaticTiles(x, y);
    // }
}
