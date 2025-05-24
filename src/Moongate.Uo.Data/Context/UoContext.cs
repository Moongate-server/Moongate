using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data.Context;

public static class UoContext
{
    public static ClientVersion ServerVersion { get; set; }

    public static Expansion Expansion { get; set; }
}
