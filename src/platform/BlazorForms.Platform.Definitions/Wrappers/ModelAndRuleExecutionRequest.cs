using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class ModelAndRuleExecutionRequestUntyped
    {
        public string ModelFullName { get; set; }
        public RuleExecutionRequest Request { get; set; }
        public object Model { get; set; }
    }

    public class ModelAndRuleExecutionRequest
    {
        public string ModelFullName { get; set; }
        public RuleExecutionRequest Request { get; set; }
        public virtual object ModelUntyped { get; }
    }

    public class ModelAndRuleExecutionRequest<T> : ModelAndRuleExecutionRequest
        where T : class, IFlowModel
    {
        public FlowModelWrapper<T> Model { get; set; }

        public override object ModelUntyped => Model.ModelUntyped;
    }
}
