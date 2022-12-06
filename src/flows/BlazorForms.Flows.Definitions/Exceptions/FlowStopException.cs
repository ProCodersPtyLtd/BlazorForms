using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowStopException : Exception
    {
        public FlowStopException() : base()
        {
        }

        public FlowStopException(string message) : base(message)
        {
        }
    }
}
