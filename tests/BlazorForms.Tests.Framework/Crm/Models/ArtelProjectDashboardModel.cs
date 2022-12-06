using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Domain.Models.Artel;

namespace BlazorForms.Platform.Crm.Artel
{
    public class ArtelProjectDashboardModel : FlowModelBase
    {
        public virtual ArtelProjectDetails Project { get; set; }
        public virtual List<ArtelMemberDetails> Members { get; set; }
        public virtual string Message { get; set; }

        public string TotalTimeSubmitted => $"{Project?.Statistics?.TotalTimesheetHours} hour(s)";

        public virtual int? SelectedMemberIndex { get; set; }

        public string PersonalStatName => SelectedMember?.FullName;
        public string PersonalStatEffortSubmitted => $"{SelectedMember?.Statistics?.TotalTimesheetHours} hour(s)";
        public int? PersonalStatShares => SelectedMember?.Statistics?.TotalShares;
        public Money PersonalStatTotalPaid => SelectedMember?.Statistics?.TotalPaid;
        public Money PersonalStatOutstandingBalance => SelectedMember?.Statistics?.OutstandingBalance;

        private ArtelMemberDetails SelectedMember
        {
            get
            {
                if (SelectedMemberIndex != null)
                {
                    return Members[SelectedMemberIndex.Value];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
