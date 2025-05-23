using Moongate.Core.Interfaces.DataLoader;

namespace Moongate.Core.Interfaces.Services.System;

public interface IDataFileLoaderService
{
    void AddDataLoader<TDataLoader>(TDataLoader dataLoader, int priority) where TDataLoader : IDataLoader<object>;
}
