namespace Domain.Contracts;
public interface ICasheRepository
{
    Task<TResult> GetAsync<TResult>(string cashKey);
    Task SetAsync(string cashKey, string value, TimeSpan expiration);
    Task SetAsync<T>(string cashKey, T value, TimeSpan expiration);

}
