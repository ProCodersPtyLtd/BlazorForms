using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Forms
{
    public static class TypeExtensions
    {
        public static bool IsSimple(this Type t)
        {
            return t.IsPrimitive || t == typeof(DateTime) || t == typeof(DateTime?) || t == typeof(decimal) || t == typeof(decimal?)
                || t == typeof(string) || Nullable.GetUnderlyingType(t) != null;
        }

        public static bool IsList(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static IEnumerable<PropertyInfo> GetSimpleTypeProperties(this Type t)
        {
            var result = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType.IsSimple());
            return result;
        }

        public static PropertyInfo GetPropertyByJsonPath(this Type t, string inputJsonPath)
        {
            var jsonPath = inputJsonPath;

            if (jsonPath.StartsWith("$."))
            {
                jsonPath = jsonPath.Replace("$.", "");
            }

            var types = jsonPath.Split('.').ToList();
            var currentType = t;

            while (types.Count() > 1)
            {
                var propName = types[0];
                types.RemoveAt(0);
                var currentProperty = currentType.GetProperty(propName);
                currentType = currentProperty.PropertyType;
            }

            var resultProperty = currentType.GetProperty(types[0]);
            return resultProperty;
        }
    }
}
