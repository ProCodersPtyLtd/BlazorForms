using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowTask
    {
        Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer);
        //void Skip();
    }
}
