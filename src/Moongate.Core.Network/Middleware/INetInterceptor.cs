namespace Moongate.Core.Network.Middleware;

public interface INetInterceptor
{
    void ProcessSend(ReadOnlyMemory<byte> output);

    void ProcessReceive(ReadOnlyMemory<byte> output);

}
