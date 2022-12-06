using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BlazorForms.Shared.Extensions
{
    public static class QueryOptionsSortHelper
    {
        public static IQueryable<T> OrderBy<T>(IQueryable<T> query, QueryOptions queryOptions, Type type = null)
        {

            var propertyName = queryOptions.GetFieldMapping(queryOptions.SortColumn);
            var direction = queryOptions.SortDirection;

            if (direction == SortDirection.None)
            {
                return (IOrderedQueryable<T>)query;
            }

            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property;

            if (propertyName.StartsWith("$."))
            {
                // Extract and convert Model to its type
                property = Expression.Convert(Expression.Property(Expression.Property(parameter, "Context"), "Model"), type);

                // iterate to the last property in sequence
                var fields = propertyName.Replace("$.", "").Split('.');

                foreach (var field in fields)
                {
                    property = Expression.Property(property, field);
                }
            }
            else
            {
                property = Expression.Property(parameter, propertyName);
            }

            var lambda = Expression.Lambda(property, parameter);

            var sortDirection = direction == SortDirection.Asc ? "OrderBy" : "OrderByDescending";

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == sortDirection && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { query, lambda });

            return (IOrderedQueryable<T>)result;
        }
    }
}
