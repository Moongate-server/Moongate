using System.Runtime.CompilerServices;

namespace Moongate.Core.Utils.Random;

public static class BuiltInRng
{
    public static System.Random Generator { get; private set; } = new();

    public static void Reset() => Generator = new System.Random();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next() => Generator.Next();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int maxValue) => Generator.Next(maxValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int count) => minValue + Generator.Next(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Next(long maxValue) => Generator.NextInt64(maxValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Next(long minValue, long count) => minValue + Generator.NextInt64(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextLong() => Generator.NextInt64();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble() => Generator.NextDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NextBytes(Span<byte> buffer) => Generator.NextBytes(buffer);
}
