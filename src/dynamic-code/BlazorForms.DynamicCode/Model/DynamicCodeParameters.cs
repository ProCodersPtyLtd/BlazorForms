using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode
{
    public class DynamicCodeParameters
    {
        public List<MetadataReference> References { get; set; }
        public string Code { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }

    }
}
