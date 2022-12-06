using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Platform.Crm.Domain.Models.Artel
{
    public class ArtelProjectStatistics
    {
        public virtual Money TotalValue { get; set; } = new Money();
        public virtual int MemberCount { get; set; }
        public virtual int TotalSharesIssued { get; set; }
        public virtual int TotalTimesheetHours { get; set; }
        public virtual Money CurrentBalance { get; set; } = new Money();
    }
}
