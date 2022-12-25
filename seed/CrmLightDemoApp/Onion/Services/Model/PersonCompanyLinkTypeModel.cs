using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using System.Dynamic;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonCompanyLinkTypeModel : PersonCompanyLinkType, IFlowModel
    {
        public virtual bool Changed { get; set; }

        // IFlowModel
        public ExpandoObject Bag => new ExpandoObject();

        public Dictionary<string, DynamicRecordset> Ext => new Dictionary<string, DynamicRecordset>();
    }

    public class PersonCompanyLinkTypeListModel : FlowModelBase
    {
        public virtual List<PersonCompanyLinkTypeModel>? Data { get; set; }
        public virtual List<PersonCompanyLinkTypeModel>? Deleted { get; set; } = new List<PersonCompanyLinkTypeModel>();
    }
}
