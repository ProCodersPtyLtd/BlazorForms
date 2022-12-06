using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public interface IModel
    {

    }

    public interface IFlowModel : IModel
    {
        ExpandoObject Bag { get; }

        Dictionary<string, DynamicRecordset> Ext { get; }

        //IFlowParams FlowParams { get; set; }
    }
}
