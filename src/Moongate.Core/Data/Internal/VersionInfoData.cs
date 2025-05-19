namespace Moongate.Core.Data.Internal;

public record VersionInfoData(
    string AppName,
    string CodeName,
    string Version,
    string GitHash,
    string Branch,
    string BuildDate
);
