using MemoryPack;
using Moongate.Core.Data.Ids;
using Moongate.Persistence.Attributes;
using Moongate.Uo.Data.Entities.Base;
using Moongate.Uo.Data.Interfaces.Entities;
using Moongate.Uo.Data.Races.Base;
using Moongate.Uo.Data.Serializers;
using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data.Entities;

[MemoryPackable]
[EntityType(0x04, Description = "Mobile Entity")]

public partial class MobileEntity : Entity, IDrawableEntity
{
    private readonly Dictionary<Layer, ItemEntity> _items = new();
    public Serial Serial { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public int ModelId { get; set; }
    public int Hue { get; set; }

    public CharacterStatus Status { get; set; }

    public Notoriety Notoriety { get; set; }

    public Direction Direction { get; set; }

    public int Strength { get; set; }

    public int Intelligence { get; set; }

    [MemoryPackAllowSerialize]
    public Race Race { get; set; }


    [MemoryPackAllowSerialize]
    public ProfessionInfo Profession { get; set;}

    public int Dexterity { get; set; }

    public int MaxStamina { get; set; }

    public int Stamina { get; set; }

    public int MaxMana { get; set; }

    public int Mana { get; set; }

    public int Weight { get; set; }

    public int MaxWeight { get; set; }

    public int CurrentHits { get; set; }

    public int MaxHits { get; set; }

    public ItemEntity? GetItemByLayer(Layer layer)
    {
        _items.TryGetValue(layer, out var item);
        return item;
    }

    public void SetItemByLayer(Layer layer, ItemEntity item)
    {
        _items[layer] = item;
    }

    public Dictionary<Layer, ItemEntity> GetItems()
    {
        return _items;
    }
}
