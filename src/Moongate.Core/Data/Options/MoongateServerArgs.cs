using Moongate.Core.Types;

namespace Moongate.Core.Data.Options;

public class MoongateServerArgs
{
    public LogLevelType DefaultLogLevel { get; set; } = LogLevelType.Debug;
    public bool LogToFile { get; set; } = true;
    public bool LoadFromEnv { get; set; } = false;
    public string? RootDirectory { get; set; } = null;
    public bool PrintHeader { get; set; } = true;
    public string ConfigName { get; set; } = "moongate.json";
}
