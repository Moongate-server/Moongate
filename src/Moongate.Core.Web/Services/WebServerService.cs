using System.Text;
using EmbedIO;
using Moongate.Core.Data.Configs.Server;
using Moongate.Core.Directories;
using Moongate.Core.Services.Base;
using Moongate.Core.Types;
using Moongate.Core.Web.Interfaces.Services;
using Moongate.Core.Web.Logging;
using Serilog;
using Swan.Logging;

namespace Moongate.Core.Web.Services;

public class WebServerService : AbstractBaseMoongateStartStopService, IWebServerService
{
    private readonly MoongateServerConfig _moongateServerConfig;
    private readonly DirectoriesConfig _directoriesConfig;

    private IWebServer? _webServer;


    public WebServerService(MoongateServerConfig moongateServerConfig, DirectoriesConfig directoriesConfig) : base(
        Log.Logger.ForContext<WebServerService>()
    )
    {
        _moongateServerConfig = moongateServerConfig;
        _directoriesConfig = directoriesConfig;
    }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_moongateServerConfig.WebServer.Enabled)
        {
            Swan.Logging.Logger.UnregisterLogger<ConsoleLogger>();

            Swan.Logging.Logger.RegisterLogger(new SwanToSerilogLogger(GetType()));


            _webServer = new WebServer(o =>
                    {
                        o
                            .WithUrlPrefix("http://0.0.0.0:" + _moongateServerConfig.WebServer.Port);
                    }
                )
                .WithAction(
                    "/",
                    HttpVerbs.Get,
                    context =>
                    {
                        context.Response.ContentType = "text/html";
                        return context.SendStringAsync("<h1>Moongate Web Server</h1>", "text/html", Encoding.UTF8);
                    }
                )
                .WithLocalSessionManager()
                .WithStaticFolder("/static", _directoriesConfig[DirectoryType.WwwRoot], true);


            Task.Run(() => _webServer.RunAsync(cancellationToken), cancellationToken);
        }
        else
        {
            Logger.Information("Web server is disabled in the configuration.");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        _webServer?.Dispose();

        return Task.CompletedTask;
    }
}
