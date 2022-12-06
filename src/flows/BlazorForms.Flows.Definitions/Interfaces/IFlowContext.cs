using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowContext : IFlowContextNoModel
    {
        IFlowModel Model { get; set; }
    }
}
