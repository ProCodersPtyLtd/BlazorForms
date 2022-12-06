using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class InvalidFlowException : Exception
    {
        public InvalidFlowException() : base()
        {

        }

        public InvalidFlowException(string message) : base(message)
        {

        }
    }
}
