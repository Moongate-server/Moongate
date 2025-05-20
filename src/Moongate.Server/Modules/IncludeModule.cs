using Moongate.Core.Attributes.Scripts;
using Moongate.Core.Directories;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Types;

namespace Moongate.Server.Modules;

[ScriptModule("include")]
public class IncludeModule
{
    private readonly IScriptEngineService _scriptEngineService;
    private readonly DirectoriesConfig _directoriesConfig;

    public IncludeModule(IScriptEngineService scriptEngineService, DirectoriesConfig directoriesConfig)
    {
        _scriptEngineService = scriptEngineService;
        _directoriesConfig = directoriesConfig;
    }

    [ScriptFunction("file")]
    public void IncludeFile(string file)
    {
        var fullPath = Path.Combine(_directoriesConfig[DirectoryType.Scripts], file);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {fullPath}");
        }

        _scriptEngineService.ExecuteFileAsync(fullPath).Wait();
    }
}
