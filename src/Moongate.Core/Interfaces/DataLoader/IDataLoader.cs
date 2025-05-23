namespace Moongate.Core.Interfaces.DataLoader;

public interface IDataLoader<T>
{
    Task<bool> LoadAsync();
    Task<T?> GetAsync(int index);
    Task<IReadOnlyList<T>> GetAllAsync();
    bool IsLoaded { get; }
    int Count { get; }
    string[] RequiredFiles { get; }
}
