namespace JurMaps.Services.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetValueAsync(string key, object value, TimeSpan? expiration = null);
        Task<T?> GetValueAsync<T>(string key);
        Task RemoveValueAsync(string key);
    }

}
