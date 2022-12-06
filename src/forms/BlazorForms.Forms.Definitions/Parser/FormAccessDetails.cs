using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormAccessDetails
    {
        public bool OnlyAssignee { get; set; }
        public string Roles { get; set; }
        public FormFlowRuleDetails CustomRule { get; set; }
    }
}
