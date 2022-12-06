using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Artel
{
    public class ArtelMemberModel : FlowModelBase
    {
        public virtual ArtelMemberDetails Member { get; set; }
    }
}
