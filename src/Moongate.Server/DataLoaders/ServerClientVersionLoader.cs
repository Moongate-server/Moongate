using System.Buffers.Binary;
using Moongate.Core.Interfaces.DataLoader;
using Moongate.Uo.Data;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Files;
using Serilog;

namespace Moongate.Server.DataLoaders;

public class ServerClientVersionLoader : IDataFileLoader
{
    private readonly ILogger _logger = Log.ForContext<ServerClientVersionLoader>();


    public async Task<bool> LoadAsync()
    {
        ClientVersion clientVersion = null;
        _logger.Information("Determining client version...");

        // if (!string.IsNullOrEmpty(_primaServerConfig.Shard.ClientVersion))
        // {
        //     _logger.LogInformation("Client version set to {@clientVersion}", _primaServerConfig.Shard.ClientVersion);
        //
        //     clientVersion = new ClientVersion(_primaServerConfig.Shard.ClientVersion);
        //
        //     if (clientVersion == null)
        //     {
        //         _logger.LogError("Invalid client version format: {@clientVersion}", _primaServerConfig.Shard.ClientVersion);
        //         throw new ArgumentException("Invalid client version format");
        //     }
        // }

        var uoClassic = UoFiles.FindDataFile("client.exe");

        if (!string.IsNullOrEmpty(uoClassic))
        {
            await using FileStream fs = new FileStream(uoClassic, FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = GC.AllocateUninitializedArray<byte>((int)fs.Length, true);
            _ = fs.Read(buffer);
            // VS_VERSION_INFO (unicode)
            Span<byte> vsVersionInfo =
            [
                0x56, 0x00, 0x53, 0x00, 0x5F, 0x00, 0x56, 0x00,
                0x45, 0x00, 0x52, 0x00, 0x53, 0x00, 0x49, 0x00,
                0x4F, 0x00, 0x4E, 0x00, 0x5F, 0x00, 0x49, 0x00,
                0x4E, 0x00, 0x46, 0x00, 0x4F, 0x00
            ];

            var versionIndex = buffer.AsSpan().IndexOf(vsVersionInfo);
            if (versionIndex > -1)
            {
                var offset = versionIndex + 42; // 30 + 12

                var minorPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset));
                var majorPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 2));
                var privatePart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 4));
                var buildPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 6));

                clientVersion = new ClientVersion(majorPart, minorPart, buildPart, privatePart);
            }
        }

        if (clientVersion == null)
        {
            _logger.Error("Client version not found");
            throw new InvalidOperationException("Client version not found");
        }

        _logger.Information("Client version found: {ClientVersion}", clientVersion);
        UoContext.ServerVersion = clientVersion;

        return true;
    }
}
