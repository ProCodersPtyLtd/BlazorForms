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

    // ToDo: issue#16 remove Bag and Ext
    public interface IFlowModel : IModel
    {
        ExpandoObject Bag { get; }

        Dictionary<string, DynamicRecordset> Ext { get; }
    }

    public interface IFlowModelExtended : IFlowModel
    {
        ExpandoObject? Bag { get; }

        Dictionary<string, DynamicRecordset>? Ext { get; }
    }
}
