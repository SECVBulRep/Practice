using Microsoft.Data.Sqlite;

namespace EfVsDapper.App;
using Dapper;
using System.Data.SqlClient;

public static class SqlConnectionExtensions
{
    public static void TrackEntity<T>(this SqliteConnection connection, T entity)
    {
        var trackedEntities = connection.GetTrackedEntities<T>();
        trackedEntities.Add(entity);
    }

    private static IList<T> GetTrackedEntities<T>(this SqliteConnection connection)
    {
        const string trackedEntitiesKey = "TrackedEntities";
        if (!connection.TryGetCachedTrackedEntities(out IList<T> trackedEntities))
        {
            trackedEntities = new List<T>();
            connection.CacheTrackedEntities(trackedEntities);
        }
        return trackedEntities;
    }

    private static void CacheTrackedEntities<T>(this SqliteConnection connection, IList<T> trackedEntities)
    {
        const string trackedEntitiesKey = "TrackedEntities";
        connection.StateChange += (sender, args) =>
        {
            if (args.CurrentState == System.Data.ConnectionState.Closed)
            {
                connection.SetCachedTrackedEntities(trackedEntities);
            }
        };
    }

    private static void SetCachedTrackedEntities<T>(this SqliteConnection connection, IList<T> trackedEntities)
    {
        const string trackedEntitiesKey = "TrackedEntities";
        connection.RemoveFromCache(trackedEntitiesKey);
        connection.AddToCache(trackedEntitiesKey, trackedEntities);
    }

    private static bool TryGetCachedTrackedEntities<T>(this SqliteConnection connection, out IList<T> trackedEntities)
    {
        const string trackedEntitiesKey = "TrackedEntities";
        if (connection.TryGetFromCache(trackedEntitiesKey, out trackedEntities))
        {
            return true;
        }
        return false;
    }

    private static void AddToCache<T>(this SqliteConnection connection, string key, T value)
    {
        if (connection.State != System.Data.ConnectionState.Closed)
        {
            throw new InvalidOperationException("Connection must be closed to add to cache.");
        }
        var cache = connection.GetType().GetProperty("Cache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(connection);
        cache?.GetType().GetMethod("Add", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)?.Invoke(cache, new object[] { key, value });
    }

    private static bool TryGetFromCache<T>(this SqliteConnection connection, string key, out T value)
    {
        var cache = connection.GetType().GetProperty("Cache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(connection);
        var tryGetValueMethod = cache?.GetType().GetMethod("TryGetValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var arguments = new object[] { key, null };
        var result = (bool)(tryGetValueMethod?.Invoke(cache, arguments) ?? false);
        value = (T)arguments[1];
        return result;
    }

    private static void RemoveFromCache(this SqliteConnection connection, string key)
    {
        var cache = connection.GetType().GetProperty("Cache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(connection);
        cache?.GetType().GetMethod("Remove", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)?.Invoke(cache, new object[] { key });
    }
}