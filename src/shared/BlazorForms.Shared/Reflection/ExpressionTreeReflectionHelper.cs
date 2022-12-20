using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared
{
    public static class ExpressionTreeReflectionHelper
    {
        public static Func<object, object> CreateGetter(Type runtimeType, string propertyName)
        {
            var propertyInfo = runtimeType.GetProperty(propertyName);

            // create a parameter (object obj)
            var obj = Expression.Parameter(typeof(object), "obj");

            // cast obj to runtimeType
            var objT = Expression.TypeAs(obj, runtimeType);

            // property accessor
            var property = Expression.Property(objT, propertyInfo);

            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<object, object>)Expression.Lambda(convert, obj).Compile();
        }
    }
}
