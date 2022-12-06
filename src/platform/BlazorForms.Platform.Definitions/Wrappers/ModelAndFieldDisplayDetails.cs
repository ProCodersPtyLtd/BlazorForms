using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class ModelAndFieldDisplayDetailsUntyped
    {
        public string ModelFullName { get; set; }
        public FieldDisplayDetails[] Details { get; set; }
        public FieldBinding Binding { get; set; }
        public object Model { get; set; }
    }

    public class ModelAndFieldDisplayDetails
    {
        public string ModelFullName { get; set; }
        public FieldDisplayDetails[] Details { get; set; }
        public FieldBinding Binding { get; set; }
        public virtual object ModelUntyped { get; }
    }

    public class ModelAndFieldDisplayDetails<T> : ModelAndFieldDisplayDetails
        where T : class, IFlowModel
    {
        public FlowModelWrapper<T> Model { get; set; }

        public override object ModelUntyped => Model.ModelUntyped;
    }
}
