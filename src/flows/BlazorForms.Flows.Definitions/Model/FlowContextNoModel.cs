using System.Collections.Generic;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows
{
    public class FlowContextNoModel : IFlowContextNoModel
    {
        public FlowContextNoModel()
        {
        }

        public string RefId { get; set; }
        public IEnumerable<string> FlowTags { get; set; }
        public string CurrentTask { get; set; }
        public int CurrentTaskLine { get; set; }
        public string AssignedUser { get; set; }
        public string AssignedTeam { get; set; }
        public string AdminUser { get; set; }
        public string StatusMessage { get; set; }
        public FlowParamsGeneric Params { get; set; }
        public TaskExecutionResult ExecutionResult { get; set; }
        public string FlowAssembly { get; set; }
        public string FlowName { get; set; }
        public string Id { get; set; }
        public List<string> CallStack { get; set; } = new List<string>();

        public ClientKeptContext GetClientContext()
        {
            var source = this;

            var result = new ClientKeptContext
            {
                CurrentTask = source.CurrentTask,
                CurrentTaskLine = source.CurrentTaskLine,
                AssignedUser = source.AssignedUser,
                AssignedTeam = source.AssignedTeam,
                AdminUser = source.AdminUser,
                StatusMessage = source.StatusMessage,
                ExecutionResult = source.ExecutionResult,
                FlowAssembly = source.FlowAssembly,
                FlowName = source.FlowName,
                Id = source.Id,
                RefId = source.RefId,
                CallStack = source.CallStack,
                Params = source.Params,
                FlowTags = source.FlowTags
            };

            return result;
        }

    }
}
