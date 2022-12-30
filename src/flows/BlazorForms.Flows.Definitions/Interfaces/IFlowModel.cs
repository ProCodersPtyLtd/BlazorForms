using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowModel 
    {
    }

    public interface IFlowModelExtended : IFlowModel
    {
        ExpandoObject Bag { get; }

        Dictionary<string, DynamicRecordset> Ext { get; }
    }
}
