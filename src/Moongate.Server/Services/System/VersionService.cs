using System.Reflection;
using Moongate.Core.Data.Internal;
using Moongate.Core.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Services.System;

namespace Moongate.Server.Services.System;

public class VersionService : IVersionService
{
    private readonly ITextTemplateService _templateService;

    public VersionService(ITextTemplateService templateService)
    {

        _templateService = templateService;

        var versionInfo = GetVersionInfo();

        _templateService.AddVariable("version", versionInfo.Version);
        _templateService.AddVariable("codename", versionInfo.CodeName);
        _templateService.AddVariable("commit", versionInfo.GitHash);
        _templateService.AddVariable("branch", versionInfo.Branch);
        _templateService.AddVariable("commit_date", versionInfo.BuildDate);


    }

    public VersionInfoData GetVersionInfo()
    {
        var version = typeof(VersionService).Assembly.GetName().Version;

        var codename = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "Codename")
            ?.Value;

        return new VersionInfoData(
            "Moongate",
            codename,
            version.ToString(),
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch,
            ThisAssembly.Git.CommitDate
        );
    }

    public void Dispose()
    {

    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
