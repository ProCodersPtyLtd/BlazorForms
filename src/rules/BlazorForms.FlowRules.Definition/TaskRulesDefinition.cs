using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class TaskRulesDefinition
    {
        public string Name { get; set; }
        public string ProcessTaskTypeFullName { get; set; }

        public Collection<FieldDetails> Fields { get; set; }
    }
}
