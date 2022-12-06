using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.Crm.Business.Admin
{
    public class BackgroundTaskModel : FlowModelBase
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Duration { get; set; }
        public virtual string State { get; set; }
        public virtual string Details { get; set; }
        public virtual DateTime StartDate { get; set; }  
        public virtual string Type { get; set; }
    }
}
