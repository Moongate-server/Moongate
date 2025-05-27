using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Interfaces.Entities;
using Moongate.Uo.Data.Interfaces.Geometry;

namespace Moongate.Uo.Data.Entities.Base;

public class Entity : IEntity
{
    public Point3D Location { get; set; }

    public int X => Location.X;
    public int Y => Location.Y;
    public int Z => Location.Z;

    public Map Map { get; private set; }


    public bool InRange(Point2D p, int range) =>
        p.X >= Location.X - range
        && p.X <= Location.X + range
        && p.Y >= Location.Y - range
        && p.Y <= Location.Y + range;

    public bool InRange(Point3D p, int range) =>
        p.X >= Location.X - range
        && p.X <= Location.X + range
        && p.Y >= Location.Y - range
        && p.Y <= Location.Y + range;

    public bool InRange(IPoint2D p, int range) =>
        p.X >= Location.X - range
        && p.X <= Location.X + range
        && p.Y >= Location.Y - range
        && p.Y <= Location.Y + range;
}
