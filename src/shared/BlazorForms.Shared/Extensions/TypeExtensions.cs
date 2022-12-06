using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSimple(this Type t)
        {
            return t.IsPrimitive || t == typeof(DateTime) || t == typeof(DateTime?) || t == typeof(string) || Nullable.GetUnderlyingType(t) != null;
        }

        public static bool IsBoxable(this Type t)
        {
            return t.IsPrimitive || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(DateTime?) || Nullable.GetUnderlyingType(t) != null;
        }

        public static bool IsList(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
