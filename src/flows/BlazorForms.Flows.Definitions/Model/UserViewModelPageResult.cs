using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class UserViewModelPageResult
    {
        public IFlowModel Model { get; set; }
        public IEnumerable<Attribute> MethodTaskAttributes { get; set; }
    }
}
