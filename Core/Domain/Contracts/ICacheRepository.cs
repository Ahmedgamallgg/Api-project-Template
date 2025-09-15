namespace Domain.Contracts;
public interface ICacheRepository
{
    Task<TResult> GetAsync<TResult>(string cashKey);
    Task SetAsync(string cashKey, string value, TimeSpan expiration);
    Task SetAsync<T>(string cashKey, T value, TimeSpan expiration);

}
