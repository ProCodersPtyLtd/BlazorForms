using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public static IOrderedQueryable<TSource> QueryOrderByDirection<TSource>(this IQueryable<TSource> query, SortDirection direction, string propertyName)
        {
            var entityType = typeof(TSource);
            string methodName = direction == SortDirection.Desc ? "OrderByDescending" : "OrderBy";

            LambdaExpression selector;
            var propertyInfo = entityType.GetProperty(propertyName);

            if (direction == SortDirection.None)
            {
                //Create x=>1
                // This one doesn't work
                ParameterExpression arg = Expression.Parameter(typeof(int), "x");
                ConstantExpression constant = Expression.Constant(1);
                selector = Expression.Lambda(constant, arg);
            }
            else
            {
                //Create x=>x.PropName
                ParameterExpression arg = Expression.Parameter(entityType, "x");
                MemberExpression property = Expression.Property(arg, propertyName);
                selector = Expression.Lambda(property, new ParameterExpression[] { arg });
            }

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);

            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();

            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }
    }
}
