using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowFailedException : Exception
    {
        public FlowFailedException() : base()
        {
        }

        public FlowFailedException(string message) : base(message)
        {
        }
    }
}
