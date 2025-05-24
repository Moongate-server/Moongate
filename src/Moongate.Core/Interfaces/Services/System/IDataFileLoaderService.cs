using Moongate.Core.Interfaces.DataLoader;

namespace Moongate.Core.Interfaces.Services.System;

public interface IDataFileLoaderService
{
    void AddDataLoaderType(Type dataLoader, int priority);
}
