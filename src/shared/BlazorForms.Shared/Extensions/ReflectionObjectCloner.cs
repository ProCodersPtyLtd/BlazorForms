using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorForms.Shared
{
    // ToDo: this class needs unit tests
    public static class ReflectionObjectCloner
    {

        static ReflectionObjectCloner()
        {
        }

        public static object New(this object source)
        {
            var type = source.GetType();
            var target = Activator.CreateInstance(type);
            return target;
        }

        public static object New(this object source, Guid id)
        {
            var type = source.GetType();
            var target = Activator.CreateInstance(type, id);
            return target;
        }

        public static T GetCopy<T>(this T source)
            where T : class
        {
            var target = New(source);
            source.ReflectionCopyTo(target);
            return (T)target;
        }

        public static object GetCopy(this object source)
        {
            var target = New(source);
            source.ReflectionCopyTo(target);
            return target;
        }

        public static object GetCopy(this object source, Guid id)
        {
            var target = New(source, id);
            source.ReflectionCopyTo(target);
            return target;
        }

        // ToDo: add FastReflectionCopyTo
        public static void ReflectionCopyTo(this object source, object target)
        {
            var properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (property.GetSetMethod() != null)
                {
                    var targetProperty = target.GetType().GetProperty(property.Name);

                    if (targetProperty != null)
                    {
                        var value = property.GetValue(source);
                        targetProperty.SetValue(target, value);
                    }
                }
            }
        }

        public static void CopyListTo(this System.Collections.IList source, System.Collections.IList target)
        {
            target.Clear();

            foreach (var item in source)
            {
                var copy = item.GetCopy();
                target.Add(copy);
            }
        }
    }
}
