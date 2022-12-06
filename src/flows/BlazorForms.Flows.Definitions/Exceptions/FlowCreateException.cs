using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowCreateException : Exception
    {
        public FlowCreateException() : base()
        {

        }

        public FlowCreateException(string message) : base(message)
        {

        }
    }
}
