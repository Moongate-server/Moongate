using Moongate.Core.Network.Middleware;
using Moongate.Uo.Network.Compression;

namespace Moongate.Uo.Network.Middlewares;

public class OutgoingCompressionMiddleware : INetMiddleware
{
    public void ProcessSend(ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output)
    {
        var outputBuffer = new Span<byte>();
        NetworkCompression.Compress(input.Span, outputBuffer);

        output = outputBuffer.ToArray();
    }

    public (bool halt, int consumedFromOrigin) ProcessReceive(
        ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output
    )
    {
        output = default;
        return (true, 0);
    }
}
