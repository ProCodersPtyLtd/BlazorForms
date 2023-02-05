using BlazorForms.Flows.Definitions;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonCompanyLinkDetailsModel : PersonCompanyLinkDetails, IFlowModelListItem
    {
        public virtual bool Changed { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
