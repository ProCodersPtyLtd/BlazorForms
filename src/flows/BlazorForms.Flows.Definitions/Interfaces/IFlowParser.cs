using System;
using System.Collections.Generic;
using System.Reflection;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowParser
    {
        // task described as a class
        FlowTaskDetails GetTaskDetails(Type type);
        // task described as a method
        FlowTaskDetails GetMethodTaskDetails(Type type, string taskMethodName);

        IFlow GetFlowInstance(string flowTypeName);
        Type GetTypeByName(string flowTypeName);
        List<Type> GetTypesInheritedFrom(Type i);
        Type GetDefaultReadonlyForm(string flowTypeName);

        IEnumerable<string> GetRegisteredFlows();
        void SetConsideredAssemblies(IEnumerable<Assembly> asms);
    }
}
