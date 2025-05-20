using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Moongate.Core.Json;

namespace Moongate.Tests;

[TestFixture]
public class JsonUtilsTests
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test_json.json");
    private TestEntity _testEntity;

    [SetUp]
    public void Setup()
    {
        // Reset options to default before each test
        JsonUtils.RebuildJsonOptions();

        // Create test entity
        _testEntity = new TestEntity
        {
            Id = 1,
            Name = "Test Entity",
            Status = TestStatus.Active
        };

        // Delete test file if it exists
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test file
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Test]
    public void RebuildJsonOptions_WithDefaults_SetsDefaultProperties()
    {
        // Act
        JsonUtils.RebuildJsonOptions();

        // Assert
        Assert.That(JsonUtils.DefaultSerializerOptions.PropertyNamingPolicy, Is.EqualTo(JsonNamingPolicy.CamelCase));
        Assert.That(JsonUtils.DefaultSerializerOptions.WriteIndented, Is.True);
    }

    [Test]
    public void RebuildJsonOptions_WithCustomOptions_UsesCustomOptions()
    {
        // Arrange
        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = false
        };

        // Act
        JsonUtils.RebuildJsonOptions(customOptions);

        // Assert
        Assert.That(JsonUtils.DefaultSerializerOptions.PropertyNamingPolicy, Is.Null);
        Assert.That(JsonUtils.DefaultSerializerOptions.WriteIndented, Is.False);
    }

    [Test]
    public void AddTypeConverter_AddsConverterToOptions()
    {
        // Arrange
        var converter = new CustomJsonConverter<TestEntity>();
        var initialCount = JsonUtils.DefaultSerializerOptions.Converters.Count;

        // Act
        JsonUtils.AddTypeConverter(converter);

        // Assert
        Assert.That(JsonUtils.DefaultSerializerOptions.Converters.Count, Is.EqualTo(initialCount + 1));
        Assert.That(JsonUtils.DefaultSerializerOptions.Converters, Contains.Item(converter));
    }

    [Test]
    public void AddConverter_AddsConverterToOptions()
    {
        // Arrange
        var converter = new CustomJsonConverter<TestEntity>();
        var initialCount = JsonUtils.DefaultSerializerOptions.Converters.Count;

        // Act
        JsonUtils.AddConverter(converter);

        // Assert
        Assert.That(JsonUtils.DefaultSerializerOptions.Converters.Count, Is.EqualTo(initialCount + 1));
        Assert.That(JsonUtils.DefaultSerializerOptions.Converters, Contains.Item(converter));
    }

    [Test]
    public void AddTypeChainConverter_AddsResolverToChain()
    {
        // Arrange
        var resolver = new TestJsonTypeInfoResolver();
        var initialCount = JsonUtils.DefaultSerializerOptions.TypeInfoResolverChain.Count;

        // Act
        JsonUtils.AddTypeChainConverter<TestEntity>(resolver);

        // Assert
        Assert.That(JsonUtils.DefaultSerializerOptions.TypeInfoResolverChain.Count, Is.EqualTo(initialCount + 1));
        Assert.That(JsonUtils.DefaultSerializerOptions.TypeInfoResolverChain, Contains.Item(resolver));
    }


    [Test]
    public void Serialize_WithCustomOptions_UsesCustomOptions()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = false
        };

        // Act
        var json = JsonUtils.Serialize(_testEntity, options);

        // Assert
        Assert.That(json, Does.Contain("\"Id\":1"));
        Assert.That(json, Does.Contain("\"Name\":\"Test Entity\""));
    }

    [Test]
    public void Deserialize_WithValidJson_ReturnsCorrectEntity()
    {
        // Arrange
        var json = "{\"id\":1,\"name\":\"Test Entity\",\"status\":\"active\"}";

        // Act
        var entity = JsonUtils.Deserialize<TestEntity>(json);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(1));
        Assert.That(entity.Name, Is.EqualTo("Test Entity"));
        Assert.That(entity.Status, Is.EqualTo(TestStatus.Active));
    }

    [Test]
    public void Deserialize_WithCustomOptions_UsesCustomOptions()
    {
        // Arrange
        var json = "{\"Id\":1,\"Name\":\"Test Entity\",\"Status\":\"Active\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            Converters = { new JsonStringEnumConverter() }
        };

        // Act
        var entity = JsonUtils.Deserialize<TestEntity>(json, options);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(1));
        Assert.That(entity.Name, Is.EqualTo("Test Entity"));
        Assert.That(entity.Status, Is.EqualTo(TestStatus.Active));
    }

    [Test]
    public async Task SerializeAsync_WithValidEntity_ReturnsCorrectJson()
    {
        // Act
        var json = await JsonUtils.SerializeAsync(_testEntity);

        // Assert
        Assert.That(json, Does.Contain("\"id\": 1"));
    }

    [Test]
    public async Task DeserializeAsync_WithValidJson_ReturnsCorrectEntity()
    {
        // Arrange
        var json = "{\"id\":1,\"name\":\"Test Entity\",\"status\":\"active\"}";

        // Act
        var entity = await JsonUtils.DeserializeAsync<TestEntity>(json);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(1));
        Assert.That(entity.Name, Is.EqualTo("Test Entity"));
        Assert.That(entity.Status, Is.EqualTo(TestStatus.Active));
    }

    [Test]
    public void SerializeToFile_WritesCorrectJsonToFile()
    {
        // Act
        JsonUtils.SerializeToFile(_testEntity, _testFilePath);

        // Assert
        Assert.That(File.Exists(_testFilePath), Is.True);
        var fileContent = File.ReadAllText(_testFilePath);

        Assert.That(fileContent, Is.Not.Empty);
    }


    [Test]
    public async Task SerializeToFileAsync_WritesCorrectJsonToFile()
    {
        // Act
        await JsonUtils.SerializeToFileAsync(_testEntity, _testFilePath);

        // Assert
        Assert.That(File.Exists(_testFilePath), Is.True);
        var fileContent = await File.ReadAllTextAsync(_testFilePath);

        Assert.That(fileContent, Is.Not.Empty);
    }

    [Test]
    public async Task DeserializeFromFileAsync_ReadsCorrectEntityFromFile()
    {
        // Arrange
        var json = "{\"id\":1,\"name\":\"Test Entity\",\"status\":\"active\"}";
        File.WriteAllText(_testFilePath, json);

        // Act
        var entity = await JsonUtils.DeserializeFromFileAsync<TestEntity>(_testFilePath);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(1));
        Assert.That(entity.Name, Is.EqualTo("Test Entity"));
        Assert.That(entity.Status, Is.EqualTo(TestStatus.Active));
    }

    [Test]
    public void Deserialize_WithInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "{\"id\":1,\"name\":\"Test Entity\",\"status\":\"active\""; // Missing closing brace

        // Act & Assert
        Assert.That(() => JsonUtils.Deserialize<TestEntity>(invalidJson), Throws.TypeOf<JsonException>());
    }
}

// Test classes for use in tests
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TestStatus Status { get; set; }
}

public enum TestStatus
{
    Active,
    Inactive
}

public class CustomJsonConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class TestJsonTypeInfoResolver : IJsonTypeInfoResolver
{
    public JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        return null;
    }
}
