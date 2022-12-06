using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorForms.FlowRules
{
    public interface IRuleDefinitionParser
    {
        TaskRulesDefinition Parse(Type taskType);
        IEnumerable<ExecutableRuleDetails> FindAllRules(IEnumerable<Assembly> assemblies);
        IFlowRule CreateRuleInstance(Type t);
    }
}
