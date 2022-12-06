using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using BlazorForms.Admin.BusinessObjects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Admin.BusinessObjects.Interfaces
{
    public interface IFlowDataProvider
    {
        Task<List<FlowDataDetails>> GetRegisteredFlows(FlowDataOptions options);
        Task<List<FlowDataDetails>> GetStoredFlows(QueryOptions queryOptions, FlowDataOptions options);
    }
}
