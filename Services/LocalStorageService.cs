using Microsoft.Maui.Storage;
using System.Threading.Tasks;

public class LocalStorageService
{
    public Task SetAsync<T>(string key, T value)
    {
        Preferences.Set(key, value?.ToString());
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        if (Preferences.ContainsKey(key))
        {
            var value = Preferences.Get(key, string.Empty);
            return Task.FromResult((T?)Convert.ChangeType(value, typeof(T)));
        }
        return Task.FromResult(default(T?));
    }
}
