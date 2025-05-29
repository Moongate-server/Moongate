using Moongate.Core.Network.Middleware;
using Moongate.Uo.Network.Compression;
using Serilog;

namespace Moongate.Uo.Network.Middlewares;

public class OutgoingCompressionMiddleware : INetMiddleware
{
    public void ProcessSend(ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output)
    {
        var inputBuffer = input.Span.ToArray();

        Span<byte> outputBuffer = stackalloc byte[NetworkCompression.CalculateMaxCompressedSize(inputBuffer.Length)];
        var length = NetworkCompression.Compress(inputBuffer, outputBuffer);

        if (length == 0)
        {
            length = inputBuffer.Length;
        }

        output = new Memory<byte>(outputBuffer[..length].ToArray());
    }

    public (bool halt, int consumedFromOrigin) ProcessReceive(
        ref ReadOnlyMemory<byte> input, out ReadOnlyMemory<byte> output
    )
    {
        output = input;
        return (true, input.Length);
    }
}
