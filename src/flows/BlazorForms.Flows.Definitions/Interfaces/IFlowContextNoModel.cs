using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowContextNoModel
    {
        string RefId { get; set; }
        IEnumerable<string> FlowTags { get; set; }
        string CurrentTask { get; set; }
        int CurrentTaskLine { get; set; }
        string AssignedUser { get; set; }
        string AssignedTeam { get; set; }
        string AdminUser { get; set; }
        string StatusMessage { get; set; }
        FlowParamsGeneric Params { get; set; }
        TaskExecutionResult ExecutionResult { get; set; }
        string FlowAssembly { get; set; }
        string FlowName { get; set; }
        string Id { get; set; }
        List<string> CallStack { get; set; }

        ClientKeptContext GetClientContext();
    }
}
