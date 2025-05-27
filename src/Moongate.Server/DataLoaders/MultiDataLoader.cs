using System.IO.Compression;
using Moongate.Core.Buffers;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Core.Spans;
using Moongate.Uo.Data.Compression;
using Moongate.Uo.Data.Files;
using Moongate.Uo.Data.Mul;
using Moongate.Uo.Data.MultiData;
using Prima.UOData.Mul;

namespace Moongate.Server.DataLoaders;

public class MultiDataLoader : IDataFileLoader
{
    public async Task<bool> LoadAsync()
    {
        var multiUOPPath = UoFiles.GetFilePath("MultiCollection.uop");

        if (File.Exists(multiUOPPath))
        {
            LoadUOP(multiUOPPath);
            return false;
        }

        var postHSMulFormat = true;

        LoadMul(postHSMulFormat);

        return true;
    }

    private void LoadUOP(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        // TODO: Find out if housing.bin is needed and read that.
        var uopEntries = UOPFiles.ReadUOPIndexes(stream, ".bin", 0x10000, 4, 6);

        byte[] compressionBuffer = STArrayPool<byte>.Shared.Rent(0x10000);
        var buffer = STArrayPool<byte>.Shared.Rent(0x10000);

        foreach (var (i, entry) in uopEntries)
        {
            stream.Seek(entry.Offset, SeekOrigin.Begin);

            Span<byte> data;

            if (entry.Compressed)
            {
                if (stream.Read(buffer.AsSpan(0, entry.CompressedSize)) != entry.CompressedSize)
                {
                    throw new FileLoadException($"Error loading file {stream.Name}.");
                }

                var decompressedSize = entry.Size;
                if (Deflate.Standard.Unpack(compressionBuffer, buffer, out var bytesDecompressed) != LibDeflateResult.Success
                    || decompressedSize != bytesDecompressed)
                {
                    throw new FileLoadException($"Error loading file {stream.Name}. Failed to unpack entry {i}.");
                }

                data = compressionBuffer.AsSpan(0, decompressedSize);
            }
            else
            {
                data = buffer.AsSpan(0, entry.Size);
            }

            var tileList = new List<MultiTileEntry>();

            var reader = new SpanReader(data);

            reader.Seek(4, SeekOrigin.Begin); // Skip the first 4 bytes
            var count = reader.ReadUInt32LE();

            for (uint t = 0; t < count; t++)
            {
                var itemId = reader.ReadUInt16LE();
                var x = reader.ReadInt16LE();
                var y = reader.ReadInt16LE();
                var z = reader.ReadInt16LE();
                var flagValue = reader.ReadUInt16LE();

                var tileFlag = flagValue switch
                {
                    1   => TileFlag.None,
                    257 => TileFlag.Generic,
                    _   => TileFlag.Background // 0
                };

                var clilocsCount = reader.ReadUInt32LE();
                var skip = (int)Math.Min(clilocsCount, int.MaxValue) * 4; // bypass binary block
                reader.Seek(skip, SeekOrigin.Current);

                tileList.Add(new MultiTileEntry(itemId, x, y, z, tileFlag));
            }

            MultiData.Components[i] = new MultiComponentList(tileList);
        }

        STArrayPool<byte>.Shared.Return(buffer);
        STArrayPool<byte>.Shared.Return(compressionBuffer);
    }

    private static void LoadMul(bool postHSMulFormat)
    {
        var idxPath = UoFiles.GetFilePath("multi.idx");
        var mulPath = UoFiles.GetFilePath("multi.mul");

        using var idx = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var idxReader = new BinaryReader(idx);

        using var stream = new FileStream(mulPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bin = new BinaryReader(stream);

        var count = (int)(idx.Length / 12);
        for (var i = 0; i < count; i++)
        {
            var lookup = idxReader.ReadInt32();
            var length = idxReader.ReadInt32();
            idx.Seek(4, SeekOrigin.Current); // Extra

            if (lookup < 0 || length <= 0)
            {
                continue;
            }

            bin.BaseStream.Seek(lookup, SeekOrigin.Begin);
            MultiData.Components[i] = new MultiComponentList(bin, length, postHSMulFormat);
        }
    }
}
