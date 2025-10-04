using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace pizzeria.Models
{
    public class PagedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public QueryOptions Options { get; }

        public PagedList(IQueryable<T> source, QueryOptions options)
        {
            Options = options ?? new QueryOptions();

            if (!string.IsNullOrWhiteSpace(options.SearchPropertyName) && !string.IsNullOrWhiteSpace(options.SearchTerm))
            {
                var pred = BuildContains(options.SearchPropertyName, options.SearchTerm);
                if (pred != null) source = source.Where(pred);
            }

            if (!string.IsNullOrWhiteSpace(options.FilterPropertyName) && !string.IsNullOrWhiteSpace(options.FilterTerm))
            {
                var pred = BuildContains(options.FilterPropertyName, options.FilterTerm);
                if (pred != null) source = source.Where(pred);
            }

            if (!string.IsNullOrWhiteSpace(options.OrderPropertyName))
                source = ApplyOrder(source, options.OrderPropertyName, options.DescendingOrder);

            var count = source.Count();
            PageIndex = options.PageNumber <= 0 ? 1 : options.PageNumber;
            if (options.PageSize <= 0) options.PageSize = 12;
            TotalPages = (int)Math.Ceiling(count / (double)options.PageSize);

            AddRange(source.Skip((PageIndex - 1) * options.PageSize).Take(options.PageSize).ToList());
        }

        static IQueryable<T> ApplyOrder(IQueryable<T> source, string path, bool desc)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;
            foreach (var part in path.Split('.'))
                body = Expression.PropertyOrField(body, part);
            var lambda = Expression.Lambda(body, param);
            var method = desc ? "OrderByDescending" : "OrderBy";
            var call = Expression.Call(typeof(Queryable), method, new[] { typeof(T), body.Type }, source.Expression, Expression.Quote(lambda));
            return source.Provider.CreateQuery<T>(call);
        }

        static Expression<Func<T, bool>> BuildContains(string path, string term)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression body = param;
            foreach (var part in path.Split('.'))
                body = Expression.PropertyOrField(body, part);
            if (body.Type != typeof(string))
            {
                var toString = body.Type.GetMethod("ToString", Type.EmptyTypes);
                body = Expression.Call(body, toString);
            }
            var toLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            body = Expression.Call(body, toLower);
            var constant = Expression.Constant(term.ToLower());
            var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var call = Expression.Call(body, contains, constant);
            return Expression.Lambda<Func<T, bool>>(call, param);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
