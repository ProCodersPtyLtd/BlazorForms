using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    public class FlowModelWrapper
    {
        public string ModelFullName { get; set; }
        public virtual object ModelUntyped { get => null; }

        public static FlowModelWrapper CreateInstance(IFlowModel result)
        {
            Type generic = typeof(FlowModelWrapper<>);
            var targetType = generic.MakeGenericType(new Type[] { result.GetType() });
            var dto = Activator.CreateInstance(targetType, result) as FlowModelWrapper;
            return dto;
        }

        public static IFlowModel Deserialize(string json, IKnownTypesBinder knownTypesBinder)
        {
            var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
            var modelTypeName = obj.ModelFullName;
            var mt = typeof(NullFlowModel);

            if (modelTypeName != null)
            {
                mt = knownTypesBinder.KnownTypesDict[modelTypeName];
            }

            var targetType = typeof(FlowModelWrapper<>).MakeGenericType(new Type[] { mt });

            var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as FlowModelWrapper;
            var model = wrapper.ModelUntyped as IFlowModel;
            return model;
        }
    }

    public class FlowModelWrapper<T> : FlowModelWrapper
        where T : class, IFlowModel
    {
        public T Model { get; set; }

        public override object ModelUntyped { get => Model; }

        public FlowModelWrapper()
        { }

        public FlowModelWrapper(IFlowModel model)
        {
            Model = model as T;
            ModelFullName = typeof(T).FullName;
        }
    }
}
