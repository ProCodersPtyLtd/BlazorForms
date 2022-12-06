using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorForms.Shared
{
    public static class JsonPathHelper
    {
        public static Type GetItemsType(Type modelType, string modelBinding)
        {
            var type = GetTypeIterateThroughPath(modelType, modelBinding);
            return GetItemsType(type);
        }

        public static Type GetItemsType(Type type)
        {
            if (!type.IsGenericType)
            {
                throw new ArgumentException($"The type '{type.Name}' is not generic");
            }

            return type.GenericTypeArguments[0];
        }

        public static Type GetTypeIterateThroughPath(Type modelType, string modelBinding)
        {
            var path = modelBinding.Replace("$", "").Split('.').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            var currentType = modelType;
            PropertyInfo currentProp = null;

            foreach (var propName in path)
            {
                //if (propName.Contains("[") && propName.Contains("]"))
                //{
                //    var propSplit = propName.Split('[');
                //    var dictPropName = propSplit[0];
                //    var dictKey = propSplit[1].Split(']')[0];
                //    dictKey = dictKey.Replace("'", "").Replace("\"", "");

                //    var prop = currentType.GetProperty(dictPropName);
                //    var dict = prop.GetValue(currentObject);

                //    if (dict.GetType().IsGenericType && dict.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                //    {
                //        var dictObj = dict as IDictionary;
                //        currentObject = dictObj[dictKey];
                //    }
                //    else if (dict.GetType().IsGenericType && dict.GetType().GetGenericTypeDefinition() == typeof(List<>))
                //    {
                //        var lictObj = dict as IList;
                //        var index = int.Parse(dictKey);
                //        currentObject = lictObj[index];
                //    }
                //    else
                //    {
                //        throw new NotImplementedException("Only Dictionary<string, > supported");
                //    }
                //}
                //else
                {
                    currentProp = currentType.GetProperty(propName);
                    currentType = currentProp.PropertyType;
                }

                if (currentType == null)
                {
                    return null;
                }
            }

            return currentType;
        }

        public static string ReplaceLambdaVar(string selectorString)
        {
            var result = selectorString.Trim();
            var values = result.Split('.');

            if (values.Length > 1)
            {
                result = result.Substring(values[0].Length + 1);
                result = "$." + result;
            }

            return result;
        }

        public static string RemoveLastProperty(string selectorString)
        {
            var result = selectorString.Trim();

            if (result == "$.")
            {
                return result;
            }

            var values = result.Split('.').ToList();
            values.RemoveAt(values.Count - 1);
            result = string.Join(".", values);

            if (result == "$")
            {
                return "$.";
            }

            return result;
        }

        /// <summary>
        /// Evaluates JSON path against specified root type
        /// </summary>
        /// <param name="selectorString">JSON path selector string to evaluate</param>
        /// <param name="processAction">Action that is executed against each level of selector evaluation</param>
        /// <typeparam name="TRoot">Root element type to evaluate against</typeparam>
        public static void Evaluate<TRoot>(string selectorString, Action<IJsonPathContext> processAction)
        {
            var currentType = typeof(TRoot);
            
            foreach (var part in selectorString.Split("."))
            {
                if (part == "$")
                {
                    processAction(new JsonPathContext {IsRoot = true});
                }
                else if (part == "@")
                {
                    processAction(new JsonPathContext {IsCurrent = true});
                }
                else
                {
                    var prop = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                    
                    if (prop is null)
                    {
                        throw new NotSupportedException(
                            $"Type '{currentType.FullName}' referenced by selector {selectorString} must define a public instance property '{part}'");
                    }

                    processAction(new JsonPathContext {PropertyInfo = prop});
                    currentType = prop.PropertyType;
                }
            }
        }

        private class JsonPathContext : IJsonPathContext
        {
            public bool IsRoot { get; init; }
            public bool IsCurrent { get; init; }
            public PropertyInfo? PropertyInfo { get; init; }
        }
    }

    public interface IJsonPathContext
    {
        /// <summary>
        /// Indicates that selector targets root element
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Indicates that selector targets current element (that was already processed before) 
        /// </summary>
        bool IsCurrent { get; }

        /// <summary>
        /// For a non-root and non-current element points to selected member info 
        /// </summary>
        PropertyInfo? PropertyInfo { get; }
    }
}