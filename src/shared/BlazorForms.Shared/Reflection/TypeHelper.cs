using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Shared
{
    public static class TypeHelper
    {
        public static string GetGenericTypeName(Type t)
        {
            var result = t.Name;

            foreach(var arg in t.GenericTypeArguments)
            {
                result += $"+{arg.Name}";
            }

            return result;
        }

        public static object[] GetConstructorParameters(IServiceProvider serviceProvider, Type classType)
        {
            var methodParameters = classType.GetConstructors().FirstOrDefault()?.GetParameters();

            var parameters = new List<object>();

            foreach (var parameter in methodParameters)
            {
                var svc = serviceProvider.GetService(parameter.ParameterType);

                if (svc == null)
                {
                    throw new InvalidDependencyException($"Cannot resolve dependency for type {parameter.ParameterType}");
                }

                parameters.Add(svc);
            }

            return parameters.ToArray();
        }

        public static List<ModelProperty> GetPublicVirtualProperties(Type t, ModelProperty parent = null)
        {
            var currentProps = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => p.GetMethod?.IsPublic == true && p.GetMethod?.IsVirtual == true && p.SetMethod?.IsPublic == true && p.SetMethod?.IsVirtual == true)
                .Select(p => new ModelProperty { Property = p, Parent = parent }).ToList();

            return currentProps;
        }
        public static List<ModelProperty> GetNestedPublicVirtualProperties(Type t)
        {
            List<ModelProperty> result = new List<ModelProperty>();

            var currentProps = GetPublicVirtualProperties(t);

            while (currentProps.Any())
            {
                var mp = currentProps.First();
                result.Add(mp);
                currentProps.RemoveAt(0);

                if (mp.Property.PropertyType.IsList())
                {
                    mp.IsList = true;
                    var argt = mp.Property.PropertyType.GetGenericArguments()[0];
                    var props = GetPublicVirtualProperties(argt, mp);
                    currentProps.InsertRange(0, props);
                }
                else if (!mp.Property.PropertyType.IsSimple())
                {
                    var props = GetPublicVirtualProperties(mp.Property.PropertyType, mp);
                    currentProps.AddRange(props);
                }
            }

            return result;
        }



        public static bool IsAsyncMethod(MethodInfo method)
        {
            if(method.ReturnType?.Name != null && method.ReturnType.Name.StartsWith("Task"))
            {
                return true;
            }

            return false;
        }

        public static Type GetGenericType<I>(Type generic, object model)
            where I: class
        {
            var targetType = generic.MakeGenericType(new Type[] { model?.GetType() ?? typeof(object) });
            return targetType;
        }

        public static I GetGenericInstance<I>(Type generic, object model)
            where I : class
        {
            var targetType = GetGenericType<I>(generic, model);
            var result = Activator.CreateInstance(targetType);
            return result as I;
        }
    }

    public class ModelProperty
    {
        public PropertyInfo Property { get; internal set; }
        public ModelProperty Parent { get; internal set; }
        public bool IsList { get; internal set; }

        public string GetPath()
        {
            var result = Property.Name;
            var prop = this;

            while (prop.Parent != null && !prop.Parent.IsList)
            {
                result = $"{prop.Parent.Property.Name}.{result}";
                prop = prop.Parent;
            }

            return result;
        }
    }

}
