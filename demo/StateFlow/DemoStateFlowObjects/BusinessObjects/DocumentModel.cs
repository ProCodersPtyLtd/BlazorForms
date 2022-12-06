using Newtonsoft.Json;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public class Document
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string AccountId { get; set; }
        public string FoundIssue { get; set; }
        public string CreatedUser { get; set; }
    }

    public class DocumentModel : FlowModelBase
    {
        public string RefId { get; set; }
        public string State { get; set; }
        public string StatusMessage { get; set; }

        // it is important to ignore reference to context, because context has a reference to model
        // and it will be an infinite loop of serialization
        [JsonIgnore]
        public IFlowContext FlowContext { get; set; }

        public Document Document { get; set; }
        public bool Selected { get; set; }
        public string Resolution { get; set; }
        public string AssignedUser { get; set; }
        public string TriggerSelectedValue { get; set; }
    }
}
