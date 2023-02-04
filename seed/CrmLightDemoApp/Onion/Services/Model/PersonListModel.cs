using BlazorForms.Flows.Definitions;
using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class PersonListModel : IFlowModel
    {
        public virtual List<Person>? Data { get; set; }
    }
}
