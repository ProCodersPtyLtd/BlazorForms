using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Extensions
{
    public static class SortExtensions
    {
        public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, SortDirection direction, Func<TSource, TKey> keySelector)
        {
            if (direction == SortDirection.Desc)
            {
                return source.OrderByDescending(keySelector);
            }
            else if (direction == SortDirection.Asc)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderBy(a => 1);
        }

        public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, SortDirection direction, Expression<Func<TSource, TKey>> keySelector)
        {
            if (direction == SortDirection.Desc)
            {
                return source.OrderByDescending(keySelector);
            }
            else if (direction == SortDirection.Asc)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderBy(a => 1);
        }
    }
}
