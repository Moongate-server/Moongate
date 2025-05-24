namespace Moongate.Core.Interfaces.DataLoader;

public interface IDataFileLoader
{
    Task<bool> LoadAsync();
    bool IsLoaded { get; }
    string[] RequiredFiles { get; }
}
