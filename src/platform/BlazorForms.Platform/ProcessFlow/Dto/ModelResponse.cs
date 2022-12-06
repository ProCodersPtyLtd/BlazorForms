using BlazorForms.Flows.Definitions;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Platform.ProcessFlow.Dto
{
    public class ModelResponse
    {
        public int FlowRunId { get; set; }
        public string FormName { get; set; }
        public string CallbackTaskName { get; set; }
        public IFlowModel Model { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string[,] RawData { get; set; }
    }
}
