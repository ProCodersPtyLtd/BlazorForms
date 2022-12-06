using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.Reflection
{
    public interface IModelNavigator
    {
        object GetValueObject(string modelBinding);
        string GetValueString(string modelBinding);
        void SetValue(string modelBinding, object val);
        IEnumerable<object> GetItems(string itemsBinding);
        string GetValue(string modelBinding);
        DateTime? GetValueDate(string modelBinding);
        bool? GetValueBool(string modelBinding);
        decimal? GetValueDecimal(string modelBinding);

        object GetValue(string tableBinding, int rowIndex, string modelBinding);
        void SetValue(string tableBinding, int rowIndex, string modelBinding, object val);

        void SetModel(object model);
    }
    public class ModelNavigator : IModelNavigator
    {
        private readonly IJsonPathNavigator _jsonPathNavigator;
        private object Model;

        public ModelNavigator(IJsonPathNavigator jsonPathNavigator)
        {
            _jsonPathNavigator = jsonPathNavigator;
        }

        public IEnumerable<object> GetItems(string itemsBinding)
        {
            var result = _jsonPathNavigator.GetItems(Model, itemsBinding);
            return result ?? (new object[] { });
        }

        public object GetValueObject(string modelBinding)
        {
            var result = _jsonPathNavigator.GetValue(Model, modelBinding);
            return result;
        }

        public string GetValueString(string modelBinding)
        {
            var result = _jsonPathNavigator.GetValue(Model, modelBinding);
            return result?.ToString();
        }

        public void SetValue(string modelBinding, object val)
        {
            _jsonPathNavigator.SetValue(Model, modelBinding, val);
        }

        public void SetModel(object model)
        {
            Model = model;
        }

        public string GetValue(string modelBinding)
        {
            return GetValueString(modelBinding);
        }

        public DateTime? GetValueDate(string modelBinding)
        {
            return (DateTime?)GetValueObject(modelBinding);
        }

        public bool? GetValueBool(string modelBinding)
        {
            return (bool?)GetValueObject(modelBinding);
        }

        public decimal? GetValueDecimal(string modelBinding)
        {
            return (decimal?)GetValueObject(modelBinding);
        }

        public object GetValue(string tableBinding, int rowIndex, string modelBinding)
        {
            var list = GetItems(tableBinding) as System.Collections.IList;
            var listItem = list[rowIndex];
            var result = _jsonPathNavigator.GetValue(listItem, modelBinding);
            return result;
        }

        public void SetValue(string tableBinding, int rowIndex, string modelBinding, object val)
        {
            var list = GetItems(tableBinding) as System.Collections.IList;
            var listItem = list[rowIndex];
            _jsonPathNavigator.SetValue(listItem, modelBinding, val);
        }
    }
}
