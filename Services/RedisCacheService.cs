using System.Text.Json;
using JurMaps.Services.Interfaces;
using StackExchange.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }

    public async Task SetValueAsync(string key, object value, TimeSpan? expiration = null)
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, jsonData, expiration);
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis error: {ex.Message}");
        }
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis error: {ex.Message}");
            return default;
        }
    }

    public async Task RemoveValueAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis error: {ex.Message}");
        }
    }
}
