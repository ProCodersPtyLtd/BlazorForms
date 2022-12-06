using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowRunParameters
    {
        public Type FlowType { get; set; }
        //public FlowSettings FlowSettings { get; set; }
        public string RefId { get; set; }
        public FlowParamsGeneric FlowParameters { get; set; }
        public bool NoStorageMode { get; set; }
        public bool FirstPass { get; set; } = true;
        public IFlowContext Context { get; set; }
        public IFlowModel Model { get; set; }
    }
}
