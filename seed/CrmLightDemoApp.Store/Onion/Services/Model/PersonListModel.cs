using BlazorForms.Flows.Definitions;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class PersonListModel : IFlowModel
    {
        public virtual List<PersonModel>? Data { get; set; }
    }
}
