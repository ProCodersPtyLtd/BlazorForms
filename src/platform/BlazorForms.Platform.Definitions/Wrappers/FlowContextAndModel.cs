using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public interface IFlowContextAndModel
    {
        object ModelUntyped { get; }
        object ContextUntyped { get; }
    }

    public class FlowContextAndModel<C, M> : IFlowContextAndModel
        where C : class
        where M : class
    {
        private M _model;

        public string ModelFullName { get; set; }
        public M Model { get => _model; set { _model = value; Init(); } }
        public C Context { get; set; }
        public object ModelUntyped { get => Model; }
        public object ContextUntyped { get => Context; }

        public void Init()
        {
            ModelFullName = typeof(M).FullName;
        }
    }
    

    public static class FlowContextAndModelHelper
    {
        public static object CreateInstance(IFlowContext result)
        {
            Type generic = typeof(FlowContextAndModel<,>);
            var targetType = generic.MakeGenericType(new Type[] { typeof(FlowContextNoModel), result.Model.GetType() });

            var dto = Activator.CreateInstance(targetType);
            var modelProp = targetType.GetProperty("Model");
            modelProp.SetValue(dto, result.Model);
            var contextProp = targetType.GetProperty("Context");
            contextProp.SetValue(dto, result);

            return dto;
        }
    }
}
