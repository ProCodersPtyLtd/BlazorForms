using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Platform.Crm.Domain.Models.Artel
{
    public class ArtelMemberStatistics
    {
        public virtual int TotalTimesheetHours { get; set; }
        public virtual int TotalShares { get; set; }
        public virtual Money TotalPaid { get; set; } = new Money();
        public virtual Money OutstandingBalance { get; set; } = new Money();
    }
}
