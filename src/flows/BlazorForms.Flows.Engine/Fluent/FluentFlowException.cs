using System;

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
