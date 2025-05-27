using Moongate.Uo.Data.Geometry;

namespace Moongate.Uo.Data.Interfaces.Entities;

public interface IEntity
{
    Point3D Location { get; set; }
    Map Map { get; }

}
