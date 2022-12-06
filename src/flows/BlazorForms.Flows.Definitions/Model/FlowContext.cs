using System.Collections.Generic;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows
{
    public class FlowContext : FlowContextNoModel, IFlowContext
    {
        public IFlowModel Model { get; set; }

        public FlowContext()
        {
        }

        public FlowContext(IFlowContextNoModel source, IFlowModel model)
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
            Model = model;
            FlowTags = source.FlowTags;
        }
    }
}
