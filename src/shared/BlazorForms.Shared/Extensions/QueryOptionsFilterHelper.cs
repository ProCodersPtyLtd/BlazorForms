using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Globalization;

namespace BlazorForms.Shared.Extensions
{
    public static class QueryOptionsFilterHelper
    {
        public static IQueryable<Q> ApplyFilters<Q>(IQueryable<Q> query, QueryOptions queryOptions, Type type = null)
        {
            var filters = queryOptions.Filters;
            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Filter))
                {
                    continue;
                }

                switch (filter.FilterType)
                {
                    case FieldFilterType.Text:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), filter.Filter, FieldFilterType.Text, type);
                        break;
                    case FieldFilterType.TextStarts:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.TextStarts, type);
                        break;
                    case FieldFilterType.TextEnds:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.TextEnds, type);
                        break;
                    case FieldFilterType.TextContains:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.TextContains, type);
                        break;
                    case FieldFilterType.Select:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.Select, type);
                        break;
                    case FieldFilterType.MultiSelect:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.MultiSelect, type);
                        break;
                    case FieldFilterType.DateExpressionFromDate:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DateExpressionFromDate, type);
                        break;
                    case FieldFilterType.DateExpressionToDate:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DateExpressionToDate, type);
                        break;
                    case FieldFilterType.DateExpressionEqual:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DateExpressionEqual, type);
                        break;
                    case FieldFilterType.DateExpressionRange:
                        if (filter.FromDate != null)
                        {
                            query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.FromDate}", FieldFilterType.DateExpressionFromDate, type);
                        }
                        if (filter.ToDate != null)
                        {
                            query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.ToDate}", FieldFilterType.DateExpressionToDate, type);
                        }
                        break;
                    case FieldFilterType.DecimalEqual:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DecimalEqual, type);
                        break;
                    case FieldFilterType.Integer:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.Integer, type);
                        break;
                    case FieldFilterType.DecimalGreaterThan:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DecimalGreaterThan, type);
                        break;
                    case FieldFilterType.DecimalLessThan:
                        query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.Filter}", FieldFilterType.DecimalLessThan, type);
                        break;
                    case FieldFilterType.DecimalRange:
                        if (filter.GreaterThan != null)
                        {
                            query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.GreaterThan}", FieldFilterType.DecimalGreaterThan, type);
                        }
                        if (filter.LessThan != null)
                        {
                            query = WhereLike(query, queryOptions.GetFieldMapping(filter.BindingProperty), $"{filter.LessThan}", FieldFilterType.DecimalLessThan, type);
                        }
                        break;
                }
            }
            return query;
        }

        public static IQueryable<Q> WhereLike<Q>(IQueryable<Q> query, string colName, string searchPattern, FieldFilterType filterType, Type type = null)
        {
            if (searchPattern == null)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(Q), "x");
            var DateTimeType = typeof(DateTimeOffset);
            Expression property;
            Expression MethodCall;

            if (colName.StartsWith("$."))
            {
                // Extract and convert Model to its type
                property = Expression.Convert(Expression.Property(Expression.Property(parameter, "Context"), "Model"), type);
                // iterate to the last property in sequence
                var fields = colName.Replace("$.", "").Split('.');

                foreach (var field in fields)
                {
                    property = Expression.Property(property, field);
                }
            }
            else
            {
                property = Expression.Property(parameter, colName);
            };

            if (property.Type.Name == "Nullable`1")
            {
                DateTimeType = typeof(DateTimeOffset?);
            }


            if (filterType == FieldFilterType.MultiSelect)
            {
                var searchPatterns = searchPattern.Replace("; ", ".").Split('.');
                var containsMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "Contains");
                MethodInfo method = null;
                foreach (var m in containsMethods)
                {
                    if (m.GetParameters().Count() == 2)
                    {
                        method = m;
                        break;
                    }
                }
                method = method.MakeGenericMethod(property.Type);

                MethodCall = Expression.Call(method, new Expression[] { Expression.Constant(searchPatterns), property });
            }
            else if (filterType == FieldFilterType.DateExpressionFromDate)
            {
                var date1 = DateTimeOffset.TryParse(searchPattern, out var date1Value) ? date1Value : default;
                MethodCall = Expression.GreaterThanOrEqual(property, Expression.Constant(date1, DateTimeType));
            }
            else if (filterType == FieldFilterType.DateExpressionToDate)
            {
                var date1 = DateTimeOffset.TryParse(searchPattern, out var date1Value) ? date1Value : default;
                MethodCall = Expression.LessThanOrEqual(property, Expression.Constant(date1, DateTimeType));
            }
            else if (filterType == FieldFilterType.DateExpressionEqual)
            {
                var date1 = DateTimeOffset.TryParse(searchPattern, out var date1Value) ? date1Value : default;
                Expression antes = Expression.GreaterThanOrEqual(property, Expression.Constant(date1.Date.AddDays(-1), DateTimeType));
                Expression despues = Expression.LessThanOrEqual(property, Expression.Constant(date1.Date.AddDays(1), DateTimeType));
                MethodCall = Expression.AndAlso(antes, despues);
            }
            else if (filterType == FieldFilterType.DecimalEqual)
            {
                var decimalVal = Convert.ToDecimal(searchPattern);
                MethodCall = Expression.Equal(property, Expression.Constant(decimalVal, decimalVal.GetType()));
            }
            else if (filterType == FieldFilterType.Integer)
            {
                var val = Convert.ToInt32(searchPattern);
                var underlyingType = Nullable.GetUnderlyingType(property.Type);

                if (property.Type.IsEnum)
                {
                    var enumVal = Enum.ToObject(property.Type, val);
                    MethodCall = Expression.Equal(property, Expression.Constant(enumVal, property.Type));
                }
                else if (underlyingType is { IsEnum: true })
                {
                    var enumToObject = typeof(Enum).GetMethod(nameof(Enum.ToObject),
                        BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Type), typeof(int) }, null);
                    var enumVal = enumToObject?.Invoke(null, new object[] { underlyingType, val });
                    var nullableEnumVal = Activator.CreateInstance(property.Type, enumVal);
                    MethodCall = Expression.Equal(property, Expression.Constant(nullableEnumVal, property.Type));
                }
                else if (underlyingType is { IsPrimitive: true })
                {
                    var nullableIntVal = Activator.CreateInstance(property.Type, val);
                    MethodCall = Expression.Equal(property, Expression.Constant(nullableIntVal, property.Type));
                }
                else
                {
                    MethodCall = Expression.Equal(property, Expression.Constant(val, val.GetType()));
                }
            }
            else if (filterType == FieldFilterType.DecimalLessThan)
            {
                var decimalVal = Convert.ToDecimal(searchPattern);
                MethodCall = Expression.LessThanOrEqual(property, Expression.Constant(decimalVal - 1, decimalVal.GetType()));
            }
            else if (filterType == FieldFilterType.DecimalGreaterThan)
            {
                var decimalVal = Convert.ToDecimal(searchPattern);
                MethodCall = Expression.GreaterThanOrEqual(property, Expression.Constant(decimalVal + 1, decimalVal.GetType()));
            }
            else
            {
                MethodCall = Expression.Call(property, "contains", null, Expression.Constant(searchPattern));
            };

            var lambdaExpression = Expression.Lambda<Func<Q, bool>>(MethodCall, parameter);
            return query.Where(lambdaExpression);
        }
    }

}
