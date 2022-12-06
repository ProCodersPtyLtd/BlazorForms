using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    // Context
    public class CheckFormAccessDataUntyped
    {
        public string ModelFullName { get; set; }
        public FormAccessDetails Access { get; set; }
        public object Context { get; set; }
    }

    public class CheckFormAccessData
    {
        public string ModelFullName { get; set; }
        public FormAccessDetails Access { get; set; }
        public virtual object ContextUntyped { get; }
    }

    public class CheckFormAccessData<T> : CheckFormAccessData
        where T : class, IFlowModel
    {
         public FlowContextWrapper<T> Context { get; set; }

        public override object ContextUntyped => Context;
    }

    // Model
    public class CheckFormAccessModelDataUntyped
    {
        public string ModelFullName { get; set; }
        public FormAccessDetails Access { get; set; }
        public FlowParamsGeneric FlowParams { get; set; }
        public UserViewAccessInformation AccessInfo { get; set; }
        public object Model { get; set; }
    }

    public class CheckFormAccessModelData
    {
        public string ModelFullName { get; set; }
        public FormAccessDetails Access { get; set; }
        public FlowParamsGeneric FlowParams { get; set; }
        public UserViewAccessInformation AccessInfo { get; set; }
        public virtual object ModelUntyped { get; }
    }

    public class CheckFormAccessModelData<T> : CheckFormAccessModelData
        where T : class, IFlowModel
    {
        public FlowModelWrapper<T> Model { get; set; }

        public override object ModelUntyped => Model;
    }
}
