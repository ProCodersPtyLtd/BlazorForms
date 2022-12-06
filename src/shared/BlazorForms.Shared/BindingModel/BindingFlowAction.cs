using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class BindingFlowAction
    {
        public string Name { get; set; }
        public string FlowFullName { get; set; }
        public string ActionsBinding { get; set; }
        public FlowReferenceOperation Operation { get; set; }
        public string Tag { get; set; }
        public bool IsNavigation { get; set; }
        public string NavigationFormat { get; set; }
    }
}
