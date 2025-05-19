namespace Moongate.Core.Data.Configs.Services;

public class ScriptEngineConfig
{
    public List<string> InitScriptsFileNames { get; set; } = new List<string>() { "bootstrap.lua", "init.lua" };
}
