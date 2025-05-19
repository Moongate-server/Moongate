namespace Moongate.Core.Interfaces.Services.Base;

public interface IMoongateStartStopService : IMoongateService
{
    /// <summary>
    ///  Starts the service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(CancellationToken cancellationToken = default);


    /// <summary>
    ///  Stops the service.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
