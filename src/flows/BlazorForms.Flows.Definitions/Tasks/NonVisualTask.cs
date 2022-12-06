using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public class NonVisualTask : IFlowTask
    {
        //protected readonly string _callbackTaskId;

        //public NonVisualTask(string callbackTaskId)
        //{
        //    _callbackTaskId = callbackTaskId;
        //}

        public async Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer)
        {
            var task = Activator.CreateInstance(taskType);
            context.ExecutionResult.IsFormTask = true;
            context.ExecutionResult.FormId = taskType.FullName;
            var formState = context?.ExecutionResult.FormState;

            if (formState != FormTaskStateEnum.Submitted)
            {
                throw new FlowStopException();
            }
        }
    }
}
