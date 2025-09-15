using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Exceptions;
using Domain.Models;
using Services.Specifications;
using Shared;
using System.Linq.Expressions;

namespace Services
{
    public class GenericService<TEntity, TKey, TListDto, TUpsertDto>
   : IGenericService<TEntity, TKey, TListDto, TUpsertDto> where TEntity : BaseEntity<TKey>
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<TEntity,TKey> _repo;
        private readonly IMapper _mapper;

        public GenericService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _repo = uow.GetRepository<TEntity,TKey>();
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<TListDto>> GetAllAsync(
            PagedRequest request,
            Expression<Func<TEntity, bool>>? extraFilter = null,
            IEnumerable<Expression<Func<TEntity, string>>>? searchSelectors = null,
            Expression<Func<TEntity, object>>? defaultOrderBy = null,
            bool defaultOrderDesc = false)
        {
            var pageIndex = Math.Max(1, request.PageIndex);
            var pageSize = Math.Clamp(request.PageSize, 1, 500);

            // Build base spec with filter + pagination
            var spec = new GeneralSpecifications<TEntity>(
                expression: extraFilter ?? (_ => true),
                PageIndex: (pageIndex - 1) * pageSize,
                PageSize: pageSize
            );

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm) && searchSelectors?.Any() == true)
            {
                spec.AddSearchTerm(request.SearchTerm!, searchSelectors.ToArray());
            }

            // Determine ordering:
            // 1) If request.OrderBy provided and resolves, use it (asc/desc via request.OrderDesc).
            // 2) Otherwise use provided defaultOrderBy (asc/desc via defaultOrderDesc).
            Expression<Func<TEntity, object>>? resolvedOrderBy = null;

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                resolvedOrderBy = BuildOrderByByName(request.OrderBy!);
                if (resolvedOrderBy is not null)
                {
                    if (request.OrderDesc) spec.AddOrderByDesc(resolvedOrderBy);
                    else spec.AddOrderBy(resolvedOrderBy);
                }
            }

            if (resolvedOrderBy is null && defaultOrderBy is not null)
            {
                if (defaultOrderDesc) spec.AddOrderByDesc(defaultOrderBy);
                else spec.AddOrderBy(defaultOrderBy);
            }

            // Fetch page
            var entities = await _repo.GetAllAsync(spec);

            // Count with the same filter + search (no pagination)
            var countSpec = new GeneralSpecifications<TEntity>(extraFilter ?? (_ => true));
            if (!string.IsNullOrWhiteSpace(request.SearchTerm) && searchSelectors?.Any() == true)
            {
                countSpec.AddSearchTerm(request.SearchTerm!, searchSelectors.ToArray());
            }
            var total = await _repo.CountAsync(countSpec);

            // Map
            var items = entities
                .AsQueryable()
                .ProjectTo<TListDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new PaginatedResponse<TListDto>(pageIndex, pageSize, total, items);
           
        }

        public async Task<TListDto?> GetByIdAsync(TKey id)
        {
            var entity = await _repo.GetAsync(id!);
            return entity is null ? default : _mapper.Map<TListDto>(entity);
        }

        public async Task<TListDto> CreateAsync(TUpsertDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _repo.Add(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<TListDto>(entity);
        }

        public async Task<TListDto> UpdateAsync(TKey id, TUpsertDto dto)
        {
            var entity = await _repo.GetAsync(id!)
                ?? throw new BadRequestException($"{typeof(TEntity).Name} with id '{id}' not found.");

            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<TListDto>(entity);
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await _repo.GetAsync(id!)
                ?? throw new BadRequestException($"{typeof(TEntity).Name} with id '{id}' not found.");

            _repo.Delete(entity);
            await _uow.SaveChangesAsync();
        }

        private static Expression<Func<TEntity, object>>? BuildOrderByByName(string propertyName)
        {
            var prop = typeof(TEntity).GetProperty(propertyName);
            if (prop is null) return null;

            var param = Expression.Parameter(typeof(TEntity), "x");
            var body = Expression.Convert(Expression.Property(param, prop), typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(body, param);
        }
    }
}
