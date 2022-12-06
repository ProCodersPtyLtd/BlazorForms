using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowModelBase : IFlowModel
    {
        public virtual ExpandoObject Bag { get; set; } = new ExpandoObject();

        public virtual Dictionary<string, DynamicRecordset> Ext { get; set; } = new Dictionary<string, DynamicRecordset>();

        public virtual bool Modified { get; set; }

        //public IFlowParams FlowParams { get; set; }
    }
}
