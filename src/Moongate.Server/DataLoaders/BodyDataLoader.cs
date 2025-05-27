using Moongate.Core.Directories;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Types;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Files;
using Moongate.Uo.Data.Types;
using Serilog;

namespace Moongate.Server.DataLoaders;

public class BodyDataLoader : IDataFileLoader
{
    private readonly ILogger _logger = Log.ForContext<BodyDataLoader>();

    private readonly DirectoriesConfig _directoriesConfig;

    public BodyDataLoader(DirectoriesConfig directoriesConfig)
    {
        _directoriesConfig = directoriesConfig;
    }

    public async Task<bool> LoadAsync()
    {
        var bodyTable =  Path.Combine(_directoriesConfig[DirectoryType.Data], "bodyTable.cfg");

        if (!File.Exists(bodyTable))
        {
            _logger.Warning("Warning: Data/bodyTable.cfg does not exist");
            Body.Types = [];
            return false;
        }

        using StreamReader ip = new StreamReader(bodyTable);
        Body.Types = new BodyType[0x1000];

        while (await ip.ReadLineAsync() is { } line)
        {
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var split = line.Split('\t');

            if (int.TryParse(split[0], out var bodyID) && Enum.TryParse(split[1], true, out BodyType type) && bodyID >= 0 &&
                bodyID < Body.Types.Length)
            {
                Body.Types[bodyID] = type;
            }
            else
            {
                _logger.Warning("Warning: Invalid bodyTable entry:");
                _logger.Warning(line);
            }
        }

        return true;
    }
}
