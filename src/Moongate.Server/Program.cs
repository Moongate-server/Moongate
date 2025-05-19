using Moongate.Core.Interfaces.Services.System;
using Moongate.Server.Provider;
using Serilog;

namespace Moongate.Server;

class Program
{
    public static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();


        Log.Information("Starting Moongate Server...");


        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        try
        {
            MoongateServiceProvider.Instance.GetService<IEventBusService>();
            MoongateServiceProvider.Instance.GetService<IVersionService>();

            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Handle cancellation
            Log.Information("Cancellation requested. Exiting...");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while running the application.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
