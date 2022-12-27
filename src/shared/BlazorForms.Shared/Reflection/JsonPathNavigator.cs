using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Shared
{
    public interface IJsonPathNavigator
    {
        object GetValue(object model, string modelBinding);
        IEnumerable<object> GetItems(object model, string itemsBinding);
        void SetValue(object model, string modelBinding, object val);
    }

    public class JsonPathNavigator : IJsonPathNavigator
    {
        public IEnumerable<object> GetItems(object model, string itemsBinding)
        {
            var result = GetValue(model, itemsBinding);
            return result as IEnumerable<object>;
        }

        public object GetValue(object model, string modelBinding)
        {
            if (modelBinding == "")
            {
                return model;
            }

            object targetObject;
            string lastProperty;
            var property = GetLastPropertyIterateThroughPath(model, modelBinding, out targetObject, out lastProperty);

            if (targetObject != null && targetObject.GetType() == typeof(ExpandoObject))
            {
                return (targetObject as ExpandoObject).GetValue(lastProperty);
            }
                
            if (property == null)
            {
                return null;
            }

            var result = property.GetValue(targetObject);
            return result;
        }

        public void SetValue(object model, string modelBinding, object val)
        {
            object targetObject;
            string lastProperty;
            var prop = GetLastPropertyIterateThroughPath(model, modelBinding, out targetObject, out lastProperty);

            if (targetObject.GetType() == typeof(ExpandoObject))
            {
                (targetObject as ExpandoObject).SetValue(lastProperty, val);
                return;
            }

            if (prop == null || targetObject == null || prop.SetMethod == null)
            {
                return;
            }

            if (prop.PropertyType == typeof(Int32))
            {
                prop.SetValue(targetObject, Convert.ToInt32(val));
            }
            else if (prop.PropertyType == typeof(Decimal))
            {
                prop.SetValue(targetObject, Convert.ToDecimal(val));
            }
            else if (prop.PropertyType == typeof(Decimal?))
            {
                prop.SetValue(targetObject, val == null ? (decimal?)null : Convert.ToDecimal(val));
            }
            else if (prop.PropertyType == typeof(Int32?))
            {
                prop.SetValue(targetObject, val == null ? (Int32?)null : Convert.ToInt32(val));
            }
            else
            {
                prop.SetValue(targetObject, val);
            }
        }

        private PropertyInfo GetLastPropertyIterateThroughPath(object model, string modelBinding, out object target, out string lastPropName)
        {
            target = null;
            var path = modelBinding.Replace("$", "").Split('.').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            object currentObject = model;
            lastPropName = path.Last();
            path.RemoveAt(path.Count - 1);

            foreach (var propName in path)
            {
                if (propName.Contains("[") && propName.Contains("]"))
                {
                    var propSplit = propName.Split('[');
                    var dictPropName = propSplit[0];
                    var dictKey = propSplit[1].Split(']')[0];
                    dictKey = dictKey.Replace("'", "").Replace("\"", "");

                    var prop = currentObject.GetType().GetProperty(dictPropName);
                    var dict = prop.GetValue(currentObject);

                    if(dict.GetType().IsGenericType && dict.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        var dictObj = dict as IDictionary;
                        currentObject = dictObj[dictKey];
                    }
                    else if (dict.GetType().IsGenericType && dict.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var lictObj = dict as IList;
                        var index = int.Parse(dictKey);
                        currentObject = lictObj[index];
                    }
                    else
                    {
                        throw new NotImplementedException("Only Dictionary<string, > supported");
                    }
                }
                else
                {
                    var prop = currentObject.GetType().GetProperty(propName);
                    currentObject = prop.GetValue(currentObject);
                }

                if(currentObject == null)
                {
                    return null;
                }
            }

            target = currentObject;
            var lastProp = currentObject.GetType().GetProperty(lastPropName);
            return lastProp;
        }
    }
}
