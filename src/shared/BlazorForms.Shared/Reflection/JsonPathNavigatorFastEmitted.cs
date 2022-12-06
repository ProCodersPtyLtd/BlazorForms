using BlazorForms.Shared.Reflection;
using Sigil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Shared
{
    public class JsonPathNavigatorFastEmitted : IJsonPathNavigator
    {
        private readonly FastReflectionPrototype _fastReflection = new FastReflectionPrototype();

        public IEnumerable<object> GetItems(object model, string itemsBinding)
        {
            var result = GetValue(model, itemsBinding);
            return result as IEnumerable<object>;
        }

        public object GetValue(object model, string modelBinding)
        {
            object targetObject;
            string lastProperty;
            var value = GetLastPropertyIterateThroughPath(model, modelBinding, out targetObject, out lastProperty);
            return value;

            //if (targetObject != null && targetObject.GetType() == typeof(ExpandoObject))
            //{
            //    return (targetObject as ExpandoObject).GetValueStringKey(lastProperty);
            //}
                
            //if (property == null)
            //{
            //    return null;
            //}

            //var result = property.GetValueStringKey(targetObject);
            //return result;
        }

        public void SetValue(object model, string modelBinding, object val)
        {
            //object targetObject;
            //string lastProperty;
            //var prop = GetLastPropertyIterateThroughPath(model, modelBinding, out targetObject, out lastProperty);

            //if (targetObject.GetType() == typeof(ExpandoObject))
            //{
            //    (targetObject as ExpandoObject).SetValue(lastProperty, val);
            //    return;
            //}

            //if (prop == null || targetObject == null || prop.SetMethod == null)
            //{
            //    return;
            //}

            //if (prop.PropertyType == typeof(Int32))
            //{
            //    prop.SetValue(targetObject, Convert.ToInt32(val));
            //}
            //else if (prop.PropertyType == typeof(Decimal))
            //{
            //    prop.SetValue(targetObject, Convert.ToDecimal(val));
            //}
            //else if (prop.PropertyType == typeof(Decimal?))
            //{
            //    prop.SetValue(targetObject, val == null ? (decimal?)null : Convert.ToDecimal(val));
            //}
            //else if (prop.PropertyType == typeof(Int32?))
            //{
            //    prop.SetValue(targetObject, val == null ? (Int32?)null : Convert.ToInt32(val));
            //}
            //else
            //{
            //    prop.SetValue(targetObject, val);
            //}
        }

        private object GetLastPropertyIterateThroughPath(object model, string modelBinding, out object target, out string lastPropName)
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
                    //var prop = currentObject.GetType().GetProperty(propName);
                    //currentObject = prop.GetValueStringKey(currentObject);
                    currentObject = GetEmittedReflectionValue(currentObject, propName);
                }

                if(currentObject == null)
                {
                    return null;
                }
            }

            target = currentObject;
            var result = GetEmittedReflectionValue(currentObject, lastPropName);
            return result;
            //var lastProp = currentObject.GetType().GetProperty(lastPropName);
            //return lastProp;
        }

        private static readonly Dictionary<string, Func<object, object>> _delegateCache = new Dictionary<string, Func<object, object>>();
        private static readonly Dictionary<string, PropertyInfo> _propertyCache = new Dictionary<string, PropertyInfo>(); 

        private object GetEmittedReflectionValue(object model, string property)
        {
            return _fastReflection.GetValueNestedDictionary(model, property);

            var modelType = model.GetType();
            var key = $"{modelType.FullName}.{property}";
            Func<object, object> emitter;

            if (!_delegateCache.TryGetValue(key, out emitter))
            {
                PropertyInfo propertyInfo;

                if (!_propertyCache.TryGetValue(key, out propertyInfo))
                {
                    propertyInfo = model.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                    _propertyCache[key] = propertyInfo;
                }
 
                emitter = GetPropertyEmitter(modelType, property, propertyInfo).CreateDelegate();
                _delegateCache[key] = emitter;
            }

            var value = emitter(model);
            return value;
        }

        private static Emit<Func<object, object>> GetPropertyEmitter(Type modelType, string property, PropertyInfo propertyInfo)
        {
            return Emit<Func<object, object>>
                    .NewDynamicMethod(property)
                    .LoadArgument(0)
                    .CastClass(modelType)
                    .Call(propertyInfo.GetGetMethod(true)!)
                    .Return();
        }
    }
}
