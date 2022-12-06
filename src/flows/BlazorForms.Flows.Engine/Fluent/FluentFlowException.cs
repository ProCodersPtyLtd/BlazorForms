using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows
{
    public class FluentFlowException : Exception
    {
        public FluentFlowException()
        {

        }

        public FluentFlowException(string message) : base(message)
        {

        }
    }

    public class FlowInfiniteExecutionException : Exception
    {
        public FlowInfiniteExecutionException()
        {

        }

        public FlowInfiniteExecutionException(string message) : base(message)
        {

        }
    }
}
