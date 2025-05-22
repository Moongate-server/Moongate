namespace Moongate.Persistence.Utils;

/// <summary>
///     Simple CRC32 implementation for data integrity
/// </summary>
internal static class CRC32
{
    private static readonly uint[] Table = GenerateTable();

    private static uint[] GenerateTable()
    {
        var table = new uint[256];
        const uint polynomial = 0xEDB88320;

        for (uint i = 0; i < 256; i++)
        {
            var crc = i;
            for (var j = 0; j < 8; j++)
            {
                crc = (crc & 1) == 1 ? (crc >> 1) ^ polynomial : crc >> 1;
            }

            table[i] = crc;
        }

        return table;
    }

    public static uint Compute(ReadOnlySpan<byte> data)
    {
        var crc = 0xFFFFFFFF;

        foreach (var b in data)
        {
            crc = Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
        }

        return ~crc;
    }
}
