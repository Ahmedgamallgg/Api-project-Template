namespace ServicesAbstractions;
public interface ICacheService
{
    Task<string?> GetAsync(string cashKey);
    Task SetAsync(string key, object value, TimeSpan expiration);
}
