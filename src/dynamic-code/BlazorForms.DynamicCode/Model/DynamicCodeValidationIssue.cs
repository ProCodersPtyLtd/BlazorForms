using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.DynamicCode
{
    public class DynamicCodeValidationIssue
    {
        public bool IsError { get; internal set; }
        public string Message { get; internal set; }

        public override string ToString()
        {
            var header = IsError ? "Validation Error: " : "Validation Warning: ";
            return $"{header}{Message}";
        }
    }
}
