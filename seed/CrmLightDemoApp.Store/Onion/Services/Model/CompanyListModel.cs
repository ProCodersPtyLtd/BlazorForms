using BlazorForms.Flows.Definitions;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class CompanyListModel : IFlowModel
    {
        public virtual List<CompanyModel>? Data { get; set; }
    }
}
