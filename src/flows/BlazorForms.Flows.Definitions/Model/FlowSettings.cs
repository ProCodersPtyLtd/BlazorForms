using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows
{
    public class FlowSettings
    {
        public FlowExecutionStoreModel StoreModel { get; set; }
    }

    public enum FlowExecutionStoreModel
    {
        Full = 1,
        FullNoHistory,
        NoStoreTillStop
    }
}
