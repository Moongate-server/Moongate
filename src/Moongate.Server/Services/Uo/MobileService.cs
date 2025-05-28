using Moongate.Core.Data.Ids;
using Moongate.Core.Directories;
using Moongate.Core.Types;
using Moongate.Persistence.Extensions;
using Moongate.Persistence.Interfaces.Services;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Services.Interfaces.Services;
using Serilog;

namespace Moongate.Server.Services.Uo;

public class MobileService : IMobileService
{
    private readonly ILogger _logger = Log.ForContext<MobileService>();

    private readonly IPersistenceManager _persistenceManager;
    private readonly Dictionary<Serial, MobileEntity> _mobiles = new();

    private Serial _lastSerial = new Serial(Serial.ItemOffset - 1);


    private readonly string _mobilesFilePath;

    public MobileService(DirectoriesConfig directoriesConfig, IPersistenceManager persistenceManager)
    {
        _persistenceManager = persistenceManager;
        _mobilesFilePath = Path.Combine(directoriesConfig[DirectoryType.Saves], "mobiles.moongate");
    }

    public MobileEntity? GetMobileBySerial(Serial serial)
    {
        if (_mobiles.TryGetValue(serial, out MobileEntity? mobile))
        {
            return mobile;
        }

        _logger.Warning("Mobile with serial {Serial} not found.", serial);
        return null;
    }

    public void AddMobile(MobileEntity mobile)
    {
        if (!_mobiles.TryAdd(mobile.Serial, mobile))
        {
            _logger.Warning("Mobile with serial {Serial} already exists.", mobile.Serial);
            return;
        }

        _logger.Debug("Added mobile with serial {Serial}.", mobile.Serial);
    }

    public MobileEntity? CreateMobile()
    {
        var serial = new Serial(_lastSerial.Value + 1);
        _logger.Debug("Creating new mobile with serial {Serial}.", serial);

        var mobile = new MobileEntity()
        {
            Serial = serial
        };

        _lastSerial = serial;

        AddMobile(mobile);

        return mobile;
    }

    private async Task LoadMobilesAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_mobilesFilePath))
        {
            return;
        }

        var entities = await _persistenceManager.LoadEntitiesAsync(_mobilesFilePath, cancellationToken);

        if (entities.TryGetValue(typeof(MobileEntity), out var mobileEntities))
        {
            foreach (var entity in mobileEntities)
            {
                if (entity is MobileEntity mobile)
                {
                    _mobiles.Add(mobile.Serial, mobile);
                    _lastSerial = mobile.Serial;
                }
            }

            _logger.Information("Loaded {Count} mobiles.", mobileEntities.Count);
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken)
    {
        return _persistenceManager.SaveEntitiesAsync(_mobilesFilePath, _mobiles.Values, cancellationToken);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await LoadMobilesAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await SaveAsync(cancellationToken);
    }

    public void Dispose()
    {
    }
}
