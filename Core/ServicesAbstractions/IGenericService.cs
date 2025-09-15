using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IGenericService<TEntity, TKey, TListDto, TUpsertDto>
        where TEntity : class
    {
        Task<PaginatedResponse<TListDto>> GetAllAsync(
            PagedRequest request,
            Expression<Func<TEntity, bool>>? extraFilter = null,
            IEnumerable<Expression<Func<TEntity, string>>>? searchSelectors = null,
            Expression<Func<TEntity, object>>? defaultOrderBy = null,
            bool defaultOrderDesc = false);

        Task<TListDto?> GetByIdAsync(TKey id);
        Task<TListDto> CreateAsync(TUpsertDto dto);
        Task<TListDto> UpdateAsync(TKey id, TUpsertDto dto);
        Task DeleteAsync(TKey id);
    }
}
