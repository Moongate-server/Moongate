using System.Runtime.InteropServices;

namespace Moongate.Uo.Data.Files;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Entry3D
{
    public int lookup;
    public int length;
    public int extra;
}
