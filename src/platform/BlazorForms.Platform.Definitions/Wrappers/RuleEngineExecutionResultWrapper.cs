using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    public class RuleEngineExecutionResultWrapper : RuleEngineExecutionResultNoModel
    {
        public string ModelFullName { get; set; }
        public virtual object ModelUntyped { get; }

        public static RuleEngineExecutionResultWrapper CreateInstance(RuleEngineExecutionResult result)
        {
            Type generic = typeof(RuleEngineExecutionResultWrapper<>);
            var targetType = generic.MakeGenericType(new Type[] { result.Model.GetType() });
            var dto = Activator.CreateInstance(targetType, result, result.Model) as RuleEngineExecutionResultWrapper;
            return dto;
        }

        public static RuleEngineExecutionResult Deserialize(string json, IKnownTypesBinder knownTypesBinder)
        {
            var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
            var modelTypeName = obj.ModelFullName;
            var mt = knownTypesBinder.KnownTypesDict[modelTypeName];
            var targetType = typeof(RuleEngineExecutionResultWrapper<>).MakeGenericType(new Type[] { mt });

            var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as RuleEngineExecutionResultWrapper;
            var result = new RuleEngineExecutionResult(wrapper, wrapper.ModelUntyped);
            return result;
        }
    }

    public class RuleEngineExecutionResultWrapper<T> : RuleEngineExecutionResultWrapper
        where T : class, IFlowModel
    {
        public T Model { get; set; }

        public override object ModelUntyped => Model;

        public RuleEngineExecutionResultWrapper()
        { }

        public RuleEngineExecutionResultWrapper(RuleEngineExecutionResult source, IFlowModel model)
        {
            AccessModel = source.AccessModel;
            Validations = source.Validations;
            FieldsDisplayProperties = source.FieldsDisplayProperties;
            Model = model as T;
            ModelFullName = typeof(T).FullName;
        }
    }
}
