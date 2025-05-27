using System.Text.Json.Serialization;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Data.Configs.Server.Sections;
using Moongate.Core.Web.Interfaces.Services;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Json.Converters;

namespace Moongate.Server.Json;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    AllowTrailingCommas = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true,
    Converters = [
        typeof(Moongate.Core.Json.Converters.FlagsConverter<>),
        typeof(ClientVersionConverter)
    ]
)]
[JsonSerializable(typeof(MoongateServerConfig))]
[JsonSerializable(typeof(ShardConfig))]
[JsonSerializable(typeof(NetworkConfig))]
[JsonSerializable(typeof(WebServerConfig))]
[JsonSerializable(typeof(ExpansionInfo))]
[JsonSerializable(typeof(ExpansionInfo[]))]
[JsonSerializable(typeof(ClientVersion))]
[JsonSerializable(typeof(SkillInfo))]
[JsonSerializable(typeof(SkillInfo[]))]
[JsonSerializable(typeof(ProfessionInfo))]
[JsonSerializable(typeof(ProfessionInfo[]))]
public partial class MoongateJsonContext : JsonSerializerContext
{
}
