using DryIoc;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Interfaces.Services.System;
using Moongate.Core.Services.Base;
using Moongate.Uo.Services.Files;
using Serilog;

namespace Moongate.Server.Services.System;

public class DataFileLoaderService : AbstractBaseMoongateService, IDataFileLoaderService
{
    private readonly IContainer _container;

    private readonly Dictionary<int, List<IDataFileLoader>> _dataFileLoaders = new();

    public DataFileLoaderService( IContainer container, MoongateServerConfig serverConfig) : base(Log.ForContext<DataFileLoaderService>())
    {
        _container = container;

        UoFiles.ScanForFiles(serverConfig.Shard.UoDirectory);

        Logger.Information("Found {Count} data files",UoFiles.MulPath.Values.Count);
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

        if (!_dataFileLoaders.ContainsKey(priority))
        {
            _dataFileLoaders[priority] = new List<IDataFileLoader>();
        }

        Logger.Debug("Registered data loader: {DataLoader} with priority {Priority}", dataLoader.Name, priority);
    }
}
