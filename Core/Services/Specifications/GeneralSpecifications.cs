using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    internal class GeneralSpecifications<T> : BaseSpecifications<T> where T : class
    {
        public GeneralSpecifications() : base(null!) { }
        public GeneralSpecifications(int PageIndex, int PageSize) : base(null!)
        {
            ApplyPagination(PageIndex, PageSize);
        }
        public GeneralSpecifications(Expression<Func<T, bool>> expression) : base(expression) { }
        public GeneralSpecifications(Expression<Func<T, bool>> expression, int PageIndex, int PageSize) : base(expression)
        {
            ApplyPagination(PageIndex, PageSize);
        }

    }

    public static class SearchPredicate
    {
        /// <summary>
        /// Builds: f1.Contains(term) OR f2.Contains(term) ... (case-insensitive, null-safe)
        /// If no fields provided, returns 'true'.
        /// </summary>
        public static Expression<Func<T, bool>> Build<T>(string term, params Expression<Func<T, string?>>[] fields)
        {
            var needle = (term ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(needle) || fields is not { Length: > 0 })
                return _ => true;

            var param = Expression.Parameter(typeof(T), "e");
            Expression? body = null;

            foreach (var field in fields)
            {
                // swap parameter
                var access = new ReplaceParamVisitor(field.Parameters[0], param).Visit(field.Body)!;

                // (field ?? string.Empty).ToLower().Contains(needle)
                var coalesce = Expression.Coalesce(access, Expression.Constant(string.Empty, typeof(string)));
                var toLower = Expression.Call(coalesce, typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
                var contains = Expression.Call(
                    toLower,
                    typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
                    Expression.Constant(needle)
                );

                body = body is null ? contains : Expression.OrElse(body, contains);
            }

            body ??= Expression.Constant(true);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private sealed class ReplaceParamVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _from;
            private readonly Expression _to;
            public ReplaceParamVisitor(ParameterExpression from, Expression to) { _from = from; _to = to; }
            protected override Expression VisitParameter(ParameterExpression node)
                => node == _from ? _to : base.VisitParameter(node);
        }
    }

}
