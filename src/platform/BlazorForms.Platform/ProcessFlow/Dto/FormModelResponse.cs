using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Platform.ProcessFlow.Dto
{
    public class FormModelResponse
    {
        public int FlowRunId { get; set; }
        public string FormName { get; set; }
        public string CallbackTaskName { get; set; }
        public Collection<FieldDetails> Fields { get; set; }
        public Collection<FieldDisplayDetails> DisplayProperties { get; set; }
        public IFlowModel Model { get; set; }
    }
}
