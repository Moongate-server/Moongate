using System.Text.RegularExpressions;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Directories;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Types;
using Serilog;

namespace Moongate.Server.Services.System;

public partial class LocalizedTextService : ILocalizedTextService
{
    private readonly ILogger _logger = Log.ForContext<LocalizedTextService>();

    private readonly MoongateServerConfig _serverConfig;

    private readonly DirectoriesConfig _directoriesConfig;

    private readonly Dictionary<int, string> _localizedText = new();

    public LocalizedTextService(
        MoongateServerConfig serverConfig, DirectoriesConfig directoriesConfig
    )
    {
        _serverConfig = serverConfig;
        _directoriesConfig = directoriesConfig;
    }

    private async Task LoadLocalizedTextAsync()
    {
        var dictionaryDirectory = Directory.GetFiles(
            Path.Combine(_directoriesConfig[DirectoryType.Dictionaries]),
            $"*." + _serverConfig.Shard.Language,
            SearchOption.AllDirectories
        );

        if (dictionaryDirectory.Length == 0)
        {
            _logger.Warning("No localized text files found in the specified directory, fallback to default.");
            dictionaryDirectory = Directory.GetFiles(
                Path.Combine(_directoriesConfig[DirectoryType.Dictionaries]),
                $"*." + "eng",
                SearchOption.AllDirectories
            );
        }

        foreach (var dictionary in dictionaryDirectory)
        {
            _logger.Information("Loading localized text from {File}", Path.GetFileName(dictionary));

            foreach (var line in File.ReadLines(dictionary))
            {
                var parts = line.Split(['='], 2);
                if (parts.Length == 2 && int.TryParse(parts[0], out var id))
                {
                    var part = parts[1].Trim();

                    if (part.Contains('#'))
                    {
                        part = part[..part.IndexOf('#')].Trim();
                    }


                    _localizedText[id] = ConvertCFormatToCSharp(part);
                }
            }
        }
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await LoadLocalizedTextAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
    }

    public string GetLocalizedText(int id)
    {
        if (_localizedText.TryGetValue(id, out var text))
        {
            return text;
        }

        _logger.Warning("Localized text with ID {Id} not found", id);
        return string.Empty;
    }

    public string GetLocalizedText(int id, params object[] args)
    {
        if (_localizedText.TryGetValue(id, out var text))
        {
            return string.Format(text, args);
        }

        _logger.Warning("Localized text with ID {Id} not found", id);
        return string.Empty;
    }

    public static string ConvertCFormatToCSharp(string input)
    {
        var regex = CToCSharpRegex();
        int index = 0;

        return regex.Replace(input, match => $"{{{index++}}}");
    }

    [GeneratedRegex(@"%[a-zA-Z]")]
    private static partial Regex CToCSharpRegex();

    public void Dispose()
    {

    }
}
