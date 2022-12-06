using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public interface IFlowRunIdGenerator
    {
        Task<int> GetNextFlowRunId();
    }
}
