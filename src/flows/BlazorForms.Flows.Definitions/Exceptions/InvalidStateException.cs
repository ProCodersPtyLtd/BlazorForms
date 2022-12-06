using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException() : base()
        {

        }

        public InvalidStateException(string message) : base(message)
        {

        }
    }
}
