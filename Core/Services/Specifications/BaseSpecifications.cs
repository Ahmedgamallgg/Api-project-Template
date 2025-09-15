using System.Linq.Expressions;

namespace Services.Specifications;
internal abstract class BaseSpecifications<T>(Expression<Func<T, bool>>? criteria)
    : ISpecifications<T>
    where T : class
{
    public Expression<Func<T, bool>> Criteria { get; set; } = criteria!;
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDesc { get; private set; }

    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPaginated { get; private set; }
    public void ApplyPagination(int pageSize, int pageIndex)
    {
        IsPaginated = true;
        Take = pageSize;
        Skip = (pageIndex - 1) * pageSize;
    }
    public void AddInclude(Expression<Func<T, object>> expression) => Includes.Add(expression);
    public void AddOrderBy(Expression<Func<T, object>> expression) => OrderBy = expression;
    public void AddOrderByDesc(Expression<Func<T, object>> expression) => OrderByDesc = expression;

    public void AddSearchTerm(string? term, params Expression<Func<T, string?>>[] fields)
    {
        var needle = (term ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(needle) || fields == null || fields.Length == 0)
            return;

        // Build (f1 != null && f1.ToLower().Contains(needleLower)) || ...
        var param = Expression.Parameter(typeof(T), "e");
        var needleConst = Expression.Constant(needle.ToLowerInvariant());

        Expression? anyFieldMatches = null;

        foreach (var field in fields)
        {
            // Replace field parameter with our unified param
            var body = new ReplaceParamVisitor(field.Parameters[0], param).Visit(field.Body)!;

            // (field != null) ? field.ToLower() : "" 
            var nullCoalesce = Expression.Coalesce(
                body,
                Expression.Constant(string.Empty, typeof(string))
            );

            var toLower = Expression.Call(nullCoalesce, typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
            var contains = Expression.Call(
                toLower,
                typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
                needleConst
            );

            anyFieldMatches = anyFieldMatches is null ? contains : Expression.OrElse(anyFieldMatches, contains);
        }

        if (anyFieldMatches is not null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(anyFieldMatches, param);
            AndCriteria(lambda);
        }

    }
    public void AndCriteria(Expression<Func<T, bool>> predicate)
    {
        Criteria = Criteria is null ? predicate : PredicateExtensions.And(Criteria, predicate);  //Criteria.And(predicate);
    }
    public void OrCriteria(Expression<Func<T, bool>> predicate)
    {
        Criteria = Criteria is null ? predicate : Criteria.Or(predicate);
    }
    private sealed class ReplaceParamVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly Expression _to;
        public ReplaceParamVisitor(ParameterExpression from, Expression to)
        {
            _from = from; _to = to;
        }
        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    }


}

internal static class SpecExpressionExtensions
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

    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.OrElse(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
