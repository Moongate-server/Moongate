using Microsoft.Extensions.Logging;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Services.Base;

public abstract class AbstractBaseMoongateStartStopService : AbstractBaseMoongateService, IMoongateStartStopService
{
    protected AbstractBaseMoongateStartStopService(ILogger<AbstractBaseMoongateService> logger) : base(logger)
    {
    }

    public abstract Task StartAsync(CancellationToken cancellationToken = default);


    public abstract Task StopAsync(CancellationToken cancellationToken = default);
}
