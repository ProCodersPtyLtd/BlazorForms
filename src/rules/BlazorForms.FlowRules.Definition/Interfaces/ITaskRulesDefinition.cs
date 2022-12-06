using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.FlowRules
{
    public interface ITaskRulesDefinition
    {
        string Name { get; }
        string ProcessTaskTypeFullName { get; }
    }
}
