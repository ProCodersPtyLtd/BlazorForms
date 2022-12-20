using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public abstract class FormOptionsBase
    {
        public string? DateFormat { get; set; }
        public bool AllowAnonymousAccess { get; set; }
        public bool AllowFlowStorage { get; set; }
        public bool SupressExceptions { get; set; }
    }
}
