using System;

namespace BlazorForms.FlowRules
{
    public class RuleExecutionInfiniteLoopException : Exception
    {
        public RuleExecutionInfiniteLoopException(string message) : base(message) { }
    }
}
