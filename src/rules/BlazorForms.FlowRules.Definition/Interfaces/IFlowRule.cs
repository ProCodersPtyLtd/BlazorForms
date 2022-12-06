using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public interface IFlowRule
    {
        string RuleCode { get; }
        RuleTypes RuleType { get; }
        RuleExecutionResult Result { get; }
        void ExecuteUntyped(object model);
        Task ExecuteUntypedAsync(object model, ILogStreamer logStreamer);
        void Initialize(RuleExecutionParameters parameters);
    }

    public interface IFlowRule<M> : IFlowRule where M : class
    {
        void Execute(M model);
    }

    public interface IFlowRuleAsync<M> : IFlowRule where M : class
    {
        Task Execute(M model);
    }

    
}
