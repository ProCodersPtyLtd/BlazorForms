using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class ResolutionModel : IEntity, IFlowModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
    }
}
