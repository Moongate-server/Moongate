using Moongate.Core.Network.Middleware;
using Moongate.Uo.Network.Compression;

namespace Moongate.Uo.Network.Middlewares;

public class OutgoingCompressionMiddleware : INetMiddleware
{
    public void ProcessSend(ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output)
    {
        var inputBuffer = input.Span.ToArray();

        Span<byte> outputBuffer = stackalloc byte[inputBuffer.Length];
        var compressionSize = NetworkCompression.Compress(inputBuffer, outputBuffer);

        if (compressionSize == 0)
        {
            output = inputBuffer;
        }

        output = new Memory<byte>(outputBuffer[..compressionSize].ToArray());
    }

    public (bool halt, int consumedFromOrigin) ProcessReceive(
        ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output
    )
    {
        output = input;
        return (true, input.Length);
    }
}
