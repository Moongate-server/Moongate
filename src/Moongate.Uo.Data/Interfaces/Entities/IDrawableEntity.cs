namespace Moongate.Uo.Data.Interfaces.Entities;

public interface IDrawableEntity : ISerialEntity
{
    int ModelId { get; set; }

    int Hue { get; set; }

}
