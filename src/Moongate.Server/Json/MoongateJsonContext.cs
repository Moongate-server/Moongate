using System.Text.Json.Serialization;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Configs.Server.Sections;
using Moongate.Core.Web.Interfaces.Services;

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
[JsonSerializable(typeof(WebServerConfig))]
public partial class MoongateJsonContext : JsonSerializerContext
{
}
