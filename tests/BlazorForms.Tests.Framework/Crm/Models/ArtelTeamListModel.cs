using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Artel
{
    public class ArtelTeamListModel : FlowModelBase
    {
        public virtual List<ArtelMemberDetails> Members { get; set; }
    }
}
