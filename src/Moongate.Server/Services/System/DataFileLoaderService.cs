using System.Diagnostics;
using DryIoc;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Services.Base;
using Moongate.Uo.Services.Files;
using Serilog;
using ZLinq;

namespace Moongate.Server.Services.System;

public class DataFileLoaderService : AbstractBaseMoongateService, IDataFileLoaderService
{
    private readonly IContainer _container;

    private readonly Dictionary<int, List<IDataFileLoader>> _dataFileLoaders = new();

    public DataFileLoaderService(IContainer container, MoongateServerConfig serverConfig) : base(
        Log.ForContext<DataFileLoaderService>()
    )
    {
        _container = container;

        UoFiles.ScanForFiles(serverConfig.Shard.UoDirectory);

        Logger.Information("Found {Count} data files", UoFiles.MulPath.Values.Count);
    }


    public void AddDataLoaderType(Type dataLoader, int priority)
    {
        if (!typeof(IDataFileLoader).IsAssignableFrom(dataLoader))
        {
            throw new InvalidOperationException($"Type {dataLoader.Name} does not implement IDataFileLoader.");
        }

        if (!_container.IsRegistered(dataLoader))
        {
            _container.Register(dataLoader, Reuse.Singleton);
        }

        if (!_dataFileLoaders.TryGetValue(priority, out List<IDataFileLoader>? value))
        {
            value = ( []);
            _dataFileLoaders[priority] = value;
        }

        var dataLoaderInstance = _container.Resolve(dataLoader) as IDataFileLoader;
        value.Add(dataLoaderInstance);

        Logger.Debug("Registered data loader: {DataLoader} with priority {Priority}", dataLoader.Name, priority);
    }

    public async Task LoadDataLoadersAsync(CancellationToken cancellationToken = default)
    {
        Logger.Information("Loading data loaders...");

        var startTime = Stopwatch.GetTimestamp();

        var orderedLoaders = _dataFileLoaders.AsValueEnumerable()
            .OrderBy(kv => kv.Key)
            .SelectMany(kv => kv.Value)
            .ToList();

        foreach (var loader in orderedLoaders)
        {
            Logger.Information("Loading data with {DataLoader}", loader.GetType().Name);
            await loader.LoadAsync();
        }

        Logger.Information("Data loaders loaded successfully in {ElapsedMs} ms ", Stopwatch.GetElapsedTime(startTime));
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return LoadDataLoadersAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
