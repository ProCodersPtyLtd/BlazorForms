using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public interface IRuleExecutionEngine
    {
        Task<RuleEngineExecutionResult> Execute(RuleExecutionParameters parameters);
    }
}
