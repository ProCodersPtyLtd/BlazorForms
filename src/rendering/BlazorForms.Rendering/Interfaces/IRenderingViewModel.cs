using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Interfaces
{
    public interface IRenderingViewModel
    {
        // helpful api
        void SetBaseUri(string uri);
        Task SaveException();
        void PopulateException([NotNullAttribute] Exception exc);
        void PopulateException(IFlowContextNoModel context);
        Task RunActionFlow(string flowType, FlowParamsGeneric parameters, bool showResult = true);
    }
}
