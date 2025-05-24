namespace Moongate.Core.Interfaces.DataLoader;

public interface IDataFileLoader
{
    Task<bool> LoadAsync();
}
