using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using System.Dynamic;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonModel : Person, IFlowModel
    {
        public virtual List<PersonCompanyLinkDetails> CompanyLinks { get; set; }

        // IFlowModel
        public virtual ExpandoObject Bag { get; set; } = new ExpandoObject();

        public virtual Dictionary<string, DynamicRecordset> Ext { get; set; } = new Dictionary<string, DynamicRecordset>();
    }
}
