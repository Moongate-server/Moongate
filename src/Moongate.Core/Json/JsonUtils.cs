using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Moongate.Core.Json;

public static class JsonUtils
{
    public static JsonSerializerOptions DefaultSerializerOptions { get; private set; } = new();


    public static void RebuildJsonOptions(JsonSerializerOptions? options = null)
    {
        DefaultSerializerOptions = options ?? new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
            },
        };
    }

    public static void AddTypeConverter<T>(JsonConverter<T> converter)
    {
        DefaultSerializerOptions.Converters.Add(converter);
    }

    public static void AddTypeChainConverter<T>(IJsonTypeInfoResolver resolver)
    {
        DefaultSerializerOptions.TypeInfoResolverChain.Add(resolver);
    }

    public static void AddConverter(JsonConverter converter)
    {
        DefaultSerializerOptions.Converters.Add(converter);
    }

    public static TEntity Deserialize<TEntity>(string json)
    {
        return Deserialize<TEntity>(json, null);
    }

    public static TEntity Deserialize<TEntity>(string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<TEntity>(json, options ?? DefaultSerializerOptions);
    }


    public static string Serialize<TEntity>(TEntity entity, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(entity, options ?? DefaultSerializerOptions);
    }


    public static async Task<TEntity> DeserializeAsync<TEntity>(string json, JsonSerializerOptions? options = null)
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        return await JsonSerializer.DeserializeAsync<TEntity>(stream, options ?? DefaultSerializerOptions);
    }

    public static async Task<string> SerializeAsync<TEntity>(TEntity entity, JsonSerializerOptions? options = null)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, entity, options ?? DefaultSerializerOptions);
        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    public static void SerializeToFile<TEntity>(TEntity entity, string filePath, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        JsonSerializer.Serialize(stream, entity, options ?? DefaultSerializerOptions);
    }

    public static TEntity DeserializeFromFile<TEntity>(string filePath, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        return JsonSerializer.Deserialize<TEntity>(stream, options ?? DefaultSerializerOptions);
    }

    public static async Task SerializeToFileAsync<TEntity>(
        TEntity entity, string filePath, JsonSerializerOptions? options = null
    )
    {
        await using var stream = new FileStream(filePath, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, entity, options ?? DefaultSerializerOptions);
    }

    public static async Task<TEntity> DeserializeFromFileAsync<TEntity>(
        string filePath, JsonSerializerOptions? options = null
    )
    {
        await using var stream = new FileStream(filePath, FileMode.Open);
        return await JsonSerializer.DeserializeAsync<TEntity>(stream, options ?? DefaultSerializerOptions);
    }
}
