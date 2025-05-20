using Moongate.Core.Interfaces.Services.Base;
using Serilog;

namespace Moongate.Core.Services.Base;

public abstract class AbstractBaseMoongateService : IMoongateService
{
    protected AbstractBaseMoongateService(ILogger logger)
    {
        Logger = logger;
    }

    protected ILogger Logger { get; }

    public virtual void Dispose()
    {
    }
}
