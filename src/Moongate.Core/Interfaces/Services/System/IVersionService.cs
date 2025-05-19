using Moongate.Core.Data.Internal;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface IVersionService : IMoongateStartStopService
{
    VersionInfoData GetVersionInfo();
}
