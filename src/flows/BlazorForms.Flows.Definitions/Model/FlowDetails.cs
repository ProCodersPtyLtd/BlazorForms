using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class FlowDetails
    {
        public string Name { get; set; }
        public Type ClassType { get; set; }
        public Dictionary<string, FlowTaskDetails> Tasks { get; internal set; }
    }
}
