using Moongate.Core.Data.Ids;
using Moongate.Core.Interfaces.Services.Base;
using Moongate.Uo.Data.Entities;

namespace Moongate.Uo.Services.Interfaces.Services;

public interface IMobileService : IMoongateStartStopService
{
    MobileEntity? GetMobileBySerial(Serial serial);
    void AddMobile(MobileEntity mobile);

    Task SaveAsync(CancellationToken cancellationToken);
    MobileEntity? CreateMobile();
}
