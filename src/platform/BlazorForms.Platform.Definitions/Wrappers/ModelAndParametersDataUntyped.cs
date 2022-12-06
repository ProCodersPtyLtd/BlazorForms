using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class ModelAndParametersDataUntyped
    {
        public string ModelFullName { get; set; }
        public FlowParamsGeneric Params { get; set; }
        public object Model { get; set; }
    }

    public class ModelAndParameters
    {
        public string ModelFullName { get; set; }
        public FlowParamsGeneric Params { get; set; }
        public virtual object ModelUntyped { get; }
    }

    public class ModelAndParameters<T> : ModelAndParameters
        where T : class, IFlowModel
    {
        public FlowModelWrapper<T> Model { get; set; }

        public override object ModelUntyped => Model;
    }
}
