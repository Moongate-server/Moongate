using Moongate.Core.Directories;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Json;
using Moongate.Core.Types;
using Moongate.Server.Json;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Types;
using Serilog;

namespace Moongate.Server.DataLoaders;

public class ExpansionLoader : IDataFileLoader
{

    private readonly string _expansionConfigurationPath = "expansion.json";
    private readonly string _expansionsPath = "expansions.json";
    private readonly ILogger _logger = Log.ForContext<ExpansionLoader>();
    private readonly DirectoriesConfig _directoriesConfig;

    public ExpansionLoader(DirectoriesConfig directoriesConfig)
    {
        _directoriesConfig = directoriesConfig;
        _expansionConfigurationPath = Path.Combine(_directoriesConfig[DirectoryType.Data], _expansionConfigurationPath);
        _expansionsPath = Path.Combine(_directoriesConfig[DirectoryType.Data], _expansionsPath);
    }

    public async Task<bool> LoadAsync()
    {
        ExpansionInfo.Table = JsonUtils.DeserializeFromFile<ExpansionInfo[]>(_expansionsPath, MoongateJsonContext.Default);
        var expansion = JsonUtils.DeserializeFromFile<ExpansionInfo>(_expansionConfigurationPath, MoongateJsonContext.Default);

        if (expansion == null)
        {
            UoContext.Expansion = Expansion.None;
        }


        var currentExpansionIndex = expansion.Id;
        ExpansionInfo.Table[currentExpansionIndex] = expansion;
        UoContext.Expansion = (Expansion)currentExpansionIndex;
        UoContext.ExpansionInfo = expansion;

        _logger.Information("Loaded expansion: {ExpansionName} (ID: {ExpansionId})", expansion.Name, currentExpansionIndex);

        return true;

        //JsonUtils.DeserializeFromFile<MoongateServerConfig>(
        //     configPath,
        //     MoongateJsonContext.Default
        // );
    }
}
