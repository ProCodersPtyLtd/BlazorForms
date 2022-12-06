using BlazorForms.Shared.Extensions;
using System.Collections.Generic;

namespace BlazorForms.Flows.Definitions
{
    public class FlowModelsQueryOptions
    {
        public string FlowName { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> RefIds { get; set; }
        public QueryOptions QueryOptions { get; set; }
        public IEnumerable<FlowStatus> FlowStatuses { get; set; }
        public bool SearchAnyTag { get; set; }
    }
}
