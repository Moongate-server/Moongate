namespace Moongate.Uo.Data.Types;

[Flags]
public enum ItemImplFlag : byte
{
    None = 0x00,
    Visible = 0x01,
    Movable = 0x02,
    Deleted = 0x04,
    Stackable = 0x08,
    InQueue = 0x10,
    Insured = 0x20,
    PaidInsurance = 0x40,
    QuestItem = 0x80
}
