using Moongate.Core.Interfaces.Services.Base;
using Moongate.Uo.Data.Network.Packets.Data;

namespace Moongate.Uo.Services.Interfaces.Services;

public interface IMapService : IMoongateService
{

    List<CityInfo> GetCities();

}
