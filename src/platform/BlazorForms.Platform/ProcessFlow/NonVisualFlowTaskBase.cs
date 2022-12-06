using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.ProcessFlow
{
    public abstract class NonVisualFlowTaskBase<TSource> : FlowTaskDefinitionBase<TSource>, IFlowTask where TSource : class
    {
        public abstract Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer);
    }
}
