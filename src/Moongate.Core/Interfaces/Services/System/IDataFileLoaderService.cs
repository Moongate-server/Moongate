using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface IDataFileLoaderService : IMoongateStartStopService
{
    void AddDataLoaderType(Type dataLoader, int priority = -1);

    Task LoadDataLoadersAsync(CancellationToken cancellationToken = default);
}
