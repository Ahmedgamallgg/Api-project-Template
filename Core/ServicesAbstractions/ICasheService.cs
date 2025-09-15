namespace ServicesAbstractions;
public interface ICasheService
{
    Task<string?> GetAsync(string cashKey);
    Task SetAsync(string key, object value, TimeSpan expiration);
}
