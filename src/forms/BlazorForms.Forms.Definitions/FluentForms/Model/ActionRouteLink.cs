using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class ActionRouteLink
    {
        public string Text { get; set; }
        public string LinkText { get; set; }
        public bool IsNavigation { get; set; }
        public Type FlowType { get; set; }
        public FlowReferenceOperation Operation { get; set; }
    }
}
