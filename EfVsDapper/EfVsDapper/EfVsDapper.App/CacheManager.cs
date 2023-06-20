namespace EfVsDapper.App;

public static class CacheManager
{
    private static readonly Dictionary<string, object> cache = new Dictionary<string, object>();

    public static T Get<T>(string key)
    {
        if (cache.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        return default;
    }

    public static void Set<T>(string key, T value)
    {
        cache[key] = value;
    }
}