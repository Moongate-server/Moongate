using Microsoft.Extensions.Logging;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Services.Base;

public abstract class AbstractBaseMoongateService : IMoongateService
{
    protected AbstractBaseMoongateService(ILogger<AbstractBaseMoongateService> logger)
    {
        Logger = logger;
    }

    protected ILogger Logger { get; }

    public virtual void Dispose()
    {
    }
}
