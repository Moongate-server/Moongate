using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Moongate.Server;

class Program
{
    public static CancellationTokenRegistration _quitTokenRegistration = new();

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.UseServiceProviderFactory(new DryIocServiceProviderFactory());

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        builder.ConfigureLogging(loggingBuilder => { loggingBuilder.ClearProviders().AddSerilog(); });

        var app = builder.Build();

        await app.RunAsync(_quitTokenRegistration.Token);
    }
}
