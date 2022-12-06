using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class AccessRuleModel
    {
        public string AssignedUser { get; set; }
        public string AssignedTeam { get; set; }

        public bool Allow { get; set; }
        //public IFlowModel Model { get; set; }
    }
}
