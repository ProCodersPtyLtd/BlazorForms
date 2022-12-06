using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using BlazorForms.DynamicCode.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode
{
    public class DynamicCodeContext
    {
        public List<MetadataReference> References { get; internal set; }
        public string Code { get; internal set; }
        public string Namespace { get; internal set; }
        public string ClassName { get; internal set; }
        public string FullName { get; internal set; }

        public string AssemblyName { get; internal set; }
        public Assembly Assembly { get; internal set; }
        public Type ClassType { get; internal set; }
        public EmitResult CompilationResult { get; internal set; }
        public bool Success { get; internal set; }
        public List<DynamicCodeValidationIssue> ValidationIssues { get; private set; } = new List<DynamicCodeValidationIssue>();

        internal DynamicCodeEngine Engine;
    }
}
