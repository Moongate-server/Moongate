using System.IO.Compression;

namespace Moongate.Uo.Data.Compression;

public static class Deflate
{
    [ThreadStatic]
    private static LibDeflateBinding _standard;

    public static LibDeflateBinding Standard => _standard ??= new LibDeflateBinding();
}
