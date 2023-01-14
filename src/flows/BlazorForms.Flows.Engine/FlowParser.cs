using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Flows.Engine
{
    public class FlowParser : IFlowParser
    {
        private IEnumerable<Assembly> _asms;
        private readonly IAssemblyRegistrator _assemblyRegistrator;
        private readonly IKnownTypesBinder _binder;

        public FlowParser(IAssemblyRegistrator assemblyRegistrator, IKnownTypesBinder binder)
        {
            _assemblyRegistrator = assemblyRegistrator;
            _asms = _assemblyRegistrator.GetConsideredAssemblies();
            _binder = binder;
        }

        public void SetConsideredAssemblies(IEnumerable<Assembly> asms)
        {
            _asms = asms;
        }

        public IFlow GetFlowInstance(string flowTypeName)
        {
            var classType = GetTypeByName(flowTypeName);
            var flow = Activator.CreateInstance(classType) as IFlow;
            return flow;
        }

        public Type GetTypeByName(string typeName)
        {
            var classType = _binder.KnownTypesDict.ContainsKey(typeName) ? _binder.KnownTypesDict[typeName] :
                _asms.SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName);

            if (classType == null)
            {
                throw new InvalidOperationException($"Type {typeName} not found, are you missing an assembly reference?");
            }

            return classType;
        }

        public FlowTaskDetails GetMethodTaskDetails(Type flowType, string taskMethodName)
        {
            var method = flowType.GetMethods().Where(m => m.Name == taskMethodName && m.GetCustomAttributes(typeof(TaskAttribute), false).Length > 0).FirstOrDefault();
            var info = GetFlowTaskDetails(method);
            return info;
        }

        public IEnumerable<string> GetRegisteredFlows()
        {
            return _asms
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes().Any(attr => attr.GetType() == typeof(FlowAttribute)))
                .Select(t => t.FullName)
                .ToList();
        }

        public FlowTaskDetails GetTaskDetails(Type type)
        {
            throw new NotImplementedException();
        }

        private FlowTaskDetails GetFlowTaskDetails(MethodInfo info)
        {
            //var nextAttr = info.GetCustomAttribute(typeof(NextTaskAttribute), false) as NextTaskAttribute;
            //var switchAttr = info.GetCustomAttribute(typeof(TaskSwitchAttribute), false) as TaskSwitchAttribute;
            //var formAttr = info.GetCustomAttribute<FormTaskAttribute>();
            //var attributes = info.GetCustomAttributes().Select(a => a.GetType().Name).ToArray();

            var result = new FlowTaskDetails
            {
                Name = info.Name,
                //Attributes = attributes,
                //NextTask = nextAttr?.Task,
                //SwitchTasks = switchAttr?.Tasks,
                //IsFormTask = formAttr != null,
            };

            return result;
        }

        public Type GetDefaultReadonlyForm(string flowTypeName)
        {
            var type = GetTypeByName(flowTypeName);
            var attr = type.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(FlowAttribute)) as FlowAttribute;
            return attr?.DefaultReadonlyView;
        }

        public List<Type> GetTypesInheritedFrom(Type i)
        {
            var result = _asms.SelectMany(a => a.GetTypes()).Where(p => i.IsAssignableFrom(p)).ToList();
            return result;
        }
    }
}
