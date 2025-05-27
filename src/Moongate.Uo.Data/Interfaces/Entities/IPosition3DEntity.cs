using Moongate.Uo.Data.Geometry;

namespace Moongate.Uo.Data.Interfaces.Entities;

public interface IPosition3DEntity
{
    public Point3D Location { get; set; }
}
