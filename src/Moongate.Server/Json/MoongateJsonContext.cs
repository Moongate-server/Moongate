using System.Text.Json.Serialization;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Configs.Server.Sections;

namespace Moongate.Server.Json;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    AllowTrailingCommas = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(MoongateServerConfig))]
[JsonSerializable(typeof(ShardConfig))]
[JsonSerializable(typeof(NetworkConfig))]
public partial class MoongateJsonContext : JsonSerializerContext
{
}
