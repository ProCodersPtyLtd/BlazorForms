using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Admin.BusinessObjects.Model
{
    public class FlowDetailsModel : FlowModelBase
    {
        public virtual List<FlowDataDetails>? Data { get; set; }
    }

    public class FlowDataDetails : FlowModelBase
    {
        public virtual string? RefId { get; set; }
        //public virtual string? FlowName { get; set; }
        public virtual string FlowType { get; set; }
        public virtual int? TaskCount { get; set; }
        public virtual string? ModelType { get; set; }
        public virtual string? ModelJson { get; set; }
        public virtual string? DocumentType { get; set; }
        public virtual string? TenantId { get; set; }
        public virtual IEnumerable<string>? FlowTags { get; set; }
        public virtual FlowStatus FlowStatus { get; set; }
        public virtual string? Version { get; set; }

        public string FlowState { get; set; }
        public string ResultState { get; set; }
        public string CurrentTask { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ChangedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
    }
}
