using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    public class FlowContextWrapper : FlowContextNoModel
    {
        public string ModelFullName { get; set; }
        public virtual object ModelUntyped { get => null; }

        public static FlowContextWrapper CreateInstance(IFlowContext result)
        {
            Type generic = typeof(FlowContextWrapper<>);
            var targetType = generic.MakeGenericType(new Type[] { result.Model.GetType() });
            var dto = Activator.CreateInstance(targetType, result, result.Model) as FlowContextWrapper;
            return dto;
        }

        public static FlowContext DeserializeContext(string json, IKnownTypesBinder knownTypesBinder)
        {
            var obj = SJ.JsonSerializer.Deserialize<JsonModelWrapper>(json);
            var modelTypeName = obj.ModelFullName;
            var mt = typeof(NullFlowModel);

            if (modelTypeName != null)
            {
                if(!knownTypesBinder.KnownTypesDict.ContainsKey(modelTypeName))
                {
                    throw new BlazorFormsModelNotFoundException($"The class {modelTypeName} is not registered for a dynamic serialization");
                }

                mt = knownTypesBinder.KnownTypesDict[modelTypeName];
            }

            var targetType = typeof(FlowContextWrapper<>).MakeGenericType(new Type[] { mt });
            var wrapper = SJ.JsonSerializer.Deserialize(json, targetType) as FlowContextWrapper;
            var fullContext = new FlowContext(wrapper, wrapper.ModelUntyped as IFlowModel);
            return fullContext;
        }
    }

    public class FlowContextWrapper<T> : FlowContextWrapper
        where T : class, IFlowModel
    {
        public T Model { get; set; }

        public override object ModelUntyped { get => Model; }

        public FlowContextWrapper()
        { }

        public FlowContextWrapper(FlowContextNoModel source, IFlowModel model)
        {
            //FlowRunId = source.FlowRunId;
            CurrentTask = source.CurrentTask;
            CurrentTaskLine = source.CurrentTaskLine;
            AssignedUser = source.AssignedUser;
            AssignedTeam = source.AssignedTeam;
            AdminUser = source.AdminUser;
            StatusMessage = source.StatusMessage;
            ExecutionResult = source.ExecutionResult;
            FlowAssembly = source.FlowAssembly;
            FlowName = source.FlowName;
            Id = source.Id;
            RefId = source.RefId;
            CallStack = source.CallStack;
            Params = source.Params;

            Model = model as T;
            ModelFullName = typeof(T).FullName;
        }
    }
}
