using BlazorForms.Flows.Definitions;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class ClientCompanyListModel : IFlowModel
    {
        public virtual List<ClientCompanyModel>? Data { get; set; }
    }
}
