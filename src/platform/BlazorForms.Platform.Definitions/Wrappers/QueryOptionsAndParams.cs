using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Definitions.Wrappers
{
    public class QueryOptionsAndParams
    {
        public QueryOptions QueryOptions { get; set; }
        public FlowParamsGeneric Params { get; set; }
    }
}
