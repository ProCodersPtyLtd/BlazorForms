using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.State
{
    public class FormSubmittedArgs
    {
        public IFlowContext Context { get; set; }
        public IFlowModel Model { get; set; }
    }
}
