using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows
{
    public delegate void FlowEvent(object sender, FlowEventArgs e);

    public class FlowEventArgs
    {
        public string TaskName { get; set; }
        public IFlowContext Context { get; set; }
        public IFlowModel Model { get; set; }
    }
}
