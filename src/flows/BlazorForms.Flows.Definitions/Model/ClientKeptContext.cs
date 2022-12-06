using System;
using System.Collections.Generic;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows
{
    public class ClientKeptContext : FlowContextNoModel
    {
        public ClientKeptContext()
        {
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(RefId);
        }
    }
}
