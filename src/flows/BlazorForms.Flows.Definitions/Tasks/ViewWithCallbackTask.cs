using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class ViewWithCallbackTask : IFlowTask
    {
        protected readonly string _callbackTaskId;
        private ILogStreamer _logStreamer;

        public ViewWithCallbackTask(string callbackTaskId, ILogStreamer logStreamer)
        {
            _callbackTaskId = callbackTaskId;
            _logStreamer = logStreamer;
        }

        public async System.Threading.Tasks.Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer)
        {
            context.ExecutionResult.IsFormTask = true;
            context.ExecutionResult.FormId = taskType.FullName;
            context.ExecutionResult.CallbackTaskId = _callbackTaskId;
            var formState = context?.ExecutionResult.FormState;

            if (formState != FormTaskStateEnum.Submitted)
            {
                logStreamer.TrackException(new FlowStopException("ViewWithCallbackTask - failed"));
                throw new FlowStopException();
            }
        }
    }
}
