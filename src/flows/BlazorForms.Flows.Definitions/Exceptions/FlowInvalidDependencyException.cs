using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowInvalidDependencyException : Exception
    {
        public FlowInvalidDependencyException() : base()
        {

        }

        public FlowInvalidDependencyException(string message) : base(message)
        {

        }
    }
}
