using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class ModelAndClientKeptContextRequestUntyped
    {
        public string ModelFullName { get; set; }
        public ClientKeptContext Ctx { get; set; }
        public FlowParamsGeneric Parameters { get; set; }
        public object Model { get; set; }
    }

    public class ModelAndClientKeptContextRequest
    {
        public string ModelFullName { get; set; }
        public ClientKeptContext Ctx { get; set; }
        public FlowParamsGeneric Parameters { get; set; }
        public virtual object ModelUntyped { get; }
    }

    public class ModelAndClientKeptContextRequest<T> : ModelAndClientKeptContextRequest
        where T : class, IFlowModel
    {
        public FlowModelWrapper<T> Model { get; set; }

        public override object ModelUntyped => Model.ModelUntyped;
    }
}
