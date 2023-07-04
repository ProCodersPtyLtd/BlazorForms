using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class PersonCompanyLinkModel : IEntity, IFlowModelListItem
    {
        // entity
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int CompanyId { get; set; }
        public int LinkTypeId { get; set; }
        public bool Deleted { get; set; }

        // Model
        public PersonModel? Person { get; set; }
        public CompanyModel? Company { get; set; }

        // ToDo: what to do with Model logical fields, that should not be saved to the Storage
        public bool Changed { get; set; }
    }
}
