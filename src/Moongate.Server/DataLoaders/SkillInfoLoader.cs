using Moongate.Core.Directories;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Json;
using Moongate.Core.Types;
using Moongate.Server.Json;
using Moongate.Uo.Data;
using Serilog;

namespace Moongate.Server.DataLoaders;

public class SkillInfoLoader : IDataFileLoader
{
    private readonly DirectoriesConfig _directoriesConfig;
    private readonly ILogger _logger = Log.ForContext<SkillInfoLoader>();

    public SkillInfoLoader(DirectoriesConfig directoriesConfig)
    {
        _directoriesConfig = directoriesConfig;
    }

    public async Task<bool> LoadAsync()
    {
        var filePath = Path.Combine(_directoriesConfig[DirectoryType.Data], "skills.json");
        SkillInfo.Table = JsonUtils.DeserializeFromFile<SkillInfo[]>(filePath, MoongateJsonContext.Default);

        return true;
    }
}
