using BlazorForms.Flows.Definitions;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class CompanyListModel : FlowModelBase
    {
        public virtual List<CompanyModel>? Data { get; set; }
    }
}
