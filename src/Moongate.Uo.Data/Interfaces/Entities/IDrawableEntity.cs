namespace Moongate.Uo.Data.Interfaces.Entities;

public interface IDrawableEntity : ISerialEntity
{
    Body Body { get; }

    int Hue { get; set; }
}
