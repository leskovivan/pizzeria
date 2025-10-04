using System;
using System.Collections.Generic;
using System.Linq;

namespace pizzeria.Models
{
    public class PagedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public QueryOptions Options { get; }

        public PagedList(IQueryable<T> source, QueryOptions options)
        {
            Options = options;
            PageIndex = options.PageNumber;
            var count = source.Count();

            if (!string.IsNullOrEmpty(options.SearchPropertyName) && !string.IsNullOrEmpty(options.SearchTerm))
            {

            }

            if (!string.IsNullOrEmpty(options.OrderPropertyName))
            {

            }

            TotalPages = (int)Math.Ceiling(count / (double)options.PageSize);
            AddRange(source.Skip((PageIndex - 1) * options.PageSize).Take(options.PageSize));
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
