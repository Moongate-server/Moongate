using System.IO.Compression;
using Moongate.Core.Buffers;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Compression;
using Moongate.Uo.Data.Files;
using Moongate.Uo.Data.Mul;
using Prima.UOData.Mul;

namespace Moongate.Uo.Data.MultiData;

public static class MultiData
{
    public static Dictionary<int, MultiComponentList> Components { get; } = new();

    public static int Count => Components.Count;

    public static MultiComponentList GetComponents(int multiID) =>
        Components.TryGetValue(multiID & 0x3FFF, out var mcl) ? mcl : MultiComponentList.Empty;
}
