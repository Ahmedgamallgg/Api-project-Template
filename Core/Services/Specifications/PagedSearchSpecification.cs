using System.Linq.Expressions;

namespace Services.Specifications
{
    internal class PagedSearchSpecification<T> : BaseSpecifications<T> where T : class
    {
        public PagedSearchSpecification(
            int pageIndex,
            int pageSize,
            string? searchTerm = null,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderByExpr = null,
            bool ascending = true,
            params Expression<Func<T, string?>>[] searchFields)
            : base(null!)
        {
            // criteria = filter AND search
            Expression<Func<T, bool>> criteria = _ => true;

            if (filter is not null)
                criteria = PredicateExtensions.And(criteria, filter); //  criteria.And(filter);

            if (!string.IsNullOrWhiteSpace(searchTerm) && searchFields is { Length: > 0 })
                criteria = PredicateExtensions.And(criteria, SearchPredicate.Build(searchTerm!, searchFields));  //criteria.And(SearchPredicate.Build(searchTerm!, searchFields));

            Criteria = criteria;

            if (orderByExpr is not null)
            {
                if (ascending) AddOrderBy(orderByExpr);
                else AddOrderByDesc(orderByExpr);
            }
            ApplyPagination(Math.Max(1, pageIndex), Math.Max(1, pageSize));
        }
    }

    internal static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

    }

}
