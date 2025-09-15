
namespace Persistence.Repositories;
internal class CasheRepository(IConnectionMultiplexer connection)
    : ICasheRepository
{

    private readonly IDatabase _database = connection.GetDatabase();


    public async Task<TResult> GetAsync<TResult>(string cashKey)
    {

        var value = await _database.StringGetAsync(cashKey);
        if (value.IsNullOrEmpty) return default!;
        return JsonSerializer.Deserialize<TResult>(value)!;
    }

    public async Task SetAsync(string cashKey, string value, TimeSpan expiration)
    => await _database.StringSetAsync(cashKey, value, expiration);

    public async Task SetAsync<T>(string cashKey, T value, TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(cashKey, json, expiration);
    }
}
