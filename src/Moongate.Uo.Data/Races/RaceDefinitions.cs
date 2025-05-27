using Moongate.Uo.Data.Races.Base;

namespace Moongate.Uo.Data.Races;

public class RaceDefinitions
{
    public static void RegisterRace(Race race)
    {
        Race.Races[race.RaceIndex] = race;
        Race.AllRaces.Add(race);
    }
}
