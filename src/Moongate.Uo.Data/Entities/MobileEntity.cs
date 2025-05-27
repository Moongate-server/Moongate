using Moongate.Core.Data.Ids;
using Moongate.Uo.Data.Entities.Base;
using Moongate.Uo.Data.Interfaces.Entities;

namespace Moongate.Uo.Data.Entities;

public class MobileEntity : Entity, ISerialEntity
{
    public Serial Serial { get; set; }
}
