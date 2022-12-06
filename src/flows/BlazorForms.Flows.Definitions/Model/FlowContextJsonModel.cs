using System.Collections.Generic;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows
{
    public class FlowContextJsonModel : FlowContext
    {
        public string ModelJson { get; set; }
        public string ModelType { get; set; }

        public FlowContextJsonModel()
        {
        }

        public FlowContextJsonModel(IFlowContextNoModel source, IFlowModel model)
        {
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
