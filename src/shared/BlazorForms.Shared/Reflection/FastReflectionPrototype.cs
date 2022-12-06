using BlazorForms.Shared.Extensions;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Reflection
{
    public class FastReflectionPrototype
    {
        private readonly Dictionary<string, Func<object, object>> _delegateCache = new Dictionary<string, Func<object, object>>();
        private readonly Dictionary<string, PropertyInfo> _propertyCache = new Dictionary<string, PropertyInfo>();

        private Func<object, object> _cached;

        public object GetValueMaxSpeed(object model, string property)
        {
            if (_cached != null)
            {
                return _cached(model);
            }

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

                _cached = emitter;
            }

            var value = emitter(model);
            return value;
        }

        public object GetValueStringKey(object model, string property)
        {
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

                _cached = emitter;
            }

            var value = emitter(model);
            return value;
        }

        private readonly Dictionary<Tuple<Type, string>, Func<object, object>> _delegate2Cache = new Dictionary<Tuple<Type, string>, Func<object, object>>();


        public object GetValueTupleKey(object model, string property)
        {
            var modelType = model.GetType();
            //var key = $"{modelType.FullName}.{property}";
            var key = new Tuple<Type, string>(modelType, property);
            Func<object, object> emitter;

            if (!_delegate2Cache.TryGetValue(key, out emitter))
            {
                var propertyInfo = model.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                emitter = GetPropertyEmitter(modelType, property, propertyInfo).CreateDelegate();
                _delegate2Cache[key] = emitter;
            }

            var value = emitter(model);
            return value;
        }

        private readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _delegateNestedCache = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
        private readonly Dictionary<PropertyInfo, Func<object, object>> _delegatePropertiesCache = new Dictionary<PropertyInfo, Func<object, object>>();

        public object GetValuePropertyDictionary(object model, string property)
        {
            var modelType = model.GetType();
            Func<object, object> emitter;
            var propertyInfo = modelType.GetProperty(property);

            if (propertyInfo == null)
            {
                return null;
            }

            if (!_delegatePropertiesCache.TryGetValue(propertyInfo, out emitter))
            {
                emitter = GetPropertyEmitter(modelType, property, propertyInfo).CreateDelegate();
                _delegatePropertiesCache[propertyInfo] = emitter;
            }

            var value = emitter(model);
            return value;

        }

        public object GetValueNestedDictionary(object model, string property)
        {
            var modelType = model.GetType();
            Dictionary<string, Func<object, object>> properties;
            Func<object, object> emitter;

            if (!_delegateNestedCache.TryGetValue(modelType, out properties))
            {
                properties = new Dictionary<string, Func<object, object>>();
                _delegateNestedCache[modelType] = properties;
            }

            if (!properties.TryGetValue(property, out emitter))
            {
                var propertyInfo = model.GetType().GetProperty(property);

                if (propertyInfo == null)
                {
                    return null;
                }

                emitter = GetPropertyEmitter(modelType, property, propertyInfo).CreateDelegate();
                properties[property] = emitter;
            }

            var value = emitter(model);
            return value;
        }

        //Dictionary<string, Func<object, object>> _pps = new Dictionary<string, Func<object, object>>();

        public object GetValueTypeDictionary(object model, string property, Dictionary<string, Func<object, object>> pps)
        {
            Func<object, object> emitter;

            if (!pps.TryGetValue(property, out emitter))
            {
                var propertyInfo = model.GetType().GetProperty(property);

                if (propertyInfo == null)
                {
                    return null;
                }

                emitter = GetPropertyEmitter(model.GetType(), property, propertyInfo).CreateDelegate();
                pps[property] = emitter;
            }

            var value = emitter(model);
            return value;
        }

        private readonly Dictionary<Type, Dictionary<string, Action<object, object>>> _settersNestedCache = new Dictionary<Type, Dictionary<string, Action<object, object>>>();


        public void SetValueNestedDictionary(object model, string property, object value)
        {
            var modelType = model.GetType();
            Dictionary<string, Action<object, object>> properties;
            Action<object, object> emitter;

            if (!_settersNestedCache.TryGetValue(modelType, out properties))
            {
                properties = new Dictionary<string, Action<object, object>>();
                _settersNestedCache[modelType] = properties;
            }

            if (!properties.TryGetValue(property, out emitter))
            {
                var propertyInfo = model.GetType().GetProperty(property);
                emitter = GetSetPropertyEmitter(modelType, property, propertyInfo).CreateDelegate();
                properties[property] = emitter;
            }

            emitter(model, value);
        }

        private readonly Dictionary<object, Dictionary<string, Func<object, object>>> _delegateObjectCache = new Dictionary<object, Dictionary<string, Func<object, object>>>();

        public object GetValueNestedObjectDictionary(object model, string property)
        {
            Dictionary<string, Func<object, object>> properties;
            Func<object, object> emitter;

            if (!_delegateObjectCache.TryGetValue(model, out properties))
            {
                properties = new Dictionary<string, Func<object, object>>();
                _delegateObjectCache[model] = properties;
            }

            if (!properties.TryGetValue(property, out emitter))
            {
                var propertyInfo = model.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                emitter = GetPropertyEmitter(model.GetType(), property, propertyInfo).CreateDelegate();
                properties[property] = emitter;
            }


            var value = emitter(model);
            return value;
        }

        private static Emit<Func<object, object>> GetPropertyEmitter(Type modelType, string property, PropertyInfo propertyInfo)
        {
            var result = Emit<Func<object, object>>
                    .NewDynamicMethod(property)
                    .LoadArgument(0)
                    .CastClass(modelType)
                    .Call(propertyInfo.GetGetMethod(true)!);

            //if (propertyInfo.PropertyType.IsPrimitive || Nullable.GetUnderlyingType(propertyInfo.PropertyType)?.IsPrimitive == true
            //    || Nullable.GetUnderlyingType(propertyInfo.PropertyType) == typeof(decimal))
            if (propertyInfo.PropertyType.IsBoxable())
            {
                result = result.Box(propertyInfo.PropertyType);
            }
            
            result = result.Return();

            return result;
        }

        /// <summary>
        /// Hardcoded level 2 nested property getter emitter
        /// </summary>
        /// <param name="model"></param>
        /// <param name="binding">only supperts level 2 nested "$.N1.V1"</param>
        /// <returns></returns>
        public static Emit<Func<object, object>> GetNestedEmitter(object model, string binding)
        {
            Emit<Func<object, object>>  result;
            var path = binding.Replace("$", "").Split('.').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            var prop1 = path[0];
            var prop2 = path[1];
            var propInfo1 = model.GetType().GetProperty(prop1);
            var propInfo2 = propInfo1.PropertyType.GetProperty(prop2);

            result = Emit<Func<object, object>>.NewDynamicMethod(prop1);
            result.LoadArgument(0).CastClass(model.GetType());
            result.Call(propInfo1.GetGetMethod(true)!);

            using (var a = result.DeclareLocal(propInfo1.PropertyType))
            {
                result.StoreLocal(a);
                result.LoadLocal(a);
            }

            result.CastClass(propInfo1.PropertyType);
            result.Call(propInfo2.GetGetMethod(true)!);

            using (var b = result.DeclareLocal(propInfo2.PropertyType))
            {
                result.StoreLocal(b);
                result.LoadLocal(b);
            }

            result = result.Return();

            return result;
        }

        public static Emit<Func<object, object>> GetJsonPathStraightEmitterGet(Type modelType, string binding)
        {
            Emit<Func<object, object>> result;
            var path = binding.Replace("$", "").Split('.').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (binding.Contains("[") || binding.Contains("]"))
            {
                throw new ArgumentException("Ony straight binding supported: '$.Model.SubModel.SubSubModel.Value'");
            }

            var currentType = modelType;

            result = Emit<Func<object, object>>.NewDynamicMethod(binding.Replace("$", "").Replace(".", ""));
            result.LoadArgument(0);

            foreach(var prop in path)
            {
                var propInfo = currentType.GetProperty(prop);

                result.CastClass(currentType);
                result.Call(propInfo.GetGetMethod(true)!);

                using (var a = result.DeclareLocal(propInfo.PropertyType))
                {
                    result.StoreLocal(a);
                    result.LoadLocal(a);
                }

                currentType = propInfo.PropertyType;
            }

            if (currentType.IsBoxable())
            {
                result.Box(currentType);
            }

            result.Return();
            return result;
        }

        public static Emit<Action<object, object>> GetJsonPathStraightEmitterSet(Type modelType, string binding)
        {
            Emit<Action<object, object>> result;
            var path = binding.Replace("$", "").Split('.').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (binding.Contains("[") || binding.Contains("]"))
            {
                throw new ArgumentException("Ony straight binding supported: '$.Model.SubModel.SubSubModel.Value'");
            }

            var currentType = modelType;
            string prop;
            PropertyInfo propInfo;

            result = Emit<Action<object, object>>.NewDynamicMethod(binding.Replace("$", "").Replace(".", ""));
            result.LoadArgument(0);

            for (int i = 0; i < path.Count-1; i++)
            //foreach (var prop in path)
            {
                prop = path[i];
                propInfo = currentType.GetProperty(prop);

                result.CastClass(currentType);
                result.Call(propInfo.GetGetMethod(true)!);

                using (var a = result.DeclareLocal(propInfo.PropertyType))
                {
                    result.StoreLocal(a);
                    result.LoadLocal(a);
                }

                currentType = propInfo.PropertyType;
            }

            result.CastClass(currentType);
            prop = path.Last();
            propInfo = currentType.GetProperty(prop);

            result.LoadArgument(1);

            if (propInfo.PropertyType.IsBoxable())
            {
                result.UnboxAny(propInfo.PropertyType);
            }
            else
            {
                result.CastClass(propInfo.PropertyType);
            }

            result.Call(propInfo.GetSetMethod(true)!);

            result.Return();
            return result;
        }

        public static Emit<Action<object, object>> GetSetPropertyEmitter(Type modelType, string property, PropertyInfo propertyInfo)
        {
            var result = Emit<Action<object, object>>
                    .NewDynamicMethod(property)
                    .LoadArgument(0)
                    .CastClass(modelType)
                    .LoadArgument(1);

            if (propertyInfo.PropertyType.IsBoxable())
            {
                result = result.UnboxAny(propertyInfo.PropertyType);
                //result = result.Convert(propertyInfo.PropertyType);
            }
            else
            {
                result.CastClass(propertyInfo.PropertyType);
            }

            result = result.Call(propertyInfo.GetSetMethod(true)!)
                .Return();

            return result;
        }
    }
}
