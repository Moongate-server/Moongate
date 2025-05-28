using MemoryPack;
using Moongate.Uo.Data.Geometry;
using Moongate.Uo.Data.Interfaces.Entities;
using Moongate.Uo.Data.Interfaces.Geometry;

namespace Moongate.Uo.Data.Entities.Base;

[MemoryPackable]
public partial class Entity : IEntity, IPosition3DEntity
{
    public int X => Location.X;
    public int Y => Location.Y;
    public int Z => Location.Z;


    public Point3D Location { get; set; }

    [MemoryPackAllowSerialize]
    public Map Map { get; set; }

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
