using Moongate.Core.Interfaces.DataLoader;
using Moongate.Uo.Data.Races;
using Moongate.Uo.Data.Races.Base;

namespace Moongate.Server.DataLoaders;

public class RaceLoader : IDataFileLoader
{
    public async Task<bool> LoadAsync()
    {
        /* Here we configure all races. Some notes:
         *
         * 1) The first 32 races are reserved for core use.
         * 2) Race 0x7F is reserved for core use.
         * 3) Race 0xFF is reserved for core use.
         * 4) Changing or removing any predefined races may cause server instability.
         */

        RaceDefinitions.RegisterRace(new Human(0, 0));
        RaceDefinitions.RegisterRace(new Elf(1, 1));
        RaceDefinitions.RegisterRace(new Gargoyle(2, 2));
        return true;
    }
}
