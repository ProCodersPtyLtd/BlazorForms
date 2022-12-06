using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Definitions.Model
{
    //[ProxyScope]
    public class ErrorModel : FlowModelBase
    {
        public virtual string Message { get; set; }
        public virtual string Type { get; set; }
        public virtual string StackTrace { get; set; }
        public virtual DateTime Created { get; set; }
    }
}
