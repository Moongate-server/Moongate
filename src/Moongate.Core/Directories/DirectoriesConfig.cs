using Moongate.Core.Extensions;

namespace Moongate.Core.Directories;

public class DirectoriesConfig
{
    private readonly string _rootDirectory;
    private readonly string[] _directories;

    public DirectoriesConfig(string rootDirectory, params string[] directories)
    {
        _rootDirectory = rootDirectory;
        _directories = directories;

        Init();
    }

    public string Root => _rootDirectory;


    public string this[string directoryType] => GetPath(directoryType);


    public string this[Enum directoryType] => GetPath(directoryType.ToString());

    public string GetPath<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return GetPath(Enum.GetName(value));
    }

    public string GetPath(string directoryType)
    {
        var path = Path.Combine(_rootDirectory, directoryType.ToSnakeCase());

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private void Init()
    {
        if (!Directory.Exists(_rootDirectory))
        {
            Directory.CreateDirectory(_rootDirectory);
        }

        var directoryTypes = _directories.ToList();


        foreach (var directory in directoryTypes)
        {
            var path = GetPath(directory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    public override string ToString()
    {
        return _rootDirectory;
    }
}
