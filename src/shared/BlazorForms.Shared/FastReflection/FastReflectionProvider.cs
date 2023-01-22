using BlazorForms.Shared.Extensions;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.FastReflection
{
    public class FastReflectionProvider : IFastReflectionProvider
    {
        private readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _delegateNestedCache = 
            new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        private readonly Dictionary<Type, Dictionary<string, Action<object, object>>> _actionNestedCache =
            new Dictionary<Type, Dictionary<string, Action<object, object>>>();

        public void UpdateBindingFastReflection(FieldBinding binding, Type modelType)
        {
            switch (binding.GetPathType())
            {
                case FieldBindingPathType.Straight:
                    binding.FastReflectionGetter = GetStraightEmitterGet(modelType, binding.Binding);
                    binding.FastReflectionSetter = GetStraightEmitterSet(modelType, binding.Binding);
                    break;

                case FieldBindingPathType.Column:
                    var itemType = JsonPathHelper.GetItemsType(modelType, binding.TableBinding);
                    binding.FastReflectionGetter = GetStraightEmitterGet(itemType, binding.Binding);
                    binding.FastReflectionTableGetter = GetStraightEmitterGet(modelType, binding.TableBinding);
                    binding.FastReflectionSetter = GetStraightEmitterSet(itemType, binding.Binding);

                    if (binding.ItemsBinding != null)
                    {
                        binding.FastReflectionItemsGetter = GetStraightEmitterGet(modelType, binding.ItemsBinding);
                    }

                    break;

                case FieldBindingPathType.SingleSelect:
                    binding.FastReflectionGetter = GetStraightEmitterGet(modelType, binding.Binding);
                    binding.FastReflectionSetter = GetStraightEmitterSet(modelType, binding.Binding);
                    var optionsType = JsonPathHelper.GetItemsType(modelType, binding.ItemsBinding);
                    binding.FastReflectionNameGetter = GetStraightEmitterGet(optionsType, binding.NameBinding);
                    binding.FastReflectionIdGetter = GetStraightEmitterGet(optionsType, binding.IdBinding);
                    binding.FastReflectionItemsGetter = GetStraightEmitterGet(modelType, binding.ItemsBinding);

                    if (binding.TableBinding != null)
                    {
                        binding.FastReflectionTableGetter = GetStraightEmitterGet(modelType, binding.TableBinding);
                    }

                    break;
            }
        }

        public Func<object, object> GetStraightEmitterGet(Type modelType, string binding)
        {
            Dictionary<string, Func<object, object>> properties;
            Func<object, object> emitter;

            if (!_delegateNestedCache.TryGetValue(modelType, out properties))
            {
                properties = new Dictionary<string, Func<object, object>>();
                _delegateNestedCache[modelType] = properties;
            }

            if (!properties.TryGetValue(binding, out emitter))
            {
                emitter = GetJsonPathStraightEmitterGet(modelType, binding).CreateDelegate();
                properties[binding] = emitter;
            }

            return emitter;
        }

        public Action<object, object> GetStraightEmitterSet(Type modelType, string binding)
        {
            Dictionary<string, Action<object, object>> properties;
            Action<object, object> emitter;

            if (!_actionNestedCache.TryGetValue(modelType, out properties))
            {
                properties = new Dictionary<string, Action<object, object>>();
                _actionNestedCache[modelType] = properties;
            }

            if (!properties.TryGetValue(binding, out emitter))
            {
                emitter = GetJsonPathStraightEmitterSet(modelType, binding)?.CreateDelegate();
                properties[binding] = emitter;
            }

            return emitter;
        }

        public Emit<Func<object, object>> GetJsonPathStraightEmitterGet(Type modelType, string binding)
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

            foreach (var prop in path)
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

        public Emit<Action<object, object>> GetJsonPathStraightEmitterSet(Type modelType, string binding)
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

            for (int i = 0; i < path.Count - 1; i++)
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

            // if SetMethod doesn't exist
            if (propInfo.SetMethod == null)
            {
                return null;
            }

            result.Call(propInfo.GetSetMethod(true)!);

            result.Return();
            return result;
        }
    }
}
