using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Platform.Crm.Domain.Models.Artel
{
    public class ArtelRoleDetails
    {
        public virtual int Id { get; set; }
        public virtual int ArtelProjectId { get; set; }
        public virtual string Name { get; set; }
        public virtual Money HourlyRate { get; set; } = new Money();
    }
}
