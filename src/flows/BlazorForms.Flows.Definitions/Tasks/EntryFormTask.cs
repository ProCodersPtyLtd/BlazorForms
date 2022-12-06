using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public class EntryFormTask : IFlowTask
    {
        public async Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer)
        {
            context.ExecutionResult.IsFormTask = true;
            context.ExecutionResult.FormId = taskType.FullName;
            var formState = context?.ExecutionResult.FormState;

            if (formState != FormTaskStateEnum.Submitted)
            {
                logStreamer.TrackException(new FlowStopException("EntryFormTask - failed"));
                throw new FlowStopException();
            }
        }
    }

    public class EntryFormTask<T> : IFlowTask
      where T : class, IFlowForm, new()
    {
        public async Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer)
        {
            context.ExecutionResult.IsFormTask = true;
            context.ExecutionResult.FormId = typeof(T).Name;
            var formState = context?.ExecutionResult.FormState;

            if (formState != FormTaskStateEnum.Submitted)
            {
                logStreamer.TrackException(new FlowStopException("EntryFormTask - failed"));
                throw new FlowStopException();
            }
        }
    }
}
