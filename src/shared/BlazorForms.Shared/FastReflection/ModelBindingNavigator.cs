using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.FastReflection
{
    public interface IModelBindingNavigator
    {
        object GetValue(object model, FieldBinding modelBinding);
        object GetNameValue(object model, FieldBinding binding);
        object GetIdValue(object model, FieldBinding modelBinding);
        object GetRowValue(object model, FieldBinding binding, int rowIndex);
        IEnumerable<object> GetItems(object model, FieldBinding modelBinding);
        IEnumerable<object> GetItems(object model, string itemsBinding);
        IEnumerable<object> GetTable(object model, FieldBinding modelBinding);
        void SetValue(object model, FieldBinding modelBinding, object val);
    }

    public class ModelBindingNavigator : IModelBindingNavigator
    {
        private readonly IJsonPathNavigator _jsonNavigator;
        //private readonly IFastReflectionProvider _fastReflection;

        public ModelBindingNavigator(IJsonPathNavigator jsonNavigator/*, IFastReflectionProvider fastReflection*/)
        {
            _jsonNavigator = jsonNavigator;
            //_fastReflection = fastReflection;
        }

        public IEnumerable<object> GetItems(object model, FieldBinding modelBinding)
        {
            if (modelBinding.FastReflectionItemsGetter != null)
            {
                return modelBinding.FastReflectionItemsGetter(model) as IEnumerable<object>;
            }

            return _jsonNavigator.GetItems(model, modelBinding.ItemsBinding);
        }

        public IEnumerable<object> GetItems(object model, string itemsBinding)
        {
            return _jsonNavigator.GetItems(model, itemsBinding);
        }

        public IEnumerable<object> GetTable(object model, FieldBinding modelBinding)
        {
            if (modelBinding.FastReflectionTableGetter != null)
            {
                return modelBinding.FastReflectionTableGetter(model) as IEnumerable<object>;
            }

            return _jsonNavigator.GetItems(model, modelBinding.TableBinding);
        }

        public object GetValue(object model, FieldBinding modelBinding)
        {
            if (modelBinding.FastReflectionGetter != null)
            {
                return modelBinding.FastReflectionGetter(model);
            }

            return _jsonNavigator.GetValue(model, modelBinding.Binding);
        }

        public object GetNameValue(object model, FieldBinding modelBinding)
        {
            if (modelBinding.FastReflectionNameGetter != null)
            {
                return modelBinding.FastReflectionNameGetter(model);
            }

            return _jsonNavigator.GetValue(model, modelBinding.NameBinding);
        }

        public object GetIdValue(object model, FieldBinding modelBinding)
        {
            if (modelBinding.FastReflectionIdGetter != null)
            {
                return modelBinding.FastReflectionIdGetter(model);
            }

            return _jsonNavigator.GetValue(model, modelBinding.IdBinding);
        }

        public object GetRowValue(object model, FieldBinding binding, int rowIndex)
        {
            var list = GetTable(model, binding) as System.Collections.IList;
            var listItem = list[rowIndex];

            if (binding.FastReflectionGetter != null)
            {
                return binding.FastReflectionGetter(listItem);
            }

            return _jsonNavigator.GetValue(listItem, binding.Binding);
        }

        public void SetValue(object model, FieldBinding modelBinding, object val)
        {
            //if (modelBinding.FastReflectionSetter != null)
            //{
            //    modelBinding.FastReflectionSetter(model, val);
            //    return;
            //}

            if (!string.IsNullOrWhiteSpace(modelBinding.Binding))
            {
                _jsonNavigator.SetValue(model, modelBinding.Binding, val);
            }
        }

        
    }
}
