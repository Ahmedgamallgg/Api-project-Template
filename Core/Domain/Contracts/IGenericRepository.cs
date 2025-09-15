using Domain.Models;

namespace Domain.Contracts;
public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    IQueryable<TEntity> Query();
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<TEntity?> GetAsync(TKey key);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetAsync(ISpecifications<TEntity> specifications);
    Task<int> CountAsync(ISpecifications<TEntity> specifications);
    Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity> specifications);
}
