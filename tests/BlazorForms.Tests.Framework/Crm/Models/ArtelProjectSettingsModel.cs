using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Artel
{
    public class ArtelProjectSettingsModel : FlowModelBase
    {
        public virtual string Message { get; set; }
        public virtual ArtelProjectDetails Project { get; set; }

        public virtual List<ArtelRoleDetails> Roles { get; set; }

        public virtual List<Currency> CurrencyListRef { get; set; }
        public virtual List<FrequencyTypeDetails> FrequencyRef { get; set; }
    }
}
